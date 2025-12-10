using System.Collections.Generic;
using System.Text;

namespace Simple.TextTemplates
{
    public abstract class TagTokenizer
    {
        #region Class Members
        protected ITextSource? mBuf;
        protected int mIdx = 0;
        #endregion

        #region Constructors and Destructors
        public TagTokenizer(ITextSource buf)
        {
            Reset(buf);
        }
        #endregion

        #region Reset
        public void Reset(ITextSource buf)
        {
            this.mBuf = buf;
            this.mIdx = 0;
        }
        #endregion

        #region Abstract Methods
        public abstract TemplateToken? GetToken();
        #endregion

        #region Get Tokens
        public virtual List<TemplateToken> GetTokens()
        {
            List<TemplateToken> list = new List<TemplateToken>();
            TemplateToken? token;
            while ((token = GetToken()) != null)
                list.Add(token);
            return list;
        }
        public virtual List<TemplateToken> GetTokens(ITextSource pattern)
        {
            Reset(pattern);
            return GetTokens();
        }
        public virtual List<TemplateToken> GetTokens(string pattern)
        {
            Reset(new StringTextSource(pattern));
            return GetTokens();
        }
        public virtual List<TemplateToken> GetTokens(StringBuilder pattern)
        {
            Reset(new StringBuilderTextSource(pattern));
            return GetTokens();
        }
        public virtual List<TemplateToken> GetTokens(char[] pattern)
        {
            Reset(new CharArrayTextSource(pattern));
            return GetTokens();
        }
        #endregion
    }

    public class StringTagTokenizer : TagTokenizer
    {
        #region Constructors and Destructors
        public StringTagTokenizer(ITextSource buf) : base(buf) { }
        #endregion

        #region Token Retreival
        public override TemplateToken? GetToken()
        {
            int offst = mIdx;
            int chars = 0;

            while (mIdx < mBuf.Length)
            {
                char tkc = mBuf[mIdx];

                // Is it a closure? 
                if (tkc == '}')
                {
                    // We have encountered a closure token but if we have chrs already consumed
                    // we allow them to take precedence as we return a text token.
                    if (chars > 0)
                    {
                        return new TemplateToken(TemplateTokenType.TkTXT, offst, chars);
                    }

                    // increment to the next char
                    mIdx++;

                    // return the closure token
                    return new TemplateToken(TemplateTokenType.TkRHT, mIdx, 1);
                }

                // Is this an Open Token, an Escaped token or Orphan '$' ???
                if (tkc == '$')
                {
                    // Is this an escaped token or a start token?
                    if (mIdx + 1 < mBuf.Length)
                    {
                        // Escaped token?
                        if (mBuf[mIdx + 1] == '$' || mBuf[mIdx + 1] == '}')
                        {
                            // We have encountered an escaped token but if we have chrs already consumed
                            // we allow them to take precedence as we return a text token.
                            if (chars > 0)
                            {
                                return new TemplateToken(TemplateTokenType.TkTXT, offst, chars);
                            }

                            // Increment past the escaped token
                            mIdx += 2;

                            // Return the escaped token
                            return new TemplateToken(TemplateTokenType.TkTXT, offst, 2, true);
                        }

                        // Is it a start token?
                        if (mBuf[mIdx + 1] == '{')
                        {
                            // We have encountered an escaped token but if we have chrs already consumed
                            // we allow them to take precedence as we return a text token.
                            if (chars > 0)
                            {
                                return new TemplateToken(TemplateTokenType.TkTXT, offst, chars);
                            }

                            // Increment past the start token
                            mIdx += 2;

                            // Return the start token
                            return new TemplateToken(TemplateTokenType.TkLHT, offst, 2, false);
                        }

                        // It's just an orphaned '$' that should have been escaped!
                        // so we let that slip on by as part of the text.
                    }


                    // We have encountered an escaped token but if we have chrs already consumed
                    // we allow them to take precedence as we return a text token.
                    if (chars > 0)
                    {
                        return new TemplateToken(TemplateTokenType.TkTXT, offst, chars);
                    }

                    // Move the index past the escaped tokens
                    mIdx += 2;

                    // Return the escaped token
                    return new TemplateToken(TemplateTokenType.TkTXT, offst, 2, true);
                }

                // Process a single char, add it to the list and move on
                chars++;
                mIdx++;
            }

            if (chars > 0)
            {
                return new TemplateToken(TemplateTokenType.TkTXT, offst, chars);
            }

            // No more tokens
            return null;
        }

        public static List<TemplateToken> Tokenize(ITextSource pattern)
        {
            return new StringTagTokenizer(pattern).GetTokens();
        }
        public static List<TemplateToken> Tokenize(string pattern)
        {
            return new StringTagTokenizer(new StringTextSource(pattern)).GetTokens();
        }
        public static List<TemplateToken> Tokenize(StringBuilder pattern)
        {
            return new StringTagTokenizer(new StringBuilderTextSource(pattern)).GetTokens();
        }
        public static List<TemplateToken> Tokenize(char[] pattern)
        {
            return new StringTagTokenizer(new CharArrayTextSource(pattern)).GetTokens();
        }
        #endregion
    }

    public class HandlebarTagTokenizer : TagTokenizer
    {
        #region Constructors and Destructors
        public HandlebarTagTokenizer(ITextSource buf) : base(buf) { }
        #endregion

        #region Token Retreival
        public override TemplateToken? GetToken()
        {
            int offst = mIdx;
            int chars = 0;

            while (mIdx < mBuf.Length)
            {
                char tkc = mBuf[mIdx];

                // Possible Escape \{ or \} or \\
                if (tkc == '\\' && mIdx + 1 < mBuf.Length && (mBuf[mIdx + 1] == '{' || mBuf[mIdx + 1] == '}' || mBuf[mIdx + 1] == '\\') )
                {
                    // We have encountered an escaped token but if we have chrs already consumed
                    // we allow them to take precedence as we return a text token.
                    if (chars > 0)
                    {
                        return new TemplateToken(TemplateTokenType.TkTXT, offst, chars);
                    }

                    // Move the index past the escaped tokens
                    mIdx += 2;

                    // return the escaped token
                    return new TemplateToken(TemplateTokenType.TkTXT, offst, 2, true);
                }

                // LHT - Close (allow fall through on single '{')
                if (tkc == '{' && mIdx + 1 < mBuf.Length && mBuf[mIdx + 1] == '{')
                {
                    // We have encountered a start token but if we have chrs already consumed
                    // we allow them to take precedence as we return a text token.
                    if (chars > 0)
                    {
                        var result = new TemplateToken(TemplateTokenType.TkTXT, offst, chars);
                        return result;
                    }

                    // Return a left hand token
                    mIdx += 2;
                    return new TemplateToken(TemplateTokenType.TkLHT, offst, 2);
                }

                // RHT - Close (allow fall through on single '}')
                if (tkc == '}' && mIdx + 1 < mBuf.Length && mBuf[mIdx + 1] == '}')
                {
                    // We have encountered an end token but if we have chrs already consumed
                    // we allow them to take precedence as we return a text token.
                    if (chars > 0)
                    {
                        // 1. Create the token result
                        // 3. Do not increase the index, reprocess the existing one
                        // 4. Return the result
                        var result = new TemplateToken(TemplateTokenType.TkTXT, offst, chars);
                        return result;
                    }

                    // Return is a left hand token
                    mIdx += 2;
                    return new TemplateToken(TemplateTokenType.TkRHT, offst, 2);
                }

                // Process a single char, add it to the list and move on
                chars++;
                mIdx++;
            }

            // If we have chars consumed but not returned then return them now.
            if (chars > 0)
            {
                return new TemplateToken(TemplateTokenType.TkTXT, offst, chars);
            }

            // No more tokens
            return null;
        }

        public static List<TemplateToken> Tokenize(ITextSource pattern)
        {
            return new HandlebarTagTokenizer(pattern).GetTokens();
        }
        public static List<TemplateToken> Tokenize(string pattern)
        {
            return new HandlebarTagTokenizer(new StringTextSource(pattern)).GetTokens();
        }
        public static List<TemplateToken> Tokenize(StringBuilder pattern)
        {
            return new HandlebarTagTokenizer(new StringBuilderTextSource(pattern)).GetTokens();
        }
        public static List<TemplateToken> Tokenize(char[] pattern)
        {
            return new HandlebarTagTokenizer(new CharArrayTextSource(pattern)).GetTokens();
        }
        #endregion
    }


}
