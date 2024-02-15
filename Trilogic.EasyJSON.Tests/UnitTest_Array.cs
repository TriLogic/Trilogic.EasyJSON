using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trilogic.EasyJSON.Test
{
    public class UnitTest_Array
    {
        public delegate void AddAnItem(JSItem parent, string key);

        [SetUp]
        public void Setup()
        {
        }

        [Test(Description = "Insure items created from JSItem.CreateArray() are arrays.")]
        public void Test_Array_IsArray()
        {
            JSItem item = JSItem.CreateArray();

            Assert.NotNull(item);
            Assert.True(item.IsContainer);
            Assert.True(item.IsArray);
            Assert.False(item.IsNull);
            Assert.False(item.IsBoolean);
            Assert.False(item.IsNumber);
            Assert.False(item.IsString);
            Assert.False(item.IsObject);
        }

        [Test(Description = "Insure JSArray support GetList().")]
        public void Test_Array_GetList()
        {
            JSItem item = JSItem.CreateArray();

            Assert.NotNull(item);
            Assert.True(item.IsContainer);
            Assert.True(item.IsArray);

            Exception exCaught = null;
            try
            {
                List<JSItem> list = item.GetList();
            }
            catch (Exception ex)
            {
                exCaught = ex;
            }
            Assert.Null(exCaught);
        }

        [Test(Description = "Insures items can be added to JSArray without string keys.")]
        public void Test_Array_AddNoKey()
        {
            JSItem item = JSItem.CreateArray();

            Assert.NotNull(item);
            Assert.True(item.IsContainer);
            Assert.True(item.IsArray);

            item.AddNull();

            Assert.True(item.Count == 1);
        }

        [Test(Description = "Insures items cannot be added to JSArray using string keys.")]
        public void Test_Array_AddWithKey()
        {
            JSItem item = JSItem.CreateArray();

            Assert.NotNull(item);
            Assert.True(item.IsContainer);
            Assert.True(item.IsArray);

            Exception? exCaught = null;
            try
            {
                item.AddNull("null");
            } catch (Exception ex)
            {
                exCaught = ex;
            }

            Assert.NotNull(exCaught);
        }

        [Test(Description = "Insures JSArray can be indexed by integer.")]
        public void Test_Array_IndexByInteger()
        {
            JSItem item = JSItem.CreateArray();

            Assert.NotNull(item);
            Assert.True(item.IsContainer);
            Assert.True(item.IsArray);
            item.AddNull();
            Assert.True(item.Count == 1);
            Assert.True(item[0].IsNull);
        }

        [Test(Description = "Insures JSArray cannot be indexed with string keys.")]
        public void Test_Array_IndexerByString()
        {
            JSItem item = JSItem.CreateArray();

            Assert.NotNull(item);
            Assert.True(item.IsContainer);
            Assert.True(item.IsArray);
            item.AddNull();
            Assert.True(item.Count == 1);

            Exception? exCaught = null;

            try
            {
                var element = item["key"];
            }
            catch (Exception ex)
            {
                exCaught = ex;
            }

            Assert.NotNull(exCaught);
        }

        [Test(Description = "Insure JSArray can be cleared of items.")]
        public void Test_Array_Clear()
        {
            JSItem item = JSItem.CreateArray();

            Assert.NotNull(item);
            Assert.True(item.IsContainer);
            Assert.True(item.IsArray);
            item.AddNull();
            Assert.True(item.Count == 1);
            Assert.True(item[0].IsNull);
            item.Clear();
            Assert.Zero(item.Count);
        }

        [Test(Description = "Insure items added to JSArray are of correct type and value.")]
        public void Test_Array_AddArray()
        {
            JSItem array = JSItem.CreateArray();
            Assert.NotNull(array);
            Assert.True(array.IsContainer);
            Assert.True(array.IsArray);

            array.AddObject();
            array.AddArray();
            array.AddString("three");
            array.AddNumber(4);
            array.AddBoolean(true);
            array.AddNull();

            Assert.True(array.Count == 6);
            Assert.True(array[0].IsObject);
            Assert.True(array[0].Count == 0);
            Assert.True(array[1].IsArray);
            Assert.True(array[1].Count == 0);
            Assert.True(array[2].IsString);
            Assert.True(array[2].GetString() == "three");
            Assert.True(array[3].IsNumber);
            Assert.AreEqual(4, array[3].GetInteger());
            Assert.True(array[4].IsBoolean);
            Assert.AreEqual(true, array[4].GetBoolean());
            Assert.True(array[5].IsNull);
        }

        [Test(Description = "Insure empty JSArray ToString() is correct.")]
        public void Test_Array_Empty_ToString()
        {
            JSItem array = JSItem.CreateArray();
            string? output = array.ToString();
            Assert.AreEqual("[]", output);
        }

    }
}
