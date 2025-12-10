using System.Collections.Generic;
using System.Text;

namespace Simple.TextTemplates
{
    public abstract class TagTokenizer
    {
        #region Class Members
        protected ITextSource mBuf;
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

                // Closure
                if (tkc == '}')
                {
                    if (chars > 0)
                        return new TemplateToken(TemplateTokenType.TkTXT, offst, chars);

                    mIdx++;
                    return new TemplateToken(TemplateTokenType.TkRHT, mIdx, 1);
                }

                // LHT or Escape
                if (tkc == '$')
                {
                    // Is there room for a LHT or Escape?
                    if (mIdx + 1 < mBuf.Length)
                    {
                        // Is this an escape - $$ or $}?
                        if (mBuf[mIdx + 1] == '$' || mBuf[mIdx + 1] == '}')
                        {
                            if (chars > 0)
                                return new TemplateToken(TemplateTokenType.TkTXT, offst, chars);

                            mIdx += 2;
                            return new TemplateToken(TemplateTokenType.TkTXT, offst + 1, 1);
                        }

                        // Is this a LHT - ${
                        if (mBuf[mIdx + 1] == '{')
                        {
                            if (chars > 0)
                                return new TemplateToken(TemplateTokenType.TkTXT, offst, chars);

                            mIdx += 2;
                            return new TemplateToken(TemplateTokenType.TkLHT, offst, 2);
                        }

                        // Dangling '$' char fall though
                    }
                }

                // add to the growing list of characters
                chars++;
                mIdx++;
            }

            // if we have a token value
            if (chars > 0)
                return new TemplateToken(TemplateTokenType.TkTXT, offst, chars);

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
        #region Class Members
        ITextSource mBuf;

        int mIdx = 0;
        #endregion

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

                // LHT - Close (allow fall through on single '{')
                if (tkc == '{' && mIdx + 1 < mBuf.Length && mBuf[mIdx + 1] == '{')
                {
                    // we have a start token but let used chars take precedence
                    if (chars > 0)
                    {
                        // 1. Create the token result
                        // 3. Do not increase the index, reprocess the existing one
                        // 4. Return the result
                        var result = new TemplateToken(TemplateTokenType.TkTXT, offst, chars);
                        return result;
                    }

                    // there were no chars use so return a left hand token
                    mIdx += 2;
                    return new TemplateToken(TemplateTokenType.TkRHT, offst, 2);
                }

                // RHT - Close (allow fall through on single '}')
                if (tkc == '}' && mIdx + 1 < mBuf.Length && mBuf[mIdx + 1] == '}')
                {
                    // we have an end token but let used chars take precedence
                    if (chars > 0)
                    {
                        // 1. Create the token result
                        // 3. Do not increase the index, reprocess the existing one
                        // 4. Return ther result
                        var result = new TemplateToken(TemplateTokenType.TkTXT, offst, chars);
                        return result;
                    }

                    // this is a left hand token
                    mIdx += 2;
                    return new TemplateToken(TemplateTokenType.TkRHT, offst, 2);
                }

                // Possible Escape \{ or \}
                if (tkc == '\\' && mIdx + 1 < mBuf.Length && (mBuf[mIdx + 1] == '{' || mBuf[mIdx + 1] == '}'))
                {
                    // stored chars take precedence
                    if (chars > 0)
                        return new TemplateToken(TemplateTokenType.TkTXT, offst, chars);

                    // it is an escaped \{ or \} so move past it to allow it to become part of the text
                    mIdx += 2;
                    chars += 2;
                    continue;
                }

                // processing a single char, add it to the list and move on
                chars++;
                mIdx++;
            }

            // if we have a token value
            if (chars > 0)
                return new TemplateToken(TemplateTokenType.TkTXT, offst, chars);

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
