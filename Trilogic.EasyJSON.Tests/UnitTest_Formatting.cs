using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trilogic.EasyJSON.Tests;

namespace Trilogic.EasyJSON.Test
{
    public class UnitTest_Formatting
    {
        JSItem json = null;

        [SetUp]
        public void Setup()
        {
            json = TestHelp.BuildCompleteObject();
        }

        [Test(Description = "Test reading and writing of JSON in Default Format.")]
        public void Test_Formating_ToString_Default()
        {
            var jsonOut = json.ToString();
            var temp = JSItem.Parse(jsonOut);
            var jsonInp = temp.ToString();
            Assert.AreEqual(jsonOut, jsonInp);
        }


        [Test(Description = "Test reading and writing of JSON in Compact Format.")]
        public void Test_Formating_ToString_Compact()
        {
            JSOutputFormat format = JSOutputFormat.OutputCompact;

            var jsonOut = json.ToString(format);
            var temp = JSItem.Parse(jsonOut);
            var jsonInp = temp.ToString(format);
            Assert.AreEqual(jsonOut, jsonInp);
        }

        [Test(Description = "Test reading and writing of JSON in Linear Format.")]
        public void Test_Formating_ToString_Linear()
        {
            JSOutputFormat format = JSOutputFormat.OutputLinear;

            var jsonOut = json.ToString(format);
            var temp = JSItem.Parse(jsonOut);
            var jsonInp = temp.ToString(format);
            Assert.AreEqual(jsonOut, jsonInp);
        }

        [Test(Description = "Test reading and writing of JSON in KNR Format.")]
        public void Test_Formating_ToString_KNR()
        {
            JSOutputFormat format = JSOutputFormat.OutputKNR;

            var jsonOut = json.ToString(format);
            var temp = JSItem.Parse(jsonOut);
            var jsonInp = temp.ToString(format);
            Assert.AreEqual(jsonOut, jsonInp);
        }

        [Test(Description = "Test reading and writing of JSON in Allman Format.")]
        public void Test_Formating_ToString_Allman()
        {
            JSOutputFormat format = JSOutputFormat.OutputAllman;

            var jsonOut = json.ToString(format);
            var temp = JSItem.Parse(jsonOut);
            var jsonInp = temp.ToString(format);
            Assert.AreEqual(jsonOut, jsonInp);
        }

        [Test(Description = "Test reading and writing of JSON in Whitesmith Format.")]
        public void Test_Formating_ToString_Whitesmith()
        {
            JSOutputFormat format = JSOutputFormat.OutputWhitesmith;

            var jsonOut = json.ToString(format);
            var temp = JSItem.Parse(jsonOut);
            var jsonInp = temp.ToString(format);
            Assert.AreEqual(jsonOut, jsonInp);
        }

    }
}
