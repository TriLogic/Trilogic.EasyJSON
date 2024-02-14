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

        [Test(Description = "Insures arrays created from JSItem are arrays.")]
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

        [Test(Description = "Insures array items can be added without string keys.")]
        public void Test_Array_AddNoKey()
        {
            JSItem item = JSItem.CreateArray();

            Assert.NotNull(item);
            Assert.True(item.IsContainer);
            Assert.True(item.IsArray);

            item.AddNull();

            Assert.True(item.Count == 1);
        }

        [Test(Description = "Insures array items cannot be added using string keys.")]
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

        [Test(Description = "Insures arrays can be indexed by integer.")]
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

        [Test(Description = "Insures arrays cannot be iundex with string keys.")]
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

        [Test(Description = "Insures arrays can be cleared of items.")]
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

        [Test(Description = "Insures elements added using AddArray are arrays.")]
        public void Test_Array_AddArray()
        {
            JSItem array = JSItem.CreateArray();
            Assert.NotNull(array);
            Assert.True(array.IsContainer);
            Assert.True(array.IsArray);
            array.AddArray();
            Assert.True(array.Count == 1);
            Assert.True(array[0].IsArray);
        }

        [Test(Description = "Insure empty array ToString() is correct.")]
        public void Test_Array_Empty_ToString()
        {
            JSItem array = JSItem.CreateArray();
            string? output = array.ToString();
            Assert.AreEqual(output, "[]");
        }

    }
}
