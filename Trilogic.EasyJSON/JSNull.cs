using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trilogic.EasyJSON
{
    public class JSNull : JSItem
    {
        #region Constructors and Destructors
        internal JSNull(JSItem parent) : base(parent) { }
        #endregion

        #region Overrides
        public override dynamic Value { get => null; set => throw new JSException("Attempt to set null value"); }
        public override string ToString() => "null";
        public override void Write(StreamWriter writer) => writer.Write("null");
        public override bool IsNull => true;
        #endregion
    }
}
