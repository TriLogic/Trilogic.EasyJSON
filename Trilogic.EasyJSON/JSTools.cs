using System.IO;
using System.Text;

namespace Trilogic.EasyJSON
{
    internal class JSTools
    {
        internal static string _nibbles = "0123456789abcdef";

        public static bool EscapeSeqToChar(StringBuilder buffer, int offset)
        {
            if (offset + 1 >= buffer.Length)
                return false;

            if (buffer[offset] != '\\')
                return false;

            char ch = buffer[offset + 1];
            switch (ch)
            {
                case '\0':  // \0
                    buffer[offset] = '\0';
                    buffer.Remove(offset + 1, 1);
                    return true;

                case '"':  // \"
                    buffer[offset] = '"';
                    buffer.Remove(offset + 1, 1);
                    return true;

                case '\\':  // \\
                    buffer[offset] = '\\';
                    buffer.Remove(offset + 1, 1);
                    return true;

                case '/':  // \/
                    buffer[offset] = '/';
                    buffer.Remove(offset + 1, 1);
                    return true;

                case 'b':  // \b
                    buffer[offset] = '\b';
                    buffer.Remove(offset + 1, 1);
                    return true;

                case 't':   // \t
                    buffer[offset] = '\t';
                    buffer.Remove(offset + 1, 1);
                    return true;

                case 'n': // \n
                    buffer[offset] = '\n';
                    buffer.Remove(offset + 1, 1);
                    return true;

                case 'f': // \f
                    buffer[offset] = '\f';
                    buffer.Remove(offset + 1, 1);
                    return true;

                case 'r': // \r
                    buffer[offset] = '\r';
                    buffer.Remove(offset + 1, 1);
                    return true;

                case 'u':
                    return EscapeToUnicode(buffer, offset);
            }

            buffer.Remove(offset, 1);
            return false;
        }

        public static int StringToEscaped(StringBuilder buffer)
        {
            int initial = buffer.Length;
            int offset = 0;

            while (offset < buffer.Length)
            {
                int incr = CharToEscapeSeq(buffer, offset);
                offset += (1 + incr);
            }
            return (buffer.Length - initial);
        }

        public static int EscapedToString(StringBuilder buffer, bool StripQuotes = true)
        {
            if (StripQuotes)
                buffer.UnQuote();

            int initial = buffer.Length;
            int offset = 0;

            while (offset < buffer.Length)
            {
                EscapeSeqToChar(buffer, offset);
                offset += 1;
            }
            return (buffer.Length - initial);
        }

        public static int CharToEscapeSeq(StringBuilder buffer, int offset)
        {
            if (offset >= buffer.Length)
                return 0;

            char ch = buffer[offset];
            if (ch >= 0 && ch <= 127)
            {
                switch (ch)
                {
                    // escape characters
                    case '\0':  // \0
                        buffer[offset] = '0';
                        buffer.Insert(offset, '\\');
                        return 1;

                    case '"':  // \"
                        buffer[offset] = ch;
                        buffer.Insert(offset, '\\');
                        return 1;

                    case '\\':  // \\
                        buffer[offset] = ch;
                        buffer.Insert(offset, '\\');
                        return 1;

                    case '/':  // \/
                        buffer[offset] = ch;
                        buffer.Insert(offset, '\\');
                        return 1;

                    case '\b':  // \b
                        buffer[offset] = 'b';
                        buffer.Insert(offset, '\\');
                        return 1;

                    case '\t':   // \t
                        buffer[offset] = 't';
                        buffer.Insert(offset, '\\');
                        return 1;

                    case '\n': // \n
                        buffer[offset] = 'n';
                        buffer.Insert(offset, '\\');
                        return 1;

                    case '\f': // \f
                        buffer[offset] = 'f';
                        buffer.Insert(offset, '\\');
                        return 1;

                    case '\r': // \r
                        buffer[offset] = 'r';
                        buffer.Insert(offset, '\\');
                        return 1;
                }
            }
            else
            {
                JSTools.UnicodeToEscape(buffer, offset);
                return 5;
            }

            return 0;
        }

        // replaces buffer[offset] with it's unicode escape value
        public static void UnicodeToEscape(StringBuilder buffer, int offset)
        {
            char ch = buffer[offset];

            // replacing the last char ?
            if (offset + 1 >= buffer.Length)
            {
                // Yes, remove it and append
                buffer.Remove(offset, 1);
                JSTools.AppendUnicodeEscape(buffer, ch);
            }
            else
            {
                // No, remove it and insert
                buffer.Remove(offset, 1);
                JSTools.InsertUnicodeEscape(buffer, offset, ch);
            }
        }

        // replaces the unicode escape sequence beginning at offset with its appropriate unicode char
        public static bool EscapeToUnicode(StringBuilder buffer, int offset)
        {
            // is there enough room for a valid unicode escape
            if (offset < 0 || offset + 5 >= buffer.Length)
                return false;

            // is the escape sequence a valid unicde escape?
            if (buffer[offset] != '\\' && !(buffer[offset + 1] == 'u' || buffer[offset + 1] == 'U'))
                return false;

            if (!(IsUnicodeNibble(buffer, offset + 2)
                && IsUnicodeNibble(buffer, offset + 3)
                && IsUnicodeNibble(buffer, offset + 4)
                && IsUnicodeNibble(buffer, offset + 5)))
                return false;

            int uch = ToUnicodeNibble(buffer, offset + 5)
                | (ToUnicodeNibble(buffer, offset + 4) << 4)
                | (ToUnicodeNibble(buffer, offset + 3) << 8)
                | (ToUnicodeNibble(buffer, offset + 2) << 16);

            // set the char
            buffer[offset] = (char)uch;
            buffer.Remove(offset + 1, 5);

            return true;
        }

        // appends the escaped unicode value for ch
        public static void AppendUnicodeEscape(StringBuilder buffer, char ch)
        {
            buffer.Append("\\u0000");
            SetUnicodeNibbles(buffer, buffer.Length - 4, ch);
        }

        // insert a unicode escape sequence for char ch at the specified offset
        internal static void InsertUnicodeEscape(StringBuilder buffer, int offset, char ch)
        {
            buffer.Insert(offset, "\\u0000");
            SetUnicodeNibbles(buffer, offset + 2, ch);
        }

        internal static void WriteUnicodeEscape(StreamWriter writer, char c)
        {
            writer.Write("@\\u");
            WriteUnicodeNibbles(writer, c);
        }

        // returns true if the char seq at buffer[offset] is a valie unicode escape
        public static bool IsUnicodeEscape(dynamic buffer, int offset)
        {
            if (buffer[offset] != '\\' && !(buffer[offset + 1] == 'u' || buffer[offset + 1] == 'U'))
                return false;

            return buffer.IsUnicodeNibble(offset + 2)
                && buffer.IsUnicodeNibble(offset + 3)
                && buffer.IsUnicodeNibble(offset + 4)
                && buffer.IsUnicodeNibble(offset + 5);
        }

        // returns the nibble value of the buffer[offset]
        public static int ToUnicodeNibble(StringBuilder buffer, int offset)
        {
            char ch = buffer[offset];
            if (ch >= 'a' && ch <= 'f')
                return (10 + (ch - 'a'));
            if (ch >= 'A' && ch <= 'F')
                return (10 + (ch - 'A'));
            if (ch >= '0' && ch <= '9')
                return (ch - '0');

            return 0;
        }
        public static char ToUnicodeNibble(string buffer, int offset)
        {
            char ch = buffer[offset];
            if (ch >= 'a' && ch <= 'f')
                return (char)(10 + (ch - 'a'));
            if (ch >= 'A' && ch <= 'F')
                return (char)(10 + (ch - 'A'));
            if (ch >= '0' && ch <= '9')
                return (char)(10 + (ch - '0'));

            return (char)0;
        }

        public static bool IsUnicodeNibble(char ch)
        {
            return (
                (ch >= 'a' && ch <= 'f') ||
                (ch >= 'A' && ch <= 'F') ||
                (ch >= '0' && ch <= '9')
                );
        }

        // returns true if buffer[offset] is a valid unicde nibble
        public static bool IsUnicodeNibble(StringBuilder buffer, int offset)
        {
            return IsUnicodeNibble(buffer[offset]);
        }

        public static bool IsUnicodeNibble(string buffer, int offset)
        {
            return IsUnicodeNibble(buffer[offset]);
        }

        // sets values 0123 for \u0123
        public static void SetUnicodeNibbles(StringBuilder buffer, int offset, char ch)
        {
            buffer[offset] = _nibbles[(ch / 4096) & 0x0F];
            buffer[offset + 1] = _nibbles[(ch / 256) & 0x0F];
            buffer[offset + 2] = _nibbles[(ch / 16) & 0x0F];
            buffer[offset + 3] = _nibbles[(ch) & 0x0F];
        }

        public static void WriteUnicodeNibbles(StreamWriter writer, char ch)
        {
            writer.Write(_nibbles[(ch / 4096) & 0x0F]);
            writer.Write(_nibbles[(ch / 256) & 0x0F]);
            writer.Write(_nibbles[(ch / 16) & 0x0F]);
            writer.Write(_nibbles[(ch) & 0x0F]);
        }
        public static bool IsQuoted(StringBuilder buffer)
        {
            return buffer.IsQuoted();
        }
        public static void UnQuote(StringBuilder buffer)
        {
            buffer.UnQuote();
        }
        public static void EnQuote(StringBuilder buffer)
        {
            buffer.Insert(0, '"').Append('"');
        }

        public static bool StartsWith(StringBuilder buffer, string value, bool IgnoreCase = false)
        {
            if (buffer.Length < value.Length)
                return false;
            if (IgnoreCase)
            {
                for (int i = 0; i < value.Length; i++)
                    if (buffer[i] != value[i])
                        return false;
            }
            else
            {
                for (int i = 0; i < value.Length; i++)
                    if (char.ToLower(buffer[i]) != char.ToLower(value[i]))
                        return false;
            }
            return true;
        }

        public static void WriteWithEscapes(StreamWriter writer, string source)
        {
            for (int idx = 0; idx < source.Length; idx++)
            {
                if (source[idx] >= 0 && source[idx] <= 127)
                {
                    switch (source[idx])
                    {
                        // printable
                        default:
                            writer.Write(source[idx]);
                            break;
                        // escape characters
                        case (char)34:  // \"
                            writer.Write("\\\"");
                            break;
                        case (char)92:  // \\
                            writer.Write(@"\\");
                            break;
                        case (char)47:  // \/
                            writer.Write(@"\/");
                            break;
                        case (char)08:  // \b
                            writer.Write(@"\b");
                            break;
                        case (char)9:   // \t
                            writer.Write(@"\t");
                            break;
                        case (char)0xA: // \n
                            writer.Write(@"\n");
                            break;
                        case (char)0xC: // \f
                            writer.Write(@"\f");
                            break;
                        case (char)0xD: // \r
                            writer.Write(@"\r");
                            break;
                    }
                }
                else
                {
                    JSTools.WriteUnicodeEscape(writer, source[idx]);
                }
            }
        }
        public static void WriteWithEscapes(StringBuilder buffer, string source)
        {
            for (int idx = 0; idx < source.Length; idx++)
            {
                if (source[idx] >= 0 && source[idx] <= 127)
                {
                    switch (source[idx])
                    {
                        // printable
                        default:
                            buffer.Append(source[idx]);
                            break;
                        // escape characters
                        case (char)34:  // \"
                            buffer.Append("\\\"");
                            break;
                        case (char)92:  // \\
                            buffer.Append("\\\\");
                            break;
                        case (char)47:  // \/
                            buffer.Append("\\/");
                            break;
                        case (char)08:  // \b
                            buffer.Append("\\b");
                            break;
                        case (char)9:   // \t
                            buffer.Append("\\t");
                            break;
                        case (char)0xA: // \n
                            buffer.Append("\\n");
                            break;
                        case (char)0xC: // \f
                            buffer.Append("\\f");
                            break;
                        case (char)0xD: // \r
                            buffer.Append("\\r");
                            break;
                    }
                }
                else
                {
                    JSTools.AppendUnicodeEscape(buffer, source[idx]);
                }
            }
        }
    }

}

