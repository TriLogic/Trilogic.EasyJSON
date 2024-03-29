﻿using System.IO;
using System.Text;

namespace Trilogic.EasyJSON
{
    public class JSString : JSItem
    {
        internal string _value = string.Empty;

        internal JSString(JSItem parent, string value) : base(parent) => Value = value;

        public override dynamic Value {
            get => _value;
            set => _value = string.IsNullOrEmpty(value) ? string.Empty : value;
        }
        public override bool IsString => true;

        public override string ToString()
        {
            if (string.IsNullOrEmpty(_value))
                return "\"\"";
            StringBuilder buffer = new StringBuilder();
            Write(buffer);
            return buffer.ToString();
        }
        public override void Write(StringBuilder builder)
        {
            builder.Append('"');
            builder.AppendWithEscapes(_value);
            builder.Append('"');
        }

        public override void Write(StreamWriter writer)
        {
            writer.Write('"');
            writer.WriteWithEscapes(_value);
            writer.Write('"');
        }

        #region ToNumeric Overrides
        public override double GetNumber()
        {
            double result = 0;
            if (double.TryParse((string)Value, out result))
                return result;
            throw new System.Exception("Invalid Numeric");
        }
        public override byte GetByte()
        {
            byte result = 0;
            if (byte.TryParse((string)Value, out result))
                return result;
            throw new System.Exception("Invalid Byte");
        }
        public override int GetInteger()
        {
            int result = 0;
            if (int.TryParse((string)Value, out result))
                return result;
            throw new System.Exception("Invalid Integer");
        }
        public override long GetLong()
        {
            long result = 0;
            if (long.TryParse((string)Value, out result))
                return result;
            throw new System.Exception("Invalid Long");
        }
        public override float GetFloat()
        {
            float result = 0;
            if (float.TryParse((string)Value, out result))
                return result;
            throw new System.Exception("Invalid Float");
        }
        public override double GetDouble()
        {
            double result = 0;
            if (double.TryParse((string)Value, out result))
                return result;
            throw new System.Exception("Invalid Double");
        }
        public override bool HasNumericContent 
        {
            get 
            {
                double result = 0;
                return (double.TryParse((string)Value, out result));
            }
        }
        #endregion

    }
}
