using System.Data;
using System.Data.SqlClient;
using System.Text;
using Trilogic.EasyJSON;
using Trilogic.EasyJSON.Data;

namespace Trilogic.EasyJSON.App
{
    class Program
    {
        static string ExportName = "generated.out.json";
        static string ImportName = $"{ExportName}";
        static string VerifyName = "generated.inp.json";

        static void Main(string[] args)
        {
            TestSerial(JSOutputFormat.OutputCompact);

            Console.WriteLine("Done...");
            Console.ReadKey();
        }

        public static string GetLocalPath(string FileName)
        {
            string thisPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fullPath = Path.Combine(thisPath, $"..\\..\\..\\{FileName}");
            fullPath = Path.GetDirectoryName(thisPath);
            fullPath = Path.Combine(new DirectoryInfo(fullPath).Parent.Parent.FullName, FileName);
            return fullPath;
        }

        public static void TestSerial(JSOutputFormat format = JSOutputFormat.OutputKNR)
        {
            var jsonOut = TestHelp.BuildCompleteObject();

            ExportViaToString(jsonOut, ExportName, format);

            string valueOut = jsonOut.ToString(format);

            var jsonInp = JSItem.Parse(valueOut);

            ExportViaToString(jsonInp, ImportName, format);

            string valueInp = jsonInp.ToString(format);

            var matched = string.Compare(valueOut, valueInp, false) == 0;

            Console.WriteLine($"Strings Match: {matched}"
                );
        }

        public static JSItem ImportFile(string fileName)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            string json = File.ReadAllText(GetLocalPath(fileName));
            stopwatch.Start();
            JSItem item = JSItem.Parse(json);
            stopwatch.Stop();
            Console.WriteLine($"Elapsed: {stopwatch.Elapsed}");
            return item;
        }

        public static void ExportViaFileWriter(JSItem json, string fileName, JSOutputFormat format = JSOutputFormat.OutputKNR)
        {
            var writer = new JSFormatter(format);
            var stream = File.CreateText(GetLocalPath(fileName));
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            writer.Write(stream, json);
            stream.Close();
            stopwatch.Stop();
            Console.WriteLine($"Elapsed: {stopwatch.Elapsed}");
        }

        public static void ExportViaToString(JSItem json, string fileName, JSOutputFormat format = JSOutputFormat.OutputKNR)
        {
            var stream = File.CreateText(GetLocalPath(fileName));

            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            var output = json.ToString(format);
            stopwatch.Stop();

            stream.WriteLine(output);
            stream.Close();

            Console.WriteLine($"Elapsed: {stopwatch.Elapsed}");
        }


        public static JSItem TestData()
        {
            JSItem itemT = JSItem.CreateObject();
            JSItem itemL = JSItem.CreateObject();

            string sql = @"SELECT * FROM [Batch]";

            SqlConnectionStringBuilder b = new SqlConnectionStringBuilder();
            b.UserID = "???";
            b.Password = "???";
            b.DataSource = "???";
            b.InitialCatalog = "???";

            string connect = b.ToString();

            using (System.Data.SqlClient.SqlConnection con = new SqlConnection(connect))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        DataTable tbl = new DataTable();
                        tbl.Load(rdr);

                        JSDataTableHelp.AddObjects(itemL, tbl, "data");
                        JSDataTableHelp.AddTable(itemT, tbl, "data");

                        // JSReaderHelp.AddTable(item, rdr, "data");
                    }
                }
            }

            string thisPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string thisFile = "json-2.txt";
            string fullPath = Path.Combine(thisPath, $"..\\..\\..\\{thisFile}");

            JSFormatter writer = new JSFormatter(JSOutputFormat.OutputAllman);

            writer.Write(File.CreateText(Path.Combine(thisPath, $"..\\..\\..\\jsonT.txt")), itemT);
            writer.Write(File.CreateText(Path.Combine(thisPath, $"..\\..\\..\\jsonL.txt")), itemL);

            //File.WriteAllText(Path.Combine(thisPath, $"..\\..\\..\\jsonT.txt"), writer.WriteJSON(itemT));
            //File.WriteAllText(Path.Combine(thisPath, $"..\\..\\..\\jsonL.txt"), writer.WriteJSON(itemL));

            return itemT;
        }

        public static string TestWriter(JSItem json, JSOutputFormat format)
        {
            var writer = new JSFormatter(format, ExpandEmpty: false, BlankBeforeColon: false);
            return writer.Format(json);
        }

        public static JSItem TestParse(string json)
        {
            return JSItem.Parse(json);
        }

        public static JSItem TestParse(StringBuilder json)
        {
            return JSItem.Parse(json);
        }
        public static JSItem TestParse(StreamReader json)
        {
            return JSItem.Parse(json);
        }

        public static JSItem TestParse(Stream json)
        {
            return JSItem.Parse(json);
        }
    }
}
