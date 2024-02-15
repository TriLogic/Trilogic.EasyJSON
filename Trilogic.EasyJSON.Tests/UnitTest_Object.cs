using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trilogic.EasyJSON.Tests;

namespace Trilogic.EasyJSON.Test
{
    public class UnitTest_Object
    {
        public delegate void AddAnItem(JSItem parent, string key);

        [SetUp]
        public void Setup()
        {
        }

        [Test(Description = "Insure items created from JSItem.CreateObject() are objects.")]
        public void Test_Object_IsObject()
        {
            JSItem item = JSItem.CreateObject();

            Assert.NotNull(item);
            Assert.True(item.IsContainer);
            Assert.True(item.IsObject);
            Assert.False(item.IsArray);
            Assert.False(item.IsNull);
            Assert.False(item.IsBoolean);
            Assert.False(item.IsNumber);
            Assert.False(item.IsString);
        }

        [Test(Description = "Insure JSObject supports GetDictionary().")]
        public void Test_Object_GetDictionary()
        {
            JSItem item = JSItem.CreateObject();

            Assert.NotNull(item);
            Assert.True(item.IsContainer);
            Assert.True(item.IsObject);

            Exception exCaught = null;
            try
            {
                Dictionary<string,JSItem> dictionary = item.GetDictionary();
            }
            catch (Exception ex)
            {
                exCaught = ex;
            }
            Assert.Null(exCaught);
        }


        [Test(Description = "Insure items cannot be added to JSObject without string keys.")]
        public void Test_Array_AddNoKey()
        {
            JSItem item = JSItem.CreateObject();

            Assert.NotNull(item);
            Assert.True(item.IsContainer);
            Assert.True(item.IsObject);

            Exception exCaught = null;
            try
            {
                item.AddNull();
            } catch (Exception ex)
            {
                exCaught = ex;
            }
            Assert.NotNull(exCaught);
        }

        [Test(Description = "Insure items can be added to JSObject and accessed using string keys.")]
        public void Test_Array_AddWithKey()
        {
            JSItem item = JSItem.CreateObject();

            Assert.NotNull(item);
            Assert.True(item.IsContainer);
            Assert.True(item.IsObject);

            var key = "null";

            Exception? exCaught = null;
            try
            {
                item.AddNull(key);
            } catch (Exception ex)
            {
                exCaught = ex;
            }

            Assert.Null(exCaught);
            Assert.True(item.Count == 1);
            Assert.True(item.Exists(key));
            Assert.True(item[key].IsNull);
        }

        [Test(Description = "Insure JSObject items cannot be indexed by integers.")]
        public void Test_Array_IndexByInteger()
        {
            JSItem item = JSItem.CreateObject();

            Assert.NotNull(item);
            Assert.True(item.IsContainer);
            Assert.True(item.IsObject);

            Exception exCaught = null; ;

            try
            {
                item.AddNull();
            }
            catch (Exception ex) 
            {
                exCaught = ex;
            }

            Assert.NotNull(exCaught);
            Assert.True(item.Count == 0);

            exCaught = null;

            string key = "null";
            item.AddNull(key);
            Assert.IsTrue(item.Count == 1);
            Assert.True(item.Exists(key));
            Assert.True(item[key].IsNull);

            try
            {
                var temp = item[0];
            }
            catch (Exception ex)
            {
                exCaught = ex;
            }

            Assert.NotNull(exCaught);
        }

        [Test(Description = "Insure JSObject can be indexed with string keys.")]
        public void Test_Object_IndexerByString()
        {
            JSItem item = JSItem.CreateObject();

            Assert.NotNull(item);
            Assert.True(item.IsContainer);
            Assert.True(item.IsObject);

            string key = "null";
            item.AddNull(key);
            Assert.True(item.Count == 1);

            Exception? exCaught = null;
            try
            {
                var element = item[key];
            }
            catch (Exception ex)
            {
                exCaught = ex;
            }
            Assert.Null(exCaught);
        }

        [Test(Description = "Insures JSObject can be cleared of items.")]
        public void Test_Array_Clear()
        {
            JSItem item = JSItem.CreateObject();

            Assert.NotNull(item);
            Assert.True(item.IsContainer);
            Assert.True(item.IsObject);
            string key = "null";
            item.AddNull(key);
            Assert.True(item.Count == 1);
            Assert.True(item[key].IsNull);
            item.Clear();
            Assert.Zero(item.Count);
        }

        [Test(Description = "Insures items added to JSObject of correct type and value.")]
        public void Test_Object_AddArray()
        {
            JSItem item = JSItem.CreateObject();
            Assert.NotNull(item);
            Assert.True(item.IsContainer);
            Assert.True(item.IsObject);

            item.AddObject("object");
            item.AddArray("array");
            item.AddString("three", "string");
            item.AddNumber(4, "number");
            item.AddBoolean(true, "boolean");
            item.AddNull("null");

            Assert.True(item.Count == 6);
            Assert.True(item["object"].IsObject);
            Assert.True(item["object"].Count == 0);
            Assert.True(item["array"].IsArray);
            Assert.True(item["array"].Count == 0);
            Assert.True(item["string"].IsString);
            Assert.True(item["string"].GetString() == "three");
            Assert.True(item["number"].IsNumber);
            Assert.AreEqual(4, item["number"].GetInteger());
            Assert.True(item["boolean"].IsBoolean);
            Assert.AreEqual(true, item["boolean"].GetBoolean());
            Assert.True(item["null"].IsNull);
        }

        [Test(Description = "Insure empty JSObject ToString() is correct.")]
        public void Test_Object_Empty_ToString()
        {
            JSItem item= JSItem.CreateObject();
            string? output = item.ToString();
            Assert.AreEqual("{}", output);
        }

        [Test(Description = "Test JSObject AsDictionary returns all items.")]
        public void Test_Object_AsDictionary_All()
        {
            JSItem item = TestHelp.BuildObjectOfStrings();

            Exception exCaptured = null;
            try
            {
                var dictionary = item.AsDictionaryOf<JSString>();
                Assert.AreEqual(item.Count, dictionary.Count);
            }
            catch (Exception ex)
            {
                exCaptured = ex;
            }
            Assert.Null(exCaptured);
        }

        [Test(Description = "Test JSObject AsDictionary returns partial list of items.")]
        public void Test_Object_AsDictionary_Partial()
        {
            JSItem item = TestHelp.BuildObjectOfStrings();
            item.AddNumber(12345.67, "NonString");

            Exception exCaptured = null;
            try
            {
                var dictionary = item.AsDictionaryOf<JSNumber>();
                Assert.AreEqual(1, dictionary.Count);
            }
            catch (Exception ex)
            {
                exCaptured = ex;
            }
            Assert.Null(exCaptured);
        }

        [Test(Description = "Test JSObject AsDictionary returns empty list.")]
        public void Test_Object_AsDictionary_None()
        {
            JSItem item = TestHelp.BuildObjectOfStrings();

            Exception exCaptured = null;
            try
            {
                var dictionary = item.AsDictionaryOf<JSNull>();
                Assert.Zero(dictionary.Count);
            }
            catch (Exception ex)
            {
                exCaptured = ex;
            }
            Assert.Null(exCaptured);
        }

    }
}
