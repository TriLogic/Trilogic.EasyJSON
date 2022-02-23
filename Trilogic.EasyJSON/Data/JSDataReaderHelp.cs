using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trilogic.EasyJSON.Data
{
    public delegate JSItem JSGetFieldFunc(JSItem host, System.Data.IDataReader reader, int index, string key);
    public class EJFieldProfile
    {
        public int FldIndex;
        public string FldName;
        public Type FldType;
        public JSGetFieldFunc FldFunc;
    }
    public class JSDataReaderHelp
    {
        #region Private Static Data
        private static Dictionary<Type, JSGetFieldFunc> _fldMap;
        private static JSGetFieldFunc GetFldBoolean;
        private static JSGetFieldFunc GetFldNumber;
        private static JSGetFieldFunc GetFldString;
        #endregion

        #region Static Constructor
        static JSDataReaderHelp()
        {
            GetFldBoolean = delegate (JSItem host, System.Data.IDataReader reader, int index, string key)
            {
                return host.AddBoolean(Convert.ToBoolean(reader.GetValue(index)), key);
            };
            GetFldNumber = delegate (JSItem host, System.Data.IDataReader reader, int index, string key)
            {
                return host.AddNumber(Convert.ToDouble(reader.GetValue(index)), key);
            };
            GetFldString = delegate (JSItem host, System.Data.IDataReader reader, int index, string key)
            {
                return host.AddString(Convert.ToString(reader.GetValue(index)), key);
            };

            _fldMap = new Dictionary<Type, JSGetFieldFunc>();
            _fldMap.Add(typeof(string), GetFldString);
            _fldMap.Add(typeof(Guid), GetFldString);
            _fldMap.Add(typeof(long), GetFldNumber);
            _fldMap.Add(typeof(bool), GetFldBoolean);
            _fldMap.Add(typeof(DateTime), GetFldString);
            _fldMap.Add(typeof(decimal), GetFldNumber);
            _fldMap.Add(typeof(double), GetFldNumber);
            _fldMap.Add(typeof(int), GetFldNumber);
            _fldMap.Add(typeof(float), GetFldNumber);
            _fldMap.Add(typeof(short), GetFldNumber);
            _fldMap.Add(typeof(byte), GetFldNumber);
            _fldMap.Add(typeof(object), GetFldString);
        }
        #endregion

        #region Setting / Gettings Mapped DataReader Field Functions
        public static JSGetFieldFunc GetFldFunc(Type type)
        {
            if (_fldMap.ContainsKey(type))
                return _fldMap[type];
            return null;
        }
        public static void SetFldFunc(Type type, JSGetFieldFunc func)
        {
            if (_fldMap.ContainsKey(type))
                _fldMap.Remove(type);
            _fldMap.Add(type, func);
        }
        #endregion

        #region Profile Record or Partial Records by Names or Ordinal (List or Array}
        public static List<EJFieldProfile> GetProfile(System.Data.IDataReader reader)
        {
            List<EJFieldProfile> result = new List<EJFieldProfile>();
            for (int index = 0; index < reader.FieldCount; index++)
                result.Add(ProfileByOrdinal(reader, index));
            return result;
        }

        public static List<EJFieldProfile> GetProfile(System.Data.IDataReader reader, List<string> fields)
        {
            List<EJFieldProfile> result = new List<EJFieldProfile>();
            foreach (string key in fields)
                result.Add(ProfileByName(reader, key));
            return result;
        }

        public static List<EJFieldProfile> GetProfile(System.Data.IDataReader reader, List<int> fields)
        {
            List<EJFieldProfile> result = new List<EJFieldProfile>();
            foreach (int ordinal in fields)
                result.Add(ProfileByOrdinal(reader, ordinal));
            return result;
        }

        public static List<EJFieldProfile> GetProfile(System.Data.IDataReader reader, string[] fields)
        {
            List<EJFieldProfile> result = new List<EJFieldProfile>();
            foreach (string key in fields)
                result.Add(ProfileByName(reader, key));
            return result;
        }

        public static List<EJFieldProfile> GetProfile(System.Data.IDataReader reader, int[] fields)
        {
            List<EJFieldProfile> result = new List<EJFieldProfile>();
            foreach (int ordinal in fields)
                result.Add(ProfileByOrdinal(reader, ordinal));
            return result;
        }
        #endregion

        #region Profile Individual Field by Name or Ordinal
        public static EJFieldProfile ProfileByName(System.Data.IDataReader reader, string fieldName)
        {
            // retrieve the ordinal
            int index = reader.GetOrdinal(fieldName);

            EJFieldProfile p = new EJFieldProfile
            {
                FldIndex = index,
                FldName = fieldName,
                FldType = reader.GetFieldType(index),
                FldFunc = null
            };
            p.FldFunc = GetFldFunc(p.FldType);

            return p;
        }

        public static EJFieldProfile ProfileByOrdinal(System.Data.IDataReader reader, int fieldOrdinal)
        {
            // retrieve the ordinal
            string name = reader.GetName(fieldOrdinal);

            EJFieldProfile p = new EJFieldProfile
            {
                FldIndex = fieldOrdinal,
                FldName = name,
                FldType = reader.GetFieldType(fieldOrdinal),
                FldFunc = null
            };
            p.FldFunc = GetFldFunc(p.FldType);

            return p;
        }
        #endregion

        #region Add All Rows or All Partial Rows as a Table Structure
        public static JSItem AddTable(JSItem host, System.Data.IDataReader reader, string key = null)
        {
            return AddTable(host, reader, JSDataReaderHelp.GetProfile(reader), key);
        }
        public static JSItem AddTable(JSItem host, System.Data.IDataReader reader, List<string> fldNames, string key = null)
        {
            return AddTable(host, reader, JSDataReaderHelp.GetProfile(reader, fldNames), key);
        }
        public static JSItem AddTable(JSItem host, System.Data.IDataReader reader, string[] fldNames, string key = null)
        {
            return AddTable(host, reader, JSDataReaderHelp.GetProfile(reader, fldNames), key);
        }
        public static JSItem AddTable(JSItem host, System.Data.IDataReader reader, List<int> fldOrdinals, string key = null)
        {
            return AddTable(host, reader, JSDataReaderHelp.GetProfile(reader, fldOrdinals), key);
        }
        public static JSItem AddTable(JSItem host, System.Data.IDataReader reader, int[] fldOrdinals, string key = null)
        {
            return AddTable(host, reader, JSDataReaderHelp.GetProfile(reader, fldOrdinals), key);
        }
        public static JSItem AddTable(JSItem host, System.Data.IDataReader reader, List<EJFieldProfile> profiles, string key = null)
        {
            JSItem tbl = host.AddObject(key);
            JSItem cols = tbl.AddArray("cols");

            // add the column names
            foreach (EJFieldProfile profile in profiles)
                cols.AddString(profile.FldName);

            // add the row data
            JSItem rows = tbl.AddArray("rows");

            while (reader.Read())
            {
                JSItem row = rows.AddArray();
                foreach (EJFieldProfile profile in profiles)
                {
                    JSDataReaderHelp.AddField(row, reader, profile);
                }
            }

            return tbl;
        }
        #endregion

        #region Add All Rows or All Partial Rows as an Array of JSObjects
        public static JSItem AddObjects(JSItem host, System.Data.IDataReader reader, string key = null)
        {
            return AddObjects(host, reader, JSDataReaderHelp.GetProfile(reader), key);
        }
        public static JSItem AddObjects(JSItem host, System.Data.IDataReader reader, List<string> fldNames, string key = null)
        {
            return AddObjects(host, reader, JSDataReaderHelp.GetProfile(reader, fldNames), key);
        }
        public static JSItem AddObjects(JSItem host, System.Data.IDataReader reader, List<int> fieldOrdinals, string key = null)
        {
            return AddObjects(host, reader, JSDataReaderHelp.GetProfile(reader, fieldOrdinals), key);
        }
        public static JSItem AddObjects(JSItem host, System.Data.IDataReader reader, string[] fieldNames, string key = null)
        {
            return AddObjects(host, reader, JSDataReaderHelp.GetProfile(reader, fieldNames), key);
        }
        public static JSItem AddObjects(JSItem host, System.Data.IDataReader reader, int[] fieldOrdinals, string key = null)
        {
            return AddObjects(host, reader, JSDataReaderHelp.GetProfile(reader, fieldOrdinals), key);
        }
        public static JSItem AddObjects(JSItem host, System.Data.IDataReader reader, List<EJFieldProfile> profiles, string key = null)
        {
            // if a key is supplied it is a request to add a new array of rows to an existing object
            // otherwise we are being asked to add directly to an existing array
            host = key == null ? host : host.AddArray(key);

            // add all the rows
            while (reader.Read())
                AddObject(host, reader, profiles, null);

            // return the original object
            return key == null ? host : host.Parent;
        }
        #endregion

        #region Add Row or Partial Row as an JSObject to a JSItem, Return host object
        public static JSItem AddObject(JSItem host, System.Data.IDataReader reader, string key = null)
        {
            JSItem item = host.AddObject(key);
            for (int index = 0; index < reader.FieldCount; index++)
                AddField(item, reader, index, null);
            return host;
        }
        public static JSItem AddObject(JSItem host, System.Data.IDataReader reader, List<string> fldNames, string key = null)
        {
            JSItem item = host.AddObject(key);
            foreach (string name in fldNames)
                AddField(item, reader, name, name);
            return null;
        }
        public static JSItem AddObject(JSItem host, System.Data.IDataReader reader, string[] fldNames, string key = null)
        {
            JSItem item = host.AddObject(key);
            foreach (string name in fldNames)
                AddField(item, reader, name, name);
            return null;
        }
        public static JSItem AddObject(JSItem host, System.Data.IDataReader reader, List<int> fldOrdinals, string key = null)
        {
            JSItem item = host.AddObject(key);
            foreach (int ordinal in fldOrdinals)
                AddField(item, reader, ordinal, reader.GetName(ordinal));
            return item;
        }
        public static JSItem AddObject(JSItem host, System.Data.IDataReader reader, int[] fldOrdinals, string key = null)
        {
            JSItem item = host.AddObject(key);
            foreach (int ordinal in fldOrdinals)
                AddField(item, reader, ordinal, reader.GetName(ordinal));
            return item;
        }
        public static JSItem AddObject(JSItem host, System.Data.IDataReader reader, List<EJFieldProfile> fldProfiles, string key = null)
        {
            JSItem item = host.AddObject(key);
            foreach (EJFieldProfile profile in fldProfiles)
                AddField(item, reader, profile);
            return item;
        }
        #endregion

        #region Adding a specific field from the row using the integer index
        public static JSItem AddField(JSItem host, System.Data.IDataReader reader, int fldOrdinal, string key = null)
        {
            if (reader.IsDBNull(fldOrdinal))
                return host.AddNull(key);

            Type fieldType = reader.GetFieldType(fldOrdinal);
            JSGetFieldFunc func = JSDataReaderHelp.GetFldFunc(fieldType);

            if (func == null)
                throw new JSException($"Unsupported field type {fieldType.Name}");

            return func(host, reader, fldOrdinal, key);
        }

        public static JSItem AddField(JSItem host, System.Data.IDataReader reader, EJFieldProfile fldProfile, string key = null)
        {
            if (fldProfile.FldFunc == null)
                throw new JSException($"Unsupported field type {fldProfile.FldType.Name}");

            if (reader.IsDBNull(fldProfile.FldIndex))
                return host.AddNull(key);

            if (host.IsArray)
                return fldProfile.FldFunc(host, reader, fldProfile.FldIndex, null);

            fldProfile.FldFunc(host, reader, fldProfile.FldIndex, key == null ? fldProfile.FldName : key);

            return host;
        }

        public static JSItem AddField(JSItem host, System.Data.IDataReader reader, string fldName, string key = null)
        {
            return AddField(host, reader, reader.GetOrdinal(fldName), key);
        }
        #endregion
    }
}
