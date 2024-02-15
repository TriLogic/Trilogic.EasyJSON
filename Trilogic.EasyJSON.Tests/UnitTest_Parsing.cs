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
        const string ArrayOfAllTypes = $"[ null, true, 123.45, \"Bob\", {EmptyArray}, {SimpleArray}, {EmptyObject}, {SimpleObject} ]";

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
            Assert.AreEqual("Bob", json["name"].GetString());
            Assert.True(json.Exists("age"));
            Assert.AreEqual(63, json["age"].GetInteger());
        }

        [Test(Description = "Test parsing of a JSON array.")]
        public void Test_Parse_SimpleArray()
        {
            JSItem json = JSItem.Parse(SimpleArray);

            Assert.IsNotNull(json);
            Assert.True(json.IsArray);
            Assert.True(json.Count == 3);
            var name = json[0].GetString();
            Assert.AreEqual("Bob", json[0].GetString());
            Assert.AreEqual("Jon", json[1].GetString());
            Assert.AreEqual("Ted", json[2].GetString());
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
            Assert.AreEqual("Bob", item1[0].GetString());
            Assert.AreEqual("Jon", item1[1].GetString());
            Assert.AreEqual("Ted", item1[2].GetString());
            
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
            Assert.AreEqual("Bob", item1["name"].GetString());

            Assert.True(item1.Exists("age"));
            Assert.True(item1["age"].IsNumber);
            Assert.AreEqual(63, item1["age"].GetInteger());

            var item2 = json["empty"];
            Assert.True(item2.IsObject);
            Assert.True(item2.Count == 0);
        }

        [Test(Description = "Test parsing of a all JSON types")]
        public void Test_Parse_ArrayOfAllTypes()
        {
            JSItem json = JSItem.Parse(ArrayOfAllTypes);

            Assert.IsNotNull(json);
            Assert.True(json.IsArray);
            Assert.True(json.Count == 8);

            Assert.True(json[0].IsNull);
            Assert.True(json[1].IsBoolean);
            Assert.True(json[2].IsNumber);
            Assert.True(json[3].IsString);
            Assert.True(json[4].IsArray);
            Assert.True(json[5].IsArray);
            Assert.True(json[6].IsObject);
            Assert.True(json[7].IsObject);
        }
    }
}
