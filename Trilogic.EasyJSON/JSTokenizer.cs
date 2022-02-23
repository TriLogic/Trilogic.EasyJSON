using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trilogic.EasyJSON
{
    internal enum JSTokenType
    {
        TK_EOF = 0,
        TK_NULL = 1,
        TK_WHITE = 32,
        TK_COMMENT_S = 47,
        TK_COMMENT_M = 42,
        TK_STRING = 34,
        TK_COMMA = 44,
        TK_NUMBER = 48,
        TK_COLON = 58,
        TK_BOOLEAN = 84,
        TK_ARRAYL = 91,
        TK_ARRAYR = 93,
        TK_OBJECTL = 123,
        TK_OBJECTR = 125
    }

    internal class JSTokenizer
    {
        #region Class Members
        private JSBuffer mInput = new JSBuffer();
        private StringBuilder mValue = new StringBuilder();
        private StringBuilder mSubValue = new StringBuilder();
        private JSTokenType mTypeT;
        #endregion

        #region Constructors and Destructors
        public JSTokenizer(StringBuilder input)
        {
            Reset(input);
        }
        public JSTokenizer(string input)
        {
            Reset(input);
        }
        #endregion

        public JSBuffer InputStream
        {
            get => mInput;
        }

        public StringBuilder TokenValue
        {
            get => mValue;
        }
        public StringBuilder TokenSubValue
        {
            get => mSubValue;
        }

        public JSTokenType TokenType
        {
            get => mTypeT;
        }

        #region Reset Methods
        public void Reset(string input)
        {
            mInput = new JSBuffer(input);
            mValue.Clear();
            mSubValue.Clear();
            mTypeT = JSTokenType.TK_EOF;
        }
        public void Reset(StringBuilder input)
        {
            mInput = new JSBuffer(input);
            mValue.Clear();
            mSubValue.Clear();
            mTypeT = JSTokenType.TK_EOF;
        }
        #endregion

        public bool GetToken()
        {
            char ch = mInput.PeekC();

            // eof
            if (ch == '\0')
                return false;

            // whitespace
            if (char.IsWhiteSpace(ch))
                return ParseWhite();

            // comments
            if (ch == '\\')
                return ParseComment();

            // numeric
            if ((ch >= '0' && ch <= '9') || (ch == '+') || (ch == '-') || (ch == '.'))
                return ParseNumeric(ch);

            if (ch == '"')
                return ParseString();

            if (ch == '[' || ch == ',' || ch == ':' || ch == '{' || ch == '}' || ch == ']')
            {
                ch = mInput.GetC();
                mTypeT = (JSTokenType)ch;
                mValue.SetValue(ch);
                return true;
            }

            return ParseConstant(ch);
        }

        public bool ParseConstant(char ch)
        {
            if (ch == 't' && ParseExact("true"))
            {
                mTypeT = JSTokenType.TK_BOOLEAN;
                return true;
            }
            if (ch == 'f' && ParseExact("false"))
            {
                mTypeT = JSTokenType.TK_BOOLEAN;
                return true;
            }
            if (ch == 'n' && ParseExact("null"))
            {
                mTypeT = JSTokenType.TK_NULL;
                return true;
            }
            return false;
        }

        public bool ParseExact(string expect)
        {
            mValue.Clear();
            for(int i=0; i < expect.Length; i++)
            {
                char ch = mInput.PeekC();
                if (ch != expect[i])
                {
                    mInput.UngetC(mValue.Length);
                    return false;
                }
                mValue.Append(mInput.GetC());        
            }
            return true;
        }
  
        #region Parsing Whitespace
        public bool ParseWhite()
        {
            mTypeT = JSTokenType.TK_WHITE;

            mValue.SetValue(mInput.GetC());

            char c = mInput.PeekC();
            while (c != '\0' && char.IsWhiteSpace(c))
            {
                mValue.Append(mInput.GetC());
                c = mInput.PeekC();
            }

            return true;
        }
        #endregion

        #region Parsing Comments
        public bool ParseComment()
        {
            mValue.SetValue(mInput.GetC());

            switch (mInput.PeekC())
            {
                case '/':
                    return ParseCommentS();

                case '*':
                    return ParseCommentM();
            }

            return false;
        }
        
        public bool ParseCommentS()
        {
            mTypeT = JSTokenType.TK_COMMENT_S;

            mValue.Append(mInput.GetC());

            char c = mInput.PeekC();
            while (c != 0 && c != '\r' && c != '\n')
                mValue.Append(mInput.GetC());

            return true;
        }

        public bool ParseCommentM()
        {
            mTypeT = JSTokenType.TK_COMMENT_M;

            char c = mInput.GetC();
            mValue.SetValue(c);

            c = mInput.PeekC();
            while (c != '\0')
            {
                if (c == '*')
                {
                    mValue.Append(c);
                    if (mInput.PeekC() == '/')
                    {
                        mValue.Append(mInput.GetC());
                        return true;
                    }
                }
            }

            return false;
        }
        #endregion

        #region Parsing Numeric
        public bool ParseNumeric(char ch)
        {
            mTypeT = JSTokenType.TK_NUMBER;

            mValue.Clear();
            mSubValue.Clear();

            // sign char ?
            if (ch == '+' || ch == '-')
                goto FoundSign;

            // decimal ?
            if (ch == '.')
                goto FoundDecimal;

            // numeric
            goto PreDecimal;

        FoundSign:

            // append the sign char
            mValue.Append(mInput.GetC());

            // decimal already?
            if (mInput.PeekC() == '.')
                goto FoundDecimal;

        PreDecimal:

            if (ParseDigits())
                mValue.Consume(mSubValue);

            // decimal ?
            ch = mInput.PeekC();
            if (ch == '.')
                goto FoundDecimal;

            if (ch == 'e' || ch == 'E')
                goto FoundExponent;

            // should be end of input test it;
            if (mValue.IsNumeric())
                return true;

            goto InvalidNumeric;

        FoundDecimal:

            mValue.Append(mInput.GetC());

        PostDecimal:

            if (ParseDigits())
                mValue.Consume(mSubValue);

            ch = mInput.PeekC();

            if (ch == 'e' || ch == 'E')
                goto FoundExponent;

            if (mValue.IsNumeric())
                return true;

            goto InvalidNumeric;

        FoundExponent:

            mValue.Append(mInput.GetC());

            ch = mInput.PeekC();
            if (ch == '+' || ch == '-')
                mValue.Append(mInput.GetC());

            if (!ParseDigits())
                goto InvalidNumeric;

            mValue.Consume(mSubValue);
            if (mValue.IsNumeric())
                return true;

        InvalidNumeric:

            mInput.UngetC(mValue.Length);
            return false;
        }

        public bool ParseDigits()
        {
            mSubValue.Clear();

            char ch = mInput.PeekC();
            while (ch >= '0' && ch <= '9')
            {
                mSubValue.Append(mInput.GetC());
                ch = mInput.PeekC();
            }

            return mSubValue.Length > 0;
        }
        #endregion

        #region Parsing Strings
        public bool ParseString()
        {
            mTypeT = JSTokenType.TK_STRING;

            char c = mInput.GetC();

            // store initial " value
            mValue.SetValue(c);

            c = mInput.PeekC();
            while (c > '\0')
            {
                if (c == '"')
                {
                    mValue.Append(mInput.GetC());
                    return true;
                }
                else if (c == '\\')
                {
                    if (!ParseEscapeChar(false))
                        return false;
                    mValue.Consume(mSubValue);
                }
                else
                {
                    mValue.Append(mInput.GetC());
                }
                c = mInput.PeekC();
            }

            return false;
        }

        public bool ParseEscapeChar(bool IsIdentity, bool IsFirstChar = false)
        {
            // store initial '\' char
            mSubValue.SetValue(mInput.GetC());

            if (mInput.EOF)
            {
                mInput.UngetC(mSubValue.Length);
                return false;
            }

            // get the next letter
            char c = mInput.GetC();

            // required to be letter of digit or unicode escape letter or digit?
            if (IsIdentity && (c != 'u' && c != 'U'))
            {
                mInput.UngetC(2);
                return false;
            }

            mSubValue.Append(c);
            switch (c)
            {
                case '"':
                    mSubValue.SetValue(c);
                    return true;

                case '\\':
                    mSubValue.SetValue(c);
                    return true;

                case '/':
                    mSubValue.SetValue(c);
                    return true;

                case 'b':
                    mSubValue.SetValue('\b');
                    return true;

                case 'f':
                    mSubValue.SetValue('\f');
                    return true;

                case 'n':
                    mSubValue.SetValue('\n');
                    return true;

                case 'r':
                    mSubValue.SetValue('\r');
                    return true;

                case 't':
                    mSubValue.SetValue('\t');
                    return true;

                case 'u':
                    if (ParseUnicodeQuad())
                        return true;
                    break;
            }

            // there was an error
            mInput.UngetC(mSubValue.Length);
            return false;
        }

        public bool ParseUnicodeQuad()
        {
            int count = 0;

            while (ParseUnicodeNibble() && (count < 4))
                count += 1;

            return count == 4;
        }

        public bool ParseUnicodeNibble()
        {
            if (!JSTools.IsUnicodeNibble(mInput.PeekC()))
                return false;
            mSubValue.Append(mInput.GetC());
            return true;
        }
        #endregion


        public bool TokenIsBoolean
        {
            get => mValue.IsBoolean();
        }
        public bool TokenIsTrue
        {
            get => mValue.IsTrue();
        }

        public bool TokenAsBoolean
        {
            get { return string.Compare(mValue.ToString(), "true", true) == 0; }
        }
        public double TokenAsNumber
        {
            get { return double.Parse(mValue.ToString()); }
        }
        public string TokenAsString
        {
            get
            {
                JSTools.EscapedToString(mValue, true);
                return mValue.ToString();
            }
        }
    }
}
