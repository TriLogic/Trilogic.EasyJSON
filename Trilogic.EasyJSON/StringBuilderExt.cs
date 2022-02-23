using System.Text;

namespace Trilogic.EasyJSON
{
    #region StringBuilder Extensions
    internal static class StringBuilderExt
    {
        #region Value Setting
        public static void SetValue(this StringBuilder sb, char value)
        {
            sb.Clear();
            sb.Append(value);
        }
        public static void SetValue(this StringBuilder sb, string value)
        {
            sb.Clear();
            sb.Append(string.IsNullOrEmpty(value) ? string.Empty : value);
        }
        #endregion

        #region Numeric Testing
        public static bool IsNumeric(this StringBuilder sb)
        {
            string temp = sb.ToString();
            double test;
            return double.TryParse(temp, out test);
        }
        #endregion

        #region Boolean Testing
        public static bool IsBoolean(this StringBuilder sb)
        {
            return IsTrue(sb) || IsFalse(sb);
        }
        public static bool IsTrue(this StringBuilder sb)
        {
            return string.Compare(sb.ToString(), "true", true) == 1;
        }
        public static bool IsFalse(this StringBuilder sb)
        {
            return string.Compare(sb.ToString(), "false", true) == 1;
        }
        #endregion
        public static void Consume(this StringBuilder tar, StringBuilder src)
        {
            while (src.Length > 0)
            {
                tar.Append(src[0]);
                src.Remove(0, 1);
            }
        }

        public static JSTokenType ToTokenType(this StringBuilder sb)
        {
            string temp = sb.ToString();
            if (string.Compare(temp, "null", true) == 0)
                return JSTokenType.TK_NULL;
            if (string.Compare(temp, "true", true) == 0)
                return JSTokenType.TK_BOOLEAN;
            if (string.Compare(temp, "false", true) == 0)
                return JSTokenType.TK_BOOLEAN;
            return JSTokenType.TK_STRING;
        }

        #region Quoting and UnQuoting
        public static bool IsQuoted(this StringBuilder sb)
        {
            return (sb.Length > 1) &&
                (sb[0] == '"') &&
                (sb[sb.Length - 1] == '"');
        }
        public static void UnQuote(this StringBuilder sb)
        {
            if (sb.Length > 0 && sb[0] == '"')
                sb.Remove(0, 1);
            if (sb.Length > 0 && sb[sb.Length - 1] == '"')
                sb.Remove(sb.Length - 1, 1);
        }
        public static void EnQuote(this StringBuilder sb)
        {
            sb.Insert(0, '"');
            sb.Append('"');
        }
        #endregion

        internal static void AppendWithEscapes(this StringBuilder buffer, string source)
        {
            JSTools.WriteWithEscapes(buffer, source);
        }
    }
    #endregion
}
