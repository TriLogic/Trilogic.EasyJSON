using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trilogic.EasyJSON
{
    public class JSNumber : JSItem
    {
        #region Class Members
        internal double _value;
        #endregion

        #region Overrides
        public JSNumber(double value) : base(null) { Value = value; }
        internal JSNumber(JSItem parent, double value) : base(parent) { Value = value; }
        public override dynamic Value { get => _value; set => _value = value; }
        public override string ToString() => _value.ToString();
        public override bool IsNumber => true;

        public override byte ToByte() => (byte)(double)Value;
        public override int ToInteger() => (int)(double)Value;
        public override long ToLong() => (long)(double)Value;
        public override float ToFloat() => (float)(double)Value;
        public override double ToDouble() => (double)Value;
        public override bool HasNumericContent { get => true; }

        #endregion
    }
}
