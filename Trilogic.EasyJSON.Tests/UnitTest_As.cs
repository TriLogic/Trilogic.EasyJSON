using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trilogic.EasyJSON.Tests;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Trilogic.EasyJSON.Test
{
    public class UnitTest_As
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test(Description = "Insure connectness of JSItem 'As' methods.")]
        public void Test_As_Methods_With_Array()
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

            Assert.AreEqual(6, array.Count);

            Exception exCaptured = null;
            try
            {
                Type t = typeof(JSObject);
                Assert.True(array[0].As<JSObject>().GetType() == t);
                Assert.True(array[0].AsObject().GetType() == t);

                t = typeof(JSArray);
                Assert.True(array[1].As<JSArray>().GetType() == t);
                Assert.True(array[1].AsArray().GetType() == t);

                t = typeof(JSString);
                Assert.True(array[2].As<JSString>().GetType() == t);
                Assert.True(array[2].AsString().GetType() == t);

                t = typeof(JSNumber);
                Assert.True(array[3].As<JSNumber>().GetType() == t);
                Assert.True(array[3].AsNumber().GetType() == t);

                t = typeof(JSBoolean);
                Assert.True(array[4].As<JSBoolean>().GetType() == t);
                Assert.True(array[4].AsBoolean().GetType() == t);

                t = typeof(JSNull);
                Assert.True(array[5].As<JSNull>().GetType() == t);
                Assert.True(array[5].AsNull().GetType() == t);
            }
            catch (Exception ex)
            {
                Assert.IsNull(ex);
            }
        }

    }
}
