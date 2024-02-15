namespace Trilogic.EasyJSON.Test
{
    public class TestNull
    {
        static JSItem? json = null;

        [SetUp]
        public void Setup()
        {
            json = JSItem.CreateArray().AddNull();
        }

        [Test(Description = "Insure JSNull is the null value.")]
        public void Test_NullIsNull()
        {
            Assert.NotNull(json);
            Assert.True(json.IsContainer);
            Assert.True(json.IsArray);
            Assert.True(json.Count == 1);
            Assert.True(json[0].IsNull);
            Assert.False(json[0].IsBoolean);
            Assert.False(json[0].IsNumber);
            Assert.False(json[0].IsString);
            Assert.False(json[0].IsObject);
        }

        [Test(Description = "Insure JSNull toString() returns the correct value.")]
        public void Test_Null_ToString_IsNull()
        {
            Assert.NotNull(json);
            Assert.True(json.IsContainer);
            Assert.True(json.IsArray);
            Assert.True(json.Count == 1);
            Assert.True(json[0].IsNull);
            string? output = json[0].ToString();
            Assert.AreEqual("null", output);
        }

    }
}