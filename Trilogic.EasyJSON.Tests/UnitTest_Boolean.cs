using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trilogic.EasyJSON.Test
{
    public class UnitTest_Boolean
    {
        static JSItem json = null;

        [SetUp]
        public void Setup()
        {
            json = JSItem.CreateArray().AddTrue().AddFalse();
        }

        [Test(Description = "Insure booleans are added via AddBoolean()")]
        public void Test_BooleanIsBoolean()
        {
            Assert.True(json.IsArray);
            Assert.True(json.Count == 2);

            JSItem item = json[0];
            Assert.True(item.IsBoolean);
            Assert.False(item.IsContainer);
            Assert.False(item.IsArray);
            Assert.False(item.IsObject);
            Assert.False(item.IsNumber);
            Assert.False(item.IsString);
            Assert.False(item.IsNull);
            Assert.NotNull(item.Value);
            Assert.True(item.GetBoolean() == true);

            item = json[1];
            Assert.True(item.IsBoolean);
            Assert.False(item.IsContainer);
            Assert.False(item.IsArray);
            Assert.False(item.IsObject);
            Assert.False(item.IsNumber);
            Assert.False(item.IsString);
            Assert.False(item.IsNull);
            Assert.NotNull(item.Value);
            Assert.True(item.GetBoolean() == false);
        }
    }
}
