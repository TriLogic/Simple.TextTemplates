using System;

namespace Simple.TextTemplates
{
    public enum TemplateTokenType
    {
        TkNIL = 0,
        TkLHT = 1,
        TkTXT = 2,
        TkRHT = 3
    }


    public class TemplateToken
    {
        #region Class Members
        public TemplateTokenType _type;
        public int _offset;
        public int _length;
        #endregion

        public TemplateToken()
        {
            _type = TemplateTokenType.TkNIL;
            _offset = 0;
            _length = 0;
        }

        public TemplateToken(TemplateTokenType type)
        {
            _type = type;
            _offset = 0;
            _length = 0;
        }
        public TemplateToken(TemplateTokenType type, int offset, int length)
        {
            _type = type;
            _offset = offset;
            _length = length;
        }

        #region Helper Methods
        public virtual void SetTXT(int offset, int length)
        {
            _type = TemplateTokenType.TkTXT;
            _offset = offset;
            _length = length;
        }
        public void SetLHT(int offset, int length)
        {
            _type = TemplateTokenType.TkLHT;
            _offset = offset;
            _length = length;
        }
        public void SetRHT(int offset, int length)
        {
            _type = TemplateTokenType.TkLHT;
            _offset = offset;
            _length = length;
        }
        public void SetNIL()
        {
            _type = TemplateTokenType.TkNIL;
            _offset = 0;
            _length = 0;
        }
        #endregion

        #region Properties
        public TemplateTokenType TokenType { get => _type; set => _type = value; }
        public int TokenOffset { get => _offset; set => _offset = value; }
        public int TokenLength { get => _length; set => _length = value; }
        public bool IsNIL
        {
            get { return _type == TemplateTokenType.TkNIL; }
        }

        public bool IsLHT
        {
            get { return _type == TemplateTokenType.TkLHT; }
        }

        public bool IsTXT
        {
            get { return _type == TemplateTokenType.TkTXT; }
        }

        public bool IsRHT
        {
            get { return _type == TemplateTokenType.TkRHT; }
        }
        #endregion

        #region Cloning
        public TemplateToken Clone()
        {
            return new TemplateToken(TokenType, TokenOffset, TokenLength);
        }
        public TemplateToken Clone(int offset)
        {
            return new TemplateToken(TokenType, offset, TokenLength);
        }
        public TemplateToken Clone(int offset, int length)
        {
            return new TemplateToken(TokenType, offset, length);
        }
        #endregion

    }

}
