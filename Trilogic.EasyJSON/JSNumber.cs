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

        public override double GetNumber() => (double)Value;
        public override byte GetByte() => (byte)(double)Value;
        public override int GetInteger() => (int)(double)Value;
        public override long GetLong() => (long)(double)Value;
        public override float GetFloat() => (float)(double)Value;
        public override double GetDouble() => (double)Value;
        public override bool HasNumericContent { get => true; }

        #endregion
    }
}
