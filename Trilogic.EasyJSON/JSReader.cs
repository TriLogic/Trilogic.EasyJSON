using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Trilogic.EasyJSON
{
    internal class JSReader
    {
        #region Constants
        const string InvalidArrSyntax = "JSON: Invalid Array Syntax";
        const string InvalidObjSyntax = "JSON: Invalid Object Syntax";
        #endregion

        #region Private Members
        private JSTokenizer _tokens;
        private JSItem _item;
        #endregion

        #region Constructors and Destructors
        public JSReader(Stream stream)
        {
            _tokens = new JSTokenizer(new StreamReader(stream).ReadToEnd());
        }
        public JSReader(StreamReader reader)
        {
            _tokens = new JSTokenizer(reader.ReadToEnd());
        }
        public JSReader(StringBuilder input)
        {
            _tokens = new JSTokenizer(input);
        }
        public JSReader(string input)
        {
            _tokens = new JSTokenizer(input);
        }
        internal JSReader(JSTokenizer tokens)
        {
            _tokens = tokens;
        }
        #endregion

        #region Public Parsing Methods
        public JSItem Parse()
        {
            if (! ExpectToken(JSTokenType.TK_ARRAYL, JSTokenType.TK_OBJECTL))
                throw new JSException("Expected Array or Object");

            if (_tokens.TokenType == JSTokenType.TK_ARRAYL)
                ParseArrayInternal();
            else
                ParseObjectInternal();

            return _item;
        }

        public JSArray ParseArray()
        {
            if (!ExpectToken(JSTokenType.TK_ARRAYL))
                throw new JSException(InvalidArrSyntax);

            ParseArrayInternal();
            return (JSArray)_item;
        }

        public JSObject ParseObject()
        {
            if (!ExpectToken(JSTokenType.TK_OBJECTL))
                throw new JSException(InvalidObjSyntax);

            ParseObjectInternal();
            return (JSObject)_item;
        }
        #endregion

        #region Internal Parsing Methods
        private void ParseObjectInternal(string key = null)
        {
            // create the new object item
            _item = new JSObject(_item);

            // if there is a parent the add the new
            // object to the parents collection
            if (_item.Parent != null)
            {
                if (_item.Parent.IsObject)
                    _item.Parent.Add(_item, key);
                else
                    _item.Parent.Add(_item);
            }

            // attempt to parse a child value
            bool more = ParseObjectItem();
            while (more )
            {
                // expect to parse either a comma or end of object
                if (!ExpectToken(JSTokenType.TK_COMMA, JSTokenType.TK_OBJECTR))
                    throw new JSException(InvalidObjSyntax);

                // if we found an object close then we're done here
                if (_tokens.TokenType == JSTokenType.TK_OBJECTR)
                    break;

                // attempt to parse another object
                more = ParseObjectItem();
            }

            // we didn't find any child items so if the token is anything
            // other than an end of object we have a syntax error
            if (_tokens.TokenType != JSTokenType.TK_OBJECTR)
                throw new JSException(InvalidObjSyntax);

            // restore the parent to the top of stack
            if (_item.Parent != null)
                _item = _item.Parent;
        }

        private void ParseArrayInternal(string key = null)
        {
            // create the new array
            _item = new JSArray(_item);

            // add the array to the parent's collection
            if (_item.Parent != null)
            {
                if (_item.Parent.IsObject)
                    _item.Parent.Add(_item, key);
                else
                    _item.Parent.Add(_item);
            }

            // attempt to parse an item
            bool more = ParseSubItem(JSTokenType.TK_ARRAYR);
            while (more)
            {
                // expect either a comma or enod of array
                if (!ExpectToken(JSTokenType.TK_COMMA, JSTokenType.TK_ARRAYR))
                    throw new JSException(InvalidArrSyntax);

                // end of array means we're done
                if (_tokens.TokenType == JSTokenType.TK_ARRAYR)
                    break;

                // attempt to parse the next array item
                more = ParseSubItem(JSTokenType.TK_ARRAYR);
            }

            // we didn't find any child items so if the token is anything
            // other than an end of array we have a syntax error
            if (_tokens.TokenType != JSTokenType.TK_ARRAYR)
                throw new JSException(InvalidArrSyntax);

            // restore the parent to the top of stack
            if (_item.Parent != null)
                _item = _item.Parent;
        }

        private bool ParseSubItem(JSTokenType ExitToken, string key = null)
        {
            if (!GetToken())
            {
                throw new JSException(ExitToken == JSTokenType.TK_ARRAYL ?
                    InvalidArrSyntax :
                    InvalidObjSyntax);
            }

            if (_tokens.TokenType == ExitToken)
                return false;

            switch (_tokens.TokenType)
            {
                case JSTokenType.TK_BOOLEAN:
                    _item.AddBoolean(_tokens.TokenAsBoolean, key);
                    return true;

                case JSTokenType.TK_NUMBER:
                    _item.AddNumber(_tokens.TokenAsNumber, key);
                    return true;

                case JSTokenType.TK_STRING:
                    StringBuilder temp = new StringBuilder(_tokens.TokenAsString);
                    _item.AddString(temp.ToString(), key);
                    return true;

                case JSTokenType.TK_NULL:
                    _item.AddNull(key);
                    return true;

                case JSTokenType.TK_ARRAYL:
                    ParseArrayInternal(key);
                    return true;

                case JSTokenType.TK_OBJECTL:
                    ParseObjectInternal(key);
                    return true;
            }

            throw new JSException(ExitToken == JSTokenType.TK_ARRAYL ?
                InvalidArrSyntax :
                InvalidObjSyntax);
        }

        private bool ParseObjectItem()
        {
            string tmp = string.Empty;

            if (!ExpectToken(JSTokenType.TK_STRING, JSTokenType.TK_OBJECTR))
                throw new JSException(InvalidObjSyntax);

            // empty object condition
            if (_tokens.TokenType == JSTokenType.TK_OBJECTR)
                return false;

            string key = _tokens.TokenAsString;

            if (_item.ContainsKey(key))
                throw new JSException(@"JSON: Duplicate Object Key ({key})");

            if (!ExpectToken(JSTokenType.TK_COLON))
                throw new JSException(InvalidObjSyntax);

            if (!ParseSubItem(JSTokenType.TK_OBJECTR, key))
                throw new JSException(InvalidObjSyntax);

            return true;
        }

        private bool GetToken()
        {
            // return only non-whitespace tokens
            while (_tokens.GetToken())
            {
                if (_tokens.TokenType != JSTokenType.TK_WHITE &&
                    _tokens.TokenType != JSTokenType.TK_COMMENT_S &&
                    _tokens.TokenType != JSTokenType.TK_COMMENT_M)
                    return true;
            }

            // end of stream
            return false;
        }
        #endregion

        #region Token Helpers
        private bool ExpectToken(JSTokenType type)
        {
            return GetToken() && _tokens.TokenType == type;
        }

        private bool ExpectToken(JSTokenType typeA, JSTokenType typeB)
        {
            return GetToken() && (_tokens.TokenType == typeA || _tokens.TokenType == typeB);
        }
        #endregion

        #region External Static Methods
        public static JSItem Parse(string json)
        {
            return ParseTokens(new JSTokenizer(json));
        }
        public static JSItem Parse(StringBuilder json)
        {
            return ParseTokens(new JSTokenizer(json));
        }
        public static JSItem Parse(System.IO.StreamReader json)
        {
            return Parse(json.ReadToEnd());
        }
        public static JSItem Parse(System.IO.Stream json)
        {
            return Parse(new System.IO.StreamReader(json));
        }

        public static JSArray ParseArray(string json)
        {
            return new JSReader(new JSTokenizer(json))
                .ParseArray();
        }
        public static JSArray ParseArray(StringBuilder json)
        {
            return new JSReader(new JSTokenizer(json))
                .ParseArray();
        }
        public static JSArray ParseArray(System.IO.StreamReader reader)
        {
            return ParseArray(reader.ReadToEnd());
        }
        public static JSArray ParseArray(System.IO.Stream stream)
        {
            return ParseArray(new System.IO.StreamReader(stream));
        }

        public static JSObject ParseObject(string json)
        {
            return new JSReader(new JSTokenizer(json))
                .ParseObject();
        }
        public static JSObject ParseObject(StringBuilder json)
        {
            return new JSReader(new JSTokenizer(json))
                .ParseObject();
        }
        public static JSObject ParseObject(System.IO.StreamReader reader)
        {
            return ParseObject(reader.ReadToEnd());
        }
        public static JSObject ParseObject(System.IO.Stream stream)
        {
            return ParseObject(new System.IO.StreamReader(stream));
        }

        private static JSItem ParseTokens(JSTokenizer tokens)
        {
            JSReader reader = new JSReader(tokens);
            return reader.Parse();
        }
        #endregion
    }
}
