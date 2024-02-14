using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Trilogic.EasyJSON;

namespace Trilogic.EasyJSON.App
{
    public static class TestHelp
    {
        public static JSItem BuildSimpleObject(JSItem parent = null, string key = null)
        {
            var item = parent == null ? JSItem.CreateArray() : parent.AddArray(key);
            return item.AddString("Bob", "fname")
                .AddString("Smith", "lname");
        }

        public static JSItem BuildSimpleArray(JSItem parent = null, string key = null)
        {
            var item = parent == null ? JSItem.CreateArray() : parent.AddArray(key);
            return item.AddString("Smith")
                .AddString("Jones")
                .AddString("Williams");
        }

        public static JSItem BuildArrayOfLocations(JSItem parent = null, string key = null)
        {
            var item = parent == null ? JSItem.CreateArray() : parent.AddArray(key);
            item.AddObject()
                .AddString("St. Louis", "city")
                .AddString("Missouri", "state")
                .Parent
            .AddObject()
                .AddString("Gatlinburg", "city")
                .AddString("Tennessee", "state");
            return item;
        }

        public static JSItem BuildArrayOfPeople(JSItem parent = null, string key = null)
        {
            var item = parent == null ? JSItem.CreateArray() : parent.AddArray(key);
            item.AddObject()
                .AddString("Bob", "fname")
                .AddString("Smith", "lname")
                .Parent
            .AddObject()
                .AddString("Tom", "fname")
                .AddString("Jones", "lname");
            return item;
        }

        public static JSItem BuildObjectOfArrays(JSItem parent = null, string key = null)
        {
            var item = parent == null ? JSItem.CreateObject() : parent.AddObject(key);
            BuildArrayOfPeople(item, "people");
            BuildArrayOfLocations(item, "locations");
            return item;
        }

        public static JSItem BuildArrayOfStrings(JSItem parent = null, string key = null)
        {
            var item = parent == null ? JSItem.CreateArray() : parent.AddArray(key);
            item.AddString("Smith")
                .AddString("Jones")
                .AddString("Williams");
            return item;
        }

        public static JSItem BuildArrayOfNumbers(JSItem parent = null, string key = null)
        {
            var item = parent == null ? JSItem.CreateArray() : parent.AddArray(key);
            item.AddNumber(100)
                .AddNumber(200)
                .AddNumber(300)
                .AddNumber(400)
                .AddNumber(500);
            return item;
        }

        public static JSItem BuildArrayOfNulls(JSItem parent = null, string key = null)
        {
            var item = parent == null ? JSItem.CreateArray() : parent.AddArray(key);
            item.AddNull()
                .AddNull()
                .AddNull();
            return item;
        }

        public static JSItem BuildEmptyArray(JSItem parent = null, string key = null)
        {
            return parent == null ? JSItem.CreateArray() : parent.AddArray(key);
        }

        public static JSItem BuildEmptyObject(JSItem parent = null, string key = null)
        {
            return parent == null ? JSItem.CreateObject() : parent.AddObject(key);
        }


        public static JSItem AddStringEmpty(JSItem parent, string key = null)
        {
            return parent.AddString(string.Empty, key);
        }
        public static JSItem AddStringEscapedUnicode(JSItem parent, string key = null)
        {
            return parent.AddString("\u0300\u0301\u0302\u0303\u0304\u0305\u0306", key);
        }

        public static JSItem AddStringEscapedCR(JSItem parent, string key = null)
        {
            return parent.AddString("\r", key);
        }
        public static JSItem AddStringEscapedLF(JSItem parent, string key = null)
        {
            return parent.AddString("\n", key);
        }
        public static JSItem AddStringEscapedCRLF(JSItem parent, string key = null)
        {
            return parent.AddString("\r\n", key);
        }
        public static JSItem AddStringEscapedTAB(JSItem parent, string key = null)
        {
            return parent.AddString("\t", key);
        }
        public static JSItem AddStringEscapedBS(JSItem parent, string key = null)
        {
            return parent.AddString("\\", key);
        }
        public static JSItem AddStringEscapedDQ(JSItem parent, string key = null)
        {
            return parent.AddString("\"", key);
        }

        public static JSItem BuildObjectOfEscapeStrings(JSItem parent = null, string key = null)
        {
            var item = parent == null ? JSItem.CreateObject() : parent.AddObject(key);
            AddStringEscapedBS(item, "backslash");
            AddStringEscapedCR(item, "cr");
            AddStringEscapedLF(item, "lf");
            AddStringEscapedCRLF(item, "crlf");
            AddStringEscapedTAB(item, "tab");
            AddStringEscapedDQ(item, "quote");
            AddStringEscapedUnicode(item, "unicode");
            return item;
        }

        public static JSItem BuildArrayOfEscapeStrings(JSItem parent = null, string key = null)
        {
            var item = parent == null ? JSItem.CreateObject() : parent.AddObject(key);
            AddStringEscapedBS(item);
            AddStringEscapedCR(item);
            AddStringEscapedLF(item);
            AddStringEscapedCRLF(item);
            AddStringEscapedTAB(item);
            AddStringEscapedDQ(item);
            AddStringEscapedUnicode(item);
            return item;
        }


        public static JSItem BuildArrayOfBooleans(JSItem parent = null, string key = null)
        {
            var item = parent == null ? JSItem.CreateArray() : parent.AddArray(key);
            return item.AddBoolean(true)
                .AddBoolean(false)
                .AddBoolean(true);
        }

        public static JSItem BuildArrayOfArrays(JSItem parent = null, string key = null)
        {
            var item = parent == null ? JSItem.CreateArray() : parent.AddArray(key);
            BuildArrayOfNumbers(item);
            BuildArrayOfNumbers(item);
            BuildArrayOfNumbers(item);
            return item;
        }

        public static JSItem BuildArrayOfEmptyArrays(JSItem parent = null, string key = null)
        {
            var item = parent == null ? JSItem.CreateArray() : parent.AddArray(key);
            BuildEmptyArray(item);
            BuildEmptyArray(item);
            BuildEmptyArray(item);
            return item;
        }

        public static JSItem BuildArrayOfEmptyObjects(JSItem parent = null, string key = null)
        {
            var item = parent == null ? JSItem.CreateArray() : parent.AddArray(key);
            BuildEmptyObject(item);
            BuildEmptyObject(item);
            BuildEmptyObject(item);
            return item;
        }

        public static JSItem BuildArrayOfObjects(JSItem parent = null, string key = null)
        {
            var item = parent == null ? JSItem.CreateArray() : parent.AddArray(key);
            return item;
        }

        public static JSItem BuildObjectOfBooleans(JSItem parent = null, string key = null)
        {
            var item = parent == null ? JSItem.CreateObject() : parent.AddObject(key);
            return item.AddBoolean(true, "true")
                .AddBoolean(false, "false");
        }

        public static JSItem BuildObjectOfNulls(JSItem parent = null, string key = null)
        {
            var item = parent == null ? JSItem.CreateObject() : parent.AddObject(key);
            return item.AddNull("name")
                .AddNull("address")
                .AddNull("city")
                .AddNull("state");
        }
        public static JSItem BuildObjectOfNumbers(JSItem parent = null, string key = null)
        {
            var item = parent == null ? JSItem.CreateObject() : parent.AddObject(key);
            return item.AddNumber(1, "one")
                .AddNumber(2, "two")
                .AddNumber(3, "three");
        }

        public static JSItem BuildObjectOfStrings(JSItem parent = null, string key = null)
        {
            var item = parent == null ? JSItem.CreateObject() : parent.AddObject(key);
            return item.AddString("Bob", "fname")
                .AddString("Jones", "lname");
        }

        public static JSItem BuildObjectOf_Arrays(JSItem parent = null, string key = null)
        {
            var item = parent == null ? JSItem.CreateObject() : parent.AddObject(key);
            BuildArrayOfBooleans(item, "booleans");
            BuildArrayOfNumbers(item, "numbers");
            BuildArrayOfStrings(item, "strings");
            BuildArrayOfNulls(item, "nulls");
            BuildArrayOfEmptyArrays(item, "empty_arrays");
            BuildArrayOfEmptyObjects(item, "empty_objects");
            return item;
        }

        public static JSItem BuildObjectOfObjects(JSItem parent = null, string key = null)
        {
            var item = parent == null ? JSItem.CreateObject() : parent.AddObject(key);
            BuildObjectOfBooleans(item, "booleans");
            BuildObjectOfNumbers(item, "numbers");
            BuildObjectOfStrings(item, "strings");
            BuildObjectOfNulls(item, "nulls");
            return item;
        }

        public static JSItem BuildCompleteObject()
        {
            JSItem json = JSItem.CreateObject();

            // Simple Things
            json.AddNull("Null");
            json.AddBoolean(true, "Boolean");
            json.AddNumber(100, "Number");
            json.AddString("Hello World!", "String");

            // Arrays of things
            BuildArrayOfStrings(json, "ArrayOf_Strings");
            BuildArrayOfBooleans(json, "ArrayOf_Booleans");
            BuildArrayOfNumbers(json, "ArrayOf_Numbers");
            BuildArrayOfNulls(json, "ArrayOf_Nulls");
            BuildArrayOfEmptyArrays(json, "ArrayOf_EmptyArrays");
            BuildArrayOfEmptyObjects(json, "ArrayOf_EmptyObjects");

            // Objects of Things
            BuildObjectOfEscapeStrings(json, "ObjectOf_EscapedStrings");
            BuildObjectOfBooleans(json, "ObjectOf_Booleans");
            BuildObjectOfStrings(json, "ObjectOf_Numbers");
            BuildObjectOfStrings(json, "ObjectOf_Strings");
            BuildObjectOfStrings(json, "ObjectOf_Nulls");
            BuildObjectOfArrays(json, "ObjectOf_Arrays");
            BuildObjectOfObjects(json, "ObjectOf_Objects");

            return json;
        }
    }
}
