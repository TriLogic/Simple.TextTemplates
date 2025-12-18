using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.TextTemplates
{
    #region Tag Lookup Delegate
    public delegate string? TagLookup(string Key);
    #endregion

    public class TextTemplate
    {
        #region Class members
        private ITextSource? mSource;
        private List<TemplateToken>? mTokens;
        #endregion

        #region Constructors & Destructors
        internal TextTemplate()
        {
        }
        #endregion

        #region Internal Properties
        internal List<TemplateToken>? Tokens
        {
            get { return mTokens; }
            set { mTokens = value ?? new List<TemplateToken>(); }
        }

        internal ITextSource? Source
        {
            get { return mSource; }
            set { mSource = value; }
        }
        #endregion

        #region Replace Tags
        public void ReplaceTags(StringBuilder target, TagLookup lookup)
        {
            if (mTokens == null)
            {
                throw new Exception("Missing or invalid template");
            }
            
            Stack<TemplateToken> stack = new();
            StringBuilder keyBuilder = new StringBuilder();

            // assign a default lookup
            if (lookup == null)
            {
                lookup = (string key) => $"[{key}]";
            }

            // Process the tokens
            foreach (TemplateToken tkn in mTokens)
            {
                // Text Token
                if (tkn.IsTXT)
                {
                    // Nothing on the stack so copy this text directly to the target
                    if (stack.Count == 0)
                    {
                        if (tkn.IsEscaped)
                        {
                            target.Append(mSource.Substring(tkn.TokenOffset + 1, 1));
                        }
                        else
                        {
                            target.Append(mSource.Substring(tkn.TokenOffset, tkn.TokenLength));
                        }
                    }
                    else
                    {
                        // This text is intended to be part of a key or a complete key value.
                        stack.Peek().TokenLength += tkn.TokenLength;
                        keyBuilder.Append(mSource.Substring(tkn.TokenOffset, tkn.TokenLength));
                    }
                    continue;
                }

                // Left hand tag token "${"
                if (tkn.IsLHT)
                {
                    // Push a clone of the token  to the top of the stack. The tokens
                    // offset is the current length of the keyValue StringBuilder.
                    stack.Push(tkn.Clone(keyBuilder.Length, 0));
                    continue;
                }

                // Right hand tag token "}"
                if (tkn.IsRHT)
                {
                    // If TOS token is not a LHT then we have a mismatch error on the tag strcuture,
                    // furthermore if the length of the tag is zero then we have an empty tag.
                    if (stack.Count < 1 || !stack.Peek().IsLHT || stack.Peek().TokenLength == 0)
                        throw new Exception($"Invalid template: mismatch at offset {tkn.TokenOffset}");

                    // Retrieve the key value for the current token
                    TemplateToken keyToken = stack.Pop();
                    string keyValue = keyBuilder.ToString(keyToken.TokenOffset, keyToken.TokenLength);

                    // Remove the key value text from the keyBuilder
                    keyBuilder.Remove(keyToken.TokenOffset, keyToken.TokenLength);

                    // Ask the value provider for the value that corresponds to the current key.
                    // It is now the job of the provider to throw an error if that's approriate.
                    string txtValue = lookup(keyValue) ?? string.Empty;

                    // If the value returned has length
                    // non-empty stack indicates we are working on a compound key
                    if (stack.Count > 0)
                    {
                        keyBuilder.Append(txtValue);
                        stack.Peek().TokenLength += txtValue.Length;
                    }
                    else
                    {
                        target.Append(txtValue);
                    }
                }
            }

            // If the stack is not empty we have a mismatch somewhere in the template
            if (stack.Count != 0)
                throw new Exception($"Invalid template: mismatch");
        }

        public StringBuilder ReplaceTags(TagLookup lookup)
        {
            StringBuilder target = new();
            ReplaceTags(target, lookup);
            return target;
        }

        public StringBuilder ReplaceTags(Dictionary<string, string> values, string? onMissing = null, bool ignoreCase = true)
        {
            TagLookup caseLookup = (string key) => { 
                if (values.ContainsKey(key))
                    return values[key];
                return values.ContainsKey(key)
                    ? values[key]
                    : onMissing;
            };
            TagLookup icaseLookup = (string key) => {
                var ikey = values.Keys.Where(x => string.Compare(x, key, StringComparison.CurrentCultureIgnoreCase) == 0).FirstOrDefault();
                return ikey == null
                    ? onMissing
                    : values[ikey];
            };

            var lookup = ignoreCase ? icaseLookup : caseLookup;
            return ReplaceTags(lookup);
        }
        #endregion

        #region Static Compile Method
        public static TextTemplate Compile(ITextSource source)
        {
            List<TemplateToken> tokens = HandlebarsTagTokenizer.Tokenize(source);
            Stack<TemplateToken> stack = new Stack<TemplateToken>();

            foreach (TemplateToken token in tokens)
            {
                if (token.IsLHT)
                {
                    stack.Push(token);
                }

                if (token.IsRHT)
                {
                    if (stack.Count < 1)
                        throw new Exception($"Invalid template: mismatch at offset {token.TokenOffset}");

                    stack.Pop();
                }
            }

            if (stack.Count > 0)
            {
                throw new Exception($"Invalid template: mismatch at offset {stack.Peek().TokenOffset}");
            }

            // Return new textTemplate
            return new TextTemplate()
            {
                mSource = source,
                mTokens = tokens
            };
        }
        #endregion
    }
}
