using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trilogic.EasyJSON
{
    public class JSArray : JSItem
    {
        #region Class Members
        internal List<JSItem> _items = new List<JSItem>();
        #endregion

        #region Constructors and Destructors
        public JSArray() : base(null)
        {
        }
        internal JSArray(JSItem parent) : base(parent)
        {
        }
        #endregion

        #region Override Functions
        public override List<JSItem> ToList() => new List<JSItem>(_items);
        public override List<JSItem> GetList() => _items;
        public override dynamic Value { get => _items; set => throw new JSException("Not allowed on container"); }
        public override int Count => _items.Count;
        public override bool IsArray => true;
        #endregion

        #region ToString Functions
        public override string ToString()
        {
            if (_items.Count == 0)
                return "[]";

            StringBuilder sb = new StringBuilder();
            Write(sb);
            return sb.ToString();
        }
        #endregion

        #region Write Functions
        public override void Write(StreamWriter writer)
        {
            if (_items.Count == 0)
            {
                writer.Write("[]");
                return;
            }

            writer.Write('[');
            bool comma = false;
            foreach (JSItem item in _items)
            {
                if (comma)
                    writer.Write(',');
                item.Write(writer);
                comma = true;
            }
            writer.Write(']');
        }

        public override void Write(StringBuilder builder)
        {
            if (_items.Count == 0)
            {
                builder.Append("[]");
                return;
            }

            builder.Append('[');
            bool comma = false;
            foreach (JSItem item in _items)
            {
                if (comma)
                    builder.Append(',');
                item.Write(builder);
                comma = true;
            }
            builder.Append(']');
        }
        #endregion

        public static new JSArray Parse (string json)
        {
            return new JSReader(json).ParseArray();
        }
    }
}
