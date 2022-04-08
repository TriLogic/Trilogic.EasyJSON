# Trilogic.EasyJSON
A lightweight and simple library for reading, writing and formatting JSON.

Reading JSON:

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

Writing JSON with Formatting:

    public static void ExportFileWriter(JSItem json, string filePath, JSOutputFormat format = JSOutputFormat.OutputKNR)
    {
        var writer = new JSFormatter(format);
        writer.SetFormatKNR();
        // writer.SetFormatWhitesmith();
        // writer.SetFormatAllman();
        // writer.SetFormatLinear();
        // writer.SetFormatCompact();
        var stream = File.CreateText(filePath);
        writer.Write(stream, json);
        stream.Close();
    }

Building JSON via code:

    public static JSItem TestBuilder()
    {
        JSItem json = JSItem.CreateObject();

        //goto EMPTIES;

        // Strings:
        json.AddObject("Strings")
            .AddString(@"\u0300\u0301\u0302\u0303\u0304\u0305\u0306", "Unicode")
            .AddString("", "empty")
            .AddString("\r\n", "vbCr")
            .AddString("\n", "vbLf")
            .AddString("\t", "vbTab")
            .AddString("\\", "backslash")
            .AddString("/", "slash")
            .AddString("\"", "dquote")
            .AddString("The quick silver fox jumped over the lazy brown dog.", "allChars");

        // Booleans:
        json.AddObject("Booleans")
            .AddBoolean(true, "true")
            .AddBoolean(false, "false");

        // Null:
        json.AddObject("Null")
            .AddNull("Null");

        // Numbers:
        json.AddObject("Numbers")
            .AddNumber(0, "zero")
            .AddNumber(1, "one")
            .AddNumber(-3, "negative")
            .AddNumber(0.005, "decimal")
            .AddNumber(-5.79E-32, "scientific");

        json.AddArray("An_Array")
            .AddNumber(100)
            .AddNumber(200)
            .AddNumber(300)
            .AddNumber(400)
            .AddNumber(500);

        json.AddObject("An_Object")
            .AddNumber(100, "X_100")
            .AddNumber(200, "X_200")
            .AddNumber(300, "X_300")
            .AddNumber(400, "X_400")
            .AddNumber(500, "X_500");

    EMPTIES:

        // Empties:
        json.AddObject("Empty_Containers")
            .AddArray("Array_Of_Empty_Objects")
                .AddObject().Parent
                .AddObject().Parent
                .Parent
            .AddArray("Array_Of_Empty_Arrays")
                .AddArray().Parent
                .AddArray().Parent
                .AddArray().Parent
                .Parent
            .AddObject("Object_Of_Empty_Arrays")
                .AddArray("Array1").Parent
                .AddArray("Array2").Parent
                .AddArray("Array3").Parent
                .Parent
            .AddObject("Object_Of_Empty_Objects")
                .AddObject("Object1").Parent
                .AddObject("Object2").Parent
                .AddObject("Object3");

    ARRAYOF:

        json.AddArray("Array_Of_Objects")
            .AddObject()
            .AddString("one", "one")
            .AddString("two", "two")
            .Parent
            .AddObject()
            .AddString("one", "one")
            .AddString("two", "two");

        //goto ENDOF;

    OBJECTOF:

        json.AddObject("Object_Of_Arrays")
            .AddArray("one")
            .AddString("one")
            .AddString("two")
            .Parent
            .AddArray("two")
            .AddString("one")
            .AddString("two");

    ENDOF:

        return json;
    }

Extended Support for DataSets and DataTables with Record Profiling:

    public static JSItem TestData(string pathT, string pathL)
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
                }
            }
        }

        JSFormatter writer = new JSFormatter(JSOutputFormat.OutputAllman);

        writer.Write(pathT, itemT);
        writer.Write(pathL, itemL);

        return itemT;
    }

NOTE: This library does not currently support POCO serialization.
