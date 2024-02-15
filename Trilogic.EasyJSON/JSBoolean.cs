using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trilogic.EasyJSON
{
    public class JSBoolean : JSItem
    {
        #region Class Members
        internal bool _value;
        #endregion

        #region Overrides
        internal JSBoolean(JSItem parent, bool value) : base(parent) { _value = value; }
        public override dynamic Value { get =>  _value; set => _value = value; }
        public override string ToString() => _value ? "true" : "false";
        public override bool IsBoolean => true;
        public override bool GetBoolean() => (bool)Value;
        #endregion
    }
}
