using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trilogic.EasyJSON
{
    public class JSObject : JSItem
    {
        #region Class Members
        Dictionary<string, JSItem> _items = new Dictionary<string, JSItem>();
        #endregion

        #region Constructors and Destructors
        public JSObject() : base(null)
        {
        }
        internal JSObject(JSItem parent) : base(parent)
        {
        }
        #endregion

        #region Override Functions
        public override Dictionary<string, JSItem> GetDictionary() => _items;
        public override dynamic Value { get => _items; set => throw new JSException("Disallowed for container."); }
        public override int Count => _items.Count;
        public override bool IsObject => true;
        #endregion

        #region ToString Functions
        public override string ToString()
        {
            if (_items.Count == 0)
                return "{}";

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
                writer.Write("{}");
                return;
            }

            writer.Write('{');
            bool comma = false;
            foreach (string key in _items.Keys)
            {
                if (comma)
                   writer.Write(',');
                writer.Write('"');
                writer.Write(key);
                writer.Write('"');
                writer.Write(':');
                _items[key].Write(writer);
                comma = true;
            }
            writer.Write('}');
        }
        public override void Write(StringBuilder builder)
        {
            if (_items.Count == 0)
            {
                builder.Append("{}");
                return;
            }

            builder.Append('{');
            bool comma = false;
            foreach (string key in _items.Keys)
            {
                if (comma)
                    builder.Append(',');
                builder.Append('"');
                builder.Append(key);
                builder.Append('"');
                builder.Append(':');
                _items[key].Write(builder);
                comma = true;
            }
            builder.Append('}');
        }
        #endregion

        public static JSObject GetObject(Dictionary<String, string> dictionary)
        {
            JSObject result = new JSObject();
            foreach (string key in dictionary.Keys)
                result.AddString(dictionary[key], key);
            return result;
        }
    }
}
