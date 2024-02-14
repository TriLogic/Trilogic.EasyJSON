using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trilogic.EasyJSON.Data
{
    public delegate JSItem JSGetColumnFunc(JSItem host, System.Data.DataRow row, int index, string key);

    public class JSColumnProfile
    {
        public int ColIndex;
        public string ColName;
        public Type ColType;
        public JSGetColumnFunc ColFunc;
    }

    public static class JSDataTableHelp
    {
        #region Private data
        private static Dictionary<Type, JSGetColumnFunc> _fldMap;
        private static JSGetColumnFunc GetColBoolean;
        private static JSGetColumnFunc GetColNumber;
        private static JSGetColumnFunc GetColString;
        #endregion

        #region Static Constructor
        static JSDataTableHelp()
        {
            _fldMap = new Dictionary<Type, JSGetColumnFunc>();

            GetColBoolean = delegate (JSItem host, System.Data.DataRow row, int index, string key)
            {
                return host.AddBoolean(Convert.ToBoolean(row.ItemArray[index]), key);
            };
            GetColNumber = delegate (JSItem host, System.Data.DataRow row, int index, string key)
            {
                return host.AddNumber(Convert.ToDouble(row.ItemArray[index]), key);
            };
            GetColString = delegate (JSItem host, System.Data.DataRow row, int index, string key)
            {
                return host.AddString(Convert.ToString(row.ItemArray[index]), key);
            };

            _fldMap = new Dictionary<Type, JSGetColumnFunc>();
            _fldMap.Add(typeof(string), GetColString);
            _fldMap.Add(typeof(Guid), GetColString);
            _fldMap.Add(typeof(long), GetColNumber);
            _fldMap.Add(typeof(bool), GetColBoolean);
            _fldMap.Add(typeof(DateTime), GetColString);
            _fldMap.Add(typeof(decimal), GetColNumber);
            _fldMap.Add(typeof(double), GetColNumber);
            _fldMap.Add(typeof(int), GetColNumber);
            _fldMap.Add(typeof(float), GetColNumber);
            _fldMap.Add(typeof(short), GetColNumber);
            _fldMap.Add(typeof(byte), GetColNumber);
            _fldMap.Add(typeof(object), GetColString);
        }
        #endregion

        #region Setting / Gettings Mapped DataRow Column Functions
        public static JSGetColumnFunc GetColFunc(Type type)
        {
            if (_fldMap.ContainsKey(type))
                return _fldMap[type];
            return null;
        }
        public static void SetColFunc(Type type, JSGetColumnFunc func)
        {
            if (_fldMap.ContainsKey(type))
                _fldMap.Remove(type);
            _fldMap.Add(type, func);
        }
        #endregion

        #region Profile Record or Partial Records by Names or Ordinal (List or Array}
        public static List<JSColumnProfile> GetProfile(System.Data.DataTable tbl)
        {
            List<JSColumnProfile> result = new List<JSColumnProfile>();
            for (int index = 0; index < tbl.Columns.Count; index++)
                result.Add(ProfileByOrdinal(tbl, index));
            return result;
        }

        public static List<JSColumnProfile> GetProfile(System.Data.DataTable tbl, List<string> fields)
        {
            List<JSColumnProfile> result = new List<JSColumnProfile>();
            foreach (string key in fields)
                result.Add(ProfileByName(tbl, key));
            return result;
        }

        public static List<JSColumnProfile> GetProfile(System.Data.DataTable tbl, List<int> fields)
        {
            List<JSColumnProfile> result = new List<JSColumnProfile>();
            foreach (int ordinal in fields)
                result.Add(ProfileByOrdinal(tbl, ordinal));
            return result;
        }

        public static List<JSColumnProfile> GetProfile(System.Data.DataTable tbl, string[] names)
        {
            List<JSColumnProfile> result = new List<JSColumnProfile>();
            foreach (string name in names)
                result.Add(ProfileByName(tbl, name));
            return result;
        }

        public static List<JSColumnProfile> GetProfile(System.Data.DataTable tbl, int[] ordinals)
        {
            List<JSColumnProfile> result = new List<JSColumnProfile>();
            foreach (int ordinal in ordinals)
                result.Add(ProfileByOrdinal(tbl, ordinal));
            return result;
        }
        #endregion

        #region Profile Individual Field by Name or Ordinal
        public static JSColumnProfile ProfileByName(System.Data.DataTable tbl, string colName)
        {
            // retrieve the ordinal
            int ordinal = tbl.Columns[colName].Ordinal;

            JSColumnProfile p = new JSColumnProfile
            {
                ColIndex = ordinal,
                ColName = colName,
                ColType = tbl.Columns[ordinal].DataType,
                ColFunc = null
            };
            p.ColFunc = GetColFunc(p.ColType);

            return p;
        }

        public static JSColumnProfile ProfileByOrdinal(System.Data.DataTable tbl, int colOrdinal)
        {
            // retrieve the column name 
            string name = tbl.Columns[colOrdinal].ColumnName;

            JSColumnProfile p = new JSColumnProfile
            {
                ColIndex = colOrdinal,
                ColName = name,
                ColType = tbl.Columns[colOrdinal].DataType,
                ColFunc = null
            };
            p.ColFunc = GetColFunc(p.ColType);

            return p;
        }
        #endregion

        #region Add All Rows or All Partial Rows as a Table Structure
        public static JSItem AddTable(JSItem host, System.Data.DataTable table, string key = null)
        {
            return AddTable(host, table, JSDataTableHelp.GetProfile(table), key);
        }
        public static JSItem AddTable(JSItem host, System.Data.DataTable table, List<string> fldNames, string key = null)
        {
            return AddTable(host, table, JSDataTableHelp.GetProfile(table, fldNames), key);
        }
        public static JSItem AddTable(JSItem host, System.Data.DataTable table, string[] fldNames, string key = null)
        {
            return AddTable(host, table, JSDataTableHelp.GetProfile(table, fldNames), key);
        }
        public static JSItem AddTable(JSItem host, System.Data.DataTable table, List<int> fldOrdinals, string key = null)
        {
            return AddTable(host, table, JSDataTableHelp.GetProfile(table, fldOrdinals), key);
        }
        public static JSItem AddTable(JSItem host, System.Data.DataTable table, int[] fldOrdinals, string key = null)
        {
            return AddTable(host, table, JSDataTableHelp.GetProfile(table, fldOrdinals), key);
        }
        public static JSItem AddTable(JSItem host, System.Data.DataTable table, List<JSColumnProfile> profiles, string key = null)
        {
            JSItem tbl = host.AddObject(key);
            JSItem cols = tbl.AddArray("cols");

            // add the column names
            foreach (JSColumnProfile profile in profiles)
                cols.AddString(profile.ColName);

            // add the row data
            JSItem rows = tbl.AddArray("rows");

            foreach(System.Data.DataRow tblRow in table.Rows)
            {
                JSItem row = rows.AddArray();
                foreach (JSColumnProfile profile in profiles)
                {
                    JSDataTableHelp.AddColumn(row, tblRow, profile);
                }
            }

            return tbl;
        }
        #endregion

        #region Add All Rows or All Partial Rows of a DataTable as an Array of JSObjects
        public static JSItem AddObjects(JSItem host, System.Data.DataTable tbl, string key = null)
        {
            return AddObjects(host, tbl, JSDataTableHelp.GetProfile(tbl), key);
        }
        public static JSItem AddObjects(JSItem host, System.Data.DataTable tbl, List<string> colNames, string key = null)
        {
            return AddObjects(host, tbl, JSDataTableHelp.GetProfile(tbl, colNames), key);
        }
        public static JSItem AddObjects(JSItem host, System.Data.DataTable tbl, List<int> colOrdinals, string key = null)
        {
            return AddObjects(host, tbl, JSDataTableHelp.GetProfile(tbl, colOrdinals), key);
        }
        public static JSItem AddObjects(JSItem host, System.Data.DataTable tbl, string[] colNames, string key = null)
        {
            return AddObjects(host, tbl, JSDataTableHelp.GetProfile(tbl, colNames), key);
        }
        public static JSItem AddObjects(JSItem host, System.Data.DataTable tbl, int[] colOrdinals, string key = null)
        {
            return AddObjects(host, tbl, JSDataTableHelp.GetProfile(tbl, colOrdinals), key);
        }
        public static JSItem AddObjects(JSItem host, System.Data.DataTable tbl, List<JSColumnProfile> colProfiles, string key = null)
        {
            // if a key is supplied it is a request to add a new array of rows to an existing object
            // otherwise we are being asked to add directly to an existing array
            host = key == null ? host : host.AddArray(key);

            // add all the rows
            foreach (System.Data.DataRow row in tbl.Rows)
                AddObject(host, row, colProfiles, null);

            // return the original object
            return key == null ? host : host.Parent;
        }
        #endregion

        #region Add a Row or a Partial DataRow as an JSObject returning the host object
        public static JSItem AddObject(JSItem host, System.Data.DataRow row, string key = null)
        {
            JSItem item = host.AddObject(key);
            for (int index = 0; index < row.Table.Columns.Count; index++)
                AddColumn(item, row, index, null);
            return host;
        }
        public static JSItem AddObject(JSItem host, System.Data.DataRow row, List<string> colNames, string key = null)
        {
            JSItem item = host.AddObject(key);
            foreach (string name in colNames)
                AddColumn(item, row, name, name);
            return null;
        }
        public static JSItem AddObject(JSItem host, System.Data.DataRow row, string[] colNames, string key = null)
        {
            JSItem item = host.AddObject(key);
            foreach (string name in colNames)
                AddColumn(item, row, name, name);
            return null;
        }
        public static JSItem AddObject(JSItem host, System.Data.DataRow row, List<int> colOrdinals, string key = null)
        {
            JSItem item = host.AddObject(key);
            foreach (int ordinal in colOrdinals)
                AddColumn(item, row, ordinal, row.Table.Columns[ordinal].ColumnName);
            return item;
        }
        public static JSItem AddObject(JSItem host, System.Data.DataRow row, int[] colOrdinals, string key = null)
        {
            JSItem item = host.AddObject(key);
            foreach (int ordinal in colOrdinals)
                AddColumn(item, row, ordinal, row.Table.Columns[ordinal].ColumnName);
            return item;
        }
        public static JSItem AddObject(JSItem host, System.Data.DataRow row, List<JSColumnProfile> colProfiles, string key = null)
        {
            JSItem item = host.AddObject(key);
            foreach (JSColumnProfile profile in colProfiles)
                AddColumn(item, row, profile);
            return item;
        }
        #endregion

        #region Adding a specific column from a DataRow by name, ordinal, or profile
        public static JSItem AddColumn(JSItem host, System.Data.DataRow row, int colOrdinal, string key = null)
        {
            if (row[colOrdinal] == null)
                return host.AddNull(key);

            Type colType = row.Table.Columns[colOrdinal].DataType;
            JSGetColumnFunc func = JSDataTableHelp.GetColFunc(colType);

            if (func == null)
                throw new JSException($"Unsupported field type {colType.Name}");

            return func(host, row, colOrdinal, key);
        }

        public static JSItem AddColumn(JSItem host, System.Data.DataRow row, string colName, string key = null)
        {
            return AddColumn(host, row, row.Table.Columns[colName].Ordinal, key);
        }

        public static JSItem AddColumn(JSItem host, System.Data.DataRow row, JSColumnProfile colProfile, string key = null)
        {
            if (colProfile.ColFunc == null)
                throw new JSException($"Unsupported field type {colProfile.ColType.Name}");

            if (row[colProfile.ColIndex] == null)
                return host.AddNull(key);

            // ignore key if host object is an array
            if (host.IsArray)
                return colProfile.ColFunc(host, row, colProfile.ColIndex, null);

            // allow key to override the column name when adding to an object
            colProfile.ColFunc(host, row, colProfile.ColIndex, key ?? colProfile.ColName);

            return host;
        }
        #endregion
    }
}
