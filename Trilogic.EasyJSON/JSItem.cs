using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Trilogic.EasyJSON
{
    public enum EJValueType
    {
        Null = 0,
        Boolean,
        Number,
        String,
        Array,
        Object
    }

    public abstract class JSItem : System.Collections.IEnumerable
    {
        #region Instance Members
        protected JSItem _parent;
        #endregion

        #region Constructors and Destructors
        internal JSItem(JSItem parent)
        {
            if (parent == null)
                return;

            if (parent.IsContainer)
            {
                _parent = parent;
                return;
            }

            throw new JSException("Invalid container");
        }
        #endregion

        #region Properties
        public JSItem Parent
        {
            get { return _parent; }
            internal set { _parent = value; }
        }

        public JSItem Root
        {
            get { return _parent == null ? this : _parent.Root; }
        }

        public abstract dynamic Value { get; set; }

        #region Type Related Properties
        public virtual bool IsContainer { get { return IsObject || IsArray; } }
        public virtual bool IsObject { get { return false; } }
        public virtual bool IsArray { get { return false; } }
        public virtual bool IsString { get { return false; } }
        public virtual bool IsNumber { get { return false; } }
        public virtual bool IsBoolean { get { return false; } }
        public virtual bool IsNull { get { return false; } }
        public virtual EJValueType JSType
        {
            get
            {
                if (IsObject)
                    return EJValueType.Object;
                if (IsArray)
                    return EJValueType.Array;
                if (IsString)
                    return EJValueType.String;
                if (IsNumber)
                    return EJValueType.Number;
                if (IsBoolean)
                    return EJValueType.Boolean;
                return EJValueType.Null;
            }
        }
        #endregion

        #endregion

        #region Container Related
        public virtual int Count
        {
            get
            {
                return IsArray ?
                 GetList().Count : IsObject ?
                     GetDictionary().Count : 0;
            }
        }

        public virtual JSItem this[int index]
        {
            get { return GetList()[index]; }
        }

        public virtual JSItem this[string key]
        {
            get { return GetDictionary()[key]; }
        }

        public virtual bool ContainsKey(string key)
        {
            return GetDictionary().ContainsKey(key);
        }

        public JSItem Add(JSItem item, string key = null)
        {

            if (string.IsNullOrEmpty(key))
            {
                GetList().Add(item);
            }
            else
            {
                GetDictionary().Add(key, item);
            }

            item.Parent = this;
            return item.IsContainer ? item : this;
        }
        public JSItem AddObject(string key = null)
        {
            return Add(new JSObject(this), key);
        }
        public JSItem AddArray(string key = null)
        {
            return Add(new JSArray(this), key);
        }
        public JSItem AddString(string value, string key = null)
        {
            if (!string.IsNullOrEmpty(value))
            {
                //StringBuilder sb = new StringBuilder(value);
                //JSTools.EscapedToString(sb);
                //return Add(new JSString(this, sb.ToString()), key);
                return Add(new JSString(this, value), key);
            }
            return Add(new JSString(this, value), key);
        }
        public JSItem AddNumber(double value, string key = null)
        {
            return Add(new JSNumber(this, value), key);
        }
        public JSItem AddNumber(float value, string key = null)
        {
            return AddNumber((double)value, key);
        }
        public JSItem AddNumber(long value, string key = null)
        {
            return AddNumber((double)value, key);
        }
        public JSItem AddNumber(int value, string key = null)
        {
            return AddNumber((double)value, key);
        }
        public JSItem AddNumber(byte value, string key = null)
        {
            return AddNumber((double)value, key);
        }
        public JSItem AddBoolean(bool value, string key = null)
        {
            return Add(new JSBoolean(this, value), key);
        }
        public JSItem AddTrue(string key = null)
        {
            return AddBoolean(true, key);
        }
        public JSItem AddFalse(string key = null)
        {
            return AddBoolean(false, key);
        }

        internal JSItem AddBoolean(string value, string key = null)
        {
            if (string.IsNullOrEmpty(value))
                return AddBoolean(false, key);
            if (string.Compare("true", value, true) == 0)
                return AddBoolean("true", key);
            if (string.Compare("false", value, true) == 0)
                return AddBoolean(false, key);
            double test;
            if (double.TryParse(value, out test))
                return AddBoolean(test != 0.0, key);
            return AddBoolean(true, key);
        }
        public JSItem AddNull(string key = null)
        {
            return Add(new JSNull(this), key);
        }

        public virtual JSItem Remove(string key)
        {
            JSItem item = GetDictionary()[key];
            GetDictionary().Remove(key);

            item.Parent = null;
            return item;
        }

        public virtual JSItem Remove(int index)
        {
            JSItem item = GetList()[index];
            GetList().RemoveAt(index);
            item.Parent = null;
            return item;
        }

        public virtual void Clear()
        {
            if (IsArray)
            {
                GetList().Clear();
                return;
            }
            else if (IsObject)
            {
                GetDictionary().Clear();
                return;
            }
            throw new Exception("Invalid Container");
        }
        #endregion

        #region Container Access
        public virtual Dictionary<string, JSItem> GetDictionary()
        {
            throw new JSException("Invalid JSObject");
        }

        public virtual Dictionary<string, JSItem> ToDictionary()
        {
            throw new JSException("Invalid JSObject");
        }

        public virtual List<JSItem> GetList()
        {
            throw new JSException("Invalid JSArray");
        }

        public virtual List<JSItem> ToList()
        {
            throw new JSException("Invalid JSArray");
        }

        public virtual bool Exists(string key)
        {
            throw new JSException("Invalid JSObject");
        }
        #endregion

        #region ToString Methods
        public override string ToString()
        {
            throw new Exception("Not Implemented");
        }

        public virtual string ToString(JSOutputFormat format, string IndentText = "\t", bool ExpandEmpty = false)
        {
            using (MemoryStream mstream = new MemoryStream())
            {
                using (StreamWriter stream = new StreamWriter(mstream))
                {
                    JSFormatter formatter = new JSFormatter(format, IndentText, ExpandEmpty);
                    Write(stream, formatter);
                    mstream.Position = 0;
                    using (StreamReader reader = new StreamReader(mstream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }
        #endregion

        #region Numeric Methods
        public virtual byte ToByte()
        {
            throw new Exception("Invalid Numeric");
        }
        public virtual int ToInteger()
        {
            throw new Exception("Invalid Numeric");
        }
        public virtual long ToLong()
        {
            throw new Exception("Invalid Numeric");
        }
        public virtual float ToFloat()
        {
            throw new Exception("Invalid Numeric");
        }
        public virtual double ToDouble()
        {
            throw new Exception("Invalid Numeric");
        }

        public virtual bool HasNumericContent { get => false; }

        #endregion

        #region Write Methods
        public virtual void Write(Stream stream)
        {
            using (var writer = new StreamWriter(stream))
            {
                Write(writer);
            }
        }

        public virtual void Write(StreamWriter writer) => writer.Write(ToString());

        public virtual void Write(StreamWriter writer, JSOutputFormat format, string IndentText = "\t", bool ExpandEmpty = false)
        {
            JSFormatter formatter = new JSFormatter(format, IndentText, ExpandEmpty);
            formatter.Write(writer, this);
        }

        public virtual void Write(StreamWriter writer, JSFormatter formatter)
        {
            formatter.Write(writer, this);
            writer.Flush();
        }

        public virtual void Write(StringBuilder builder) => builder.Append(ToString());
        #endregion

        #region Static Helper Methods

        #region Create Arrays or Objects
        public static JSItem CreateObject()
        {
            return new JSObject();
        }
        public static JSItem CreateArray()
        {
            return new JSArray();
        }
        #endregion

        #region Parse Generic Items
        public static JSItem Parse(string json)
        {
            return JSReader.Parse(json);
        }
        public static JSItem Parse(StringBuilder json)
        {
            return JSReader.Parse(json);
        }
        public static JSItem Parse(System.IO.StreamReader reader)
        {
            return JSReader.Parse(reader);
        }
        public static JSItem Parse(System.IO.Stream stream)
        {
            return JSReader.Parse(stream);
        }
        #endregion

        #region Parse Arrays
        public static JSArray ParseArray(string json)
        {
            return JSReader.ParseArray(json);
        }
        public static JSArray ParseArray(StringBuilder json)
        {
            return JSReader.ParseArray(json);
        }
        public static JSArray ParseArray(System.IO.StreamReader reader)
        {
            return JSReader.ParseArray(reader);
        }
        public static JSArray ParseArray(System.IO.Stream stream)
        {
            return JSReader.ParseArray(stream);
        }
        #endregion

        #region Parse Objects
        public static JSObject ParseObject(string json)
        {
            return JSReader.ParseObject(json);
        }
        public static JSObject ParseObject(StringBuilder json)
        {
            return JSReader.ParseObject(json);
        }
        public static JSObject ParseObject(System.IO.StreamReader reader)
        {
            return JSReader.ParseObject(reader);
        }
        public static JSObject ParseObject(System.IO.Stream stream)
        {
            return JSReader.ParseObject(stream);
        }
        #endregion

        #endregion

        #region Data Retrieval Methods
        public bool GetBoolean()
        {
            if (IsBoolean)
                return (bool)Value;
            if (IsString)
            {
                if (bool.TryParse((string)Value, out bool temp))
                    return temp;
            }
            throw new JSException("Invalid boolean");
        }
        public bool GetBoolean(int index)
        {
            return this[index].GetBoolean();
        }
        public bool GetBoolean(string key)
        {
            return this[key].GetBoolean();
        }

        public double GetNumber()
        {
            if (IsNumber)
                return (double)Value;
            if (IsString)
            {
                if (double.TryParse((string)Value, out double temp))
                    return temp;
            }
            throw new JSException("Invalid numeric");
        }
        public double GetNumber(int index)
        {
            return this[index].GetNumber();
        }
        public double GetNumber(string key)
        {
            return this[key].GetNumber();
        }

        public string GetString()
        {
            if (IsString)
                return (string)Value;
            if (IsNull)
                return null;
            if (IsNumber)
                return ((double)Value).ToString();
            if (IsBoolean)
                return ((bool)Value).ToString();
            return ToString();
        }
        public string GetString(int index)
        {
            return this[index].GetString();
        }
        public string GetString(string key)
        {
            return this[key].GetString();
        }
        #endregion

        #region IEnumerable
        public System.Collections.IEnumerator GetEnumerator()
        {
            if (IsObject)
                return GetDictionary().GetEnumerator();
            if (IsArray)
                return GetList().GetEnumerator();

            throw new JSException("Invalid container");
        }
        #endregion
    }
}
