using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trilogic.EasyJSON.Tests;

namespace Trilogic.EasyJSON.Test
{
    public class UnitTest_Parsing
    {
        const string EmptyArray = "[]";
        const string EmptyObject = "{}";
        const string SimpleObject = "{\"name\":\"Bob\",\"age\":63 }";
        const string ComplexObject = $"{{\"object\": {SimpleObject},\"empty\": {EmptyObject}}}";
        const string SimpleArray = "[\"Bob\",\"Jon\", \"Ted\"]";
        const string ComplexArray = $"[{SimpleArray},{EmptyArray}]";

        [SetUp]
        public void Setup()
        {
        }

        [Test(Description = "Test parsing of a JSON object.")]
        public void Test_Parse_SimpleObject()
        {
            JSItem json = JSItem.Parse(SimpleObject);

            Assert.IsNotNull(json);
            Assert.True(json.IsObject);
            Assert.True(json.Count == 2);
            Assert.True(json.Exists("name"));
            var name = json["name"].GetString();
            Assert.AreEqual(json["name"].GetString(), "Bob");
            Assert.True(json.Exists("age"));
            Assert.AreEqual(json["age"].ToInteger(), 63);
        }

        [Test(Description = "Test parsing of a JSON array.")]
        public void Test_Parse_SimpleArray()
        {
            JSItem json = JSItem.Parse(SimpleArray);

            Assert.IsNotNull(json);
            Assert.True(json.IsArray);
            Assert.True(json.Count == 3);
            var name = json[0].GetString();
            Assert.AreEqual(json[0].GetString(), "Bob");
            Assert.AreEqual(json[1].GetString(), "Jon");
            Assert.AreEqual(json[2].GetString(), "Ted");
        }

        [Test(Description = "Test parsing of a complex JSON array")]
        public void Test_Parse_ComplexArray()
        {
            JSItem json = JSItem.Parse(ComplexArray);

            Assert.IsNotNull(json);
            Assert.True(json.IsArray);
            Assert.True(json.Count == 2);

            var item1 = json[0];
            Assert.True(item1.IsArray);
            Assert.True(item1.Count == 3);
            Assert.AreEqual(item1[0].GetString(), "Bob");
            Assert.AreEqual(item1[1].GetString(), "Jon");
            Assert.AreEqual(item1[2].GetString(), "Ted");
            
            var item2 = json[1];
            Assert.True(item2.IsArray);
            Assert.True(item2.Count == 0);
        }

        [Test(Description = "Test parsing of a complex JSON object")]
        public void Test_Parse_ComplexObject()
        {
            JSItem json = JSItem.Parse(ComplexObject);

            Assert.IsNotNull(json);
            Assert.True(json.IsObject);
            Assert.True(json.Count == 2);

            Assert.True(json.Exists("object"));
            
            var item1 = json["object"];
            Assert.True(item1.IsObject);
            Assert.True(item1.Count == 2);

            Assert.True(item1.Exists("name"));
            Assert.True(item1["name"].IsString);
            Assert.AreEqual(item1["name"].GetString(), "Bob");

            Assert.True(item1.Exists("age"));
            Assert.True(item1["age"].IsNumber);
            Assert.AreEqual(item1["age"].ToInteger(), 63);

            var item2 = json["empty"];
            Assert.True(item2.IsObject);
            Assert.True(item2.Count == 0);
        }

    }
}
