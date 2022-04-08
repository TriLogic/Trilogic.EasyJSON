using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Trilogic.EasyJSON
{
    public enum JSOutputFormat
    {
        OutputCompact = 0,
        OutputLinear,
        OutputKNR,
        OutputAllman,
        OutputWhitesmith
    }
    public class JSOutputContext
    {
        #region Class Instance Members
        private List<JSItem> mItemArray = new List<JSItem>();
        private List<string> mTextArray = new List<string>();
        private StreamWriter mStream = null;

        private string mIndentText = string.Empty;
        private string mArrayEmptyText = string.Empty;
        private string mObjectEmptyText = string.Empty;
        #endregion

        #region Constructors and Destructors
        internal JSOutputContext(StreamWriter stream)
        {
            mStream = stream;
            mIndentText = "\t";
            ResetStacks();
        }
        internal JSOutputContext(StreamWriter stream, string IndentText)
        {
            mStream = stream;
            mIndentText = IndentText;
            ResetStacks();
        }
        internal JSOutputContext(StreamWriter stream, string IndentText, string ObjectEmptyText, string ArrayEmptyText)
        {
            mStream = stream;
            mIndentText = IndentText;
            mArrayEmptyText = ArrayEmptyText;
            mObjectEmptyText = ObjectEmptyText;
            ResetStacks();
        }
        #endregion

        #region Property Definitions
        public string IndentText 
        { 
            get { return mIndentText; } set { mIndentText = value ?? string.Empty; } 
        }
        public string IndentString 
        { 
            get { return mItemArray.Count >= 0 ? mTextArray[mItemArray.Count] : string.Empty; } 
        }
        public string OutdentString 
        { 
            get { return mItemArray.Count > 1 ? mTextArray[mItemArray.Count - 1] : string.Empty; } 
        }
        public bool IsTopLevel 
        { 
            get { return mItemArray.Count == 1; } 
        }
        public bool InContainer
        {
            get { return (mItemArray.Count > 1 && mItemArray[mItemArray.Count - 1].IsContainer); }
        }
        public bool InArray
        {
            get { return mItemArray.Count > 1 && mItemArray[mItemArray.Count - 1].IsArray; }
        }
        public bool InObject
        {
            get { return mItemArray.Count > 1 && mItemArray[mItemArray.Count - 1].IsObject; }
        }
        public int StackLevel 
        { 
            get { return mItemArray.Count; } 
        }
        public StreamWriter Writer 
        { 
            get { return mStream; } 
            set { mStream = value; } 
        }
        public string EmptyArrayInnerText
        {
            get { return mArrayEmptyText; }
            set { mArrayEmptyText = value ?? string.Empty; }
        }
        public string EmptyObjectInnerText
        {
            get { return mObjectEmptyText; }
            set { mObjectEmptyText = value ?? string.Empty; }
        }
        #endregion

        #region Stack Manipulation
        public void PushObject(JSItem item)
        {
            // push the item onto the item array
            mItemArray.Add(item);

            // push new indentation text if it does not already exist
            if (mTextArray.Count <= mItemArray.Count)
                mTextArray.Add(mTextArray[mItemArray.Count - 1] + mIndentText);
        }

        public void PopObject()
        {
            if (mItemArray.Count == 0)
                return;
            
            //JSItem item = mItemArray[mItemArray.Count - 1];
            mItemArray.RemoveAt(mItemArray.Count - 1);
        }

        public JSItem PeekObject()
        {
            return mItemArray.Count < 1 ? null : mItemArray[mItemArray.Count - 1];
        }
        #endregion

        #region Helper Functions

        #region Appending Text
        public void Write(char c)
        {
            Writer.Write(c);
        }
        public void Write(string text)
        {
            Writer.Write(text);
        }
        #endregion

        #region Methods Appending a Newline
        public void WriteLine() => mStream.WriteLine();
        public void WriteLine(char c) => mStream.WriteLine(c);
        public void WriteLine(string s) => mStream.WriteLine(s);
        public void WriteLineIndent() => mStream.WriteLine(IndentString);
        public void WriteLineIndentPlus() => mStream.WriteLine(IndentString + mIndentText);
        public void WriteLineOutdent() => mStream.WriteLine(OutdentString);
        #endregion

        #region Methods Prepending Newline Before Writing Text
        public void WriteLinefeed(char c)
        {
            mStream.WriteLine();
            mStream.Write(c);
        }
        public void WriteLinefeed(string c)
        {
            mStream.WriteLine();
            mStream.Write(c);
        }
        #endregion

        #region Appending Indentation
        public void WriteIndent()
        {
            mStream.Write(IndentString);
        }
        public void WriteIndentPlus()
        {
            mStream.Write(IndentString);
            mStream.Write(mIndentText);
        }
        public void WriteOutdent()
        {
            mStream.Write(OutdentString);
        }
        #endregion

        #region Append Linefeed then Indentation then Text
        public void WriteLinefeedIndent()
        {
            mStream.WriteLine();
            mStream.Write(IndentString);
        }
        public void WriteLinefeedIndent(char c)
        {
            mStream.WriteLine();
            mStream.Write(IndentString);
            mStream.Write(c);
        }
        public void WriteLinefeedOutdent()
        {
            mStream.WriteLine();
            mStream.Write(OutdentString);
        }
        public void WriteLinefeedOutdent(char c)
        {
            mStream.WriteLine();
            mStream.Write(OutdentString);
            mStream.Write(c);
        }
        #endregion

        #region AppendText then Linefeed then Indentation

        public void WriteTextLinefeedIndent(char c)
        {
            mStream.WriteLine(c);
            mStream.Write(IndentString);
        }
        public void WriteTextLinefeedIndent(string s)
        {
            mStream.WriteLine(s);
            mStream.Write(IndentString);
        }
        public void WriteTextLinefeedOutdent(char c)
        {
            mStream.WriteLine(c);
            mStream.Write(OutdentString);
        }
        public void WriteTextLinefeedOutdent(string s)
        {
            mStream.WriteLine(s);
            mStream.Write(OutdentString);
        }
        #endregion

        #region Appending Empty Containers
        public void WriteArrayEmpty()
        {
            mStream.Write("[]");
        }
        public void WriteArrayEmptyExpanded()
        {
            mStream.Write('[');
            mStream.Write(mArrayEmptyText);
            mStream.Write(']');
        }
        public void WriteObjectEmpty()
        {
            mStream.Write("{}");
        }
        public void WriteObjectEmptyExpanded()
        {
            mStream.Write('[');
            mStream.Write(mObjectEmptyText);
            mStream.Write(']');
        }
        #endregion

        #region Opening and Closing Non-Empty Arrays
        public void WriteArrayOpen() => mStream.Write('[');
        public void WriteArrayOpenSpace() => mStream.Write("[ ");
        public void WriteArrayOpenLinefeed() => WriteLine('[');
        public void WriteArrayOpenLinefeedIndent() => WriteTextLinefeedIndent('[');
        public void WriteArrayOpenLinefeedOutdent() => WriteTextLinefeedOutdent('[');
        public void WriteArrayClose() => mStream.Write(']');
        public void WriteArrayCloseLinefeed() => mStream.WriteLine(']');
        public void WriteArrayCloseLinefeedIndent() => WriteTextLinefeedIndent(']');
        public void WriteArrayCloseLinefeedOutdent() => WriteTextLinefeedOutdent(']');
        #endregion

        #region Opening and Closing non-Empty Objects
        public void WriteObjectOpen() => mStream.Write('{');
        public void WriteObjectOpenSpace() => mStream.Write("{ ");
        public void WriteObjectOpenLinefeed() => WriteLine('{');
        public void WriteObjectOpenLinefeedIndent() => WriteTextLinefeedIndent('{');
        public void WriteObjectOpenLinefeedOutdent() => WriteTextLinefeedOutdent('{');
        public void WriteObjectClose() => mStream.Write('}');
        public void WriteObjectCloseLinefeed() => mStream.WriteLine('}');
        public void WriteObjectCloseLinefeedIndent() => WriteTextLinefeedIndent('}');
        public void WriteObjectCloseLinefeedOutdent() => WriteTextLinefeedOutdent('}');
        #endregion

        #region Reset Stacks
        internal void ResetStacks()
        {
            mItemArray.Clear();
            mTextArray.Clear();
            mTextArray.Add(string.Empty);
        }
        #endregion

        #endregion
    }

    // Define a delegate type for formatter functions
    public delegate void JSFormatFunc(JSOutputContext context);
    public class JSFormatter
    {
        // Fixme: Move These items
        #region Static Function Delegates
        public static JSFormatFunc FnNoOutput = delegate (JSOutputContext context) { /* empty */ };
        public static JSFormatFunc FnSpace = delegate (JSOutputContext context) { context.Writer.Write(' '); };
        public static JSFormatFunc FnIndent = delegate (JSOutputContext context) { context.WriteIndent(); };
        public static JSFormatFunc FnIndentPlus = delegate (JSOutputContext context) { context.WriteIndentPlus(); };
        public static JSFormatFunc FnOutdent = delegate (JSOutputContext context) { context.WriteOutdent(); };

        public static JSFormatFunc FnComma = delegate (JSOutputContext context) { context.Write(','); };
        public static JSFormatFunc FnCommaSpace = delegate (JSOutputContext context) { context.Write(", "); };
        public static JSFormatFunc FnCommaSpaceLinefeed = delegate (JSOutputContext context) { context.WriteLine(", ");};
        public static JSFormatFunc FnCommaLinefeed = delegate (JSOutputContext context) { context.WriteLine(','); };
        public static JSFormatFunc FnSpaceComma = delegate (JSOutputContext context) { context.Write(" ,"); };
        public static JSFormatFunc FnSpaceCommaLinefeedIndent = delegate (JSOutputContext context) { context.WriteTextLinefeedIndent(" ,"); };
        public static JSFormatFunc FnSpaceCommaLinefeedOutdent = delegate (JSOutputContext context) { context.WriteTextLinefeedOutdent(" ,"); };
        public static JSFormatFunc FnSpaceCommaLinefeed = delegate (JSOutputContext context) { context.WriteLine(" ,"); };
        public static JSFormatFunc FnCommaLinefeedIndent = delegate (JSOutputContext context) { context.WriteLine(','); context.WriteIndent();  };
        public static JSFormatFunc FnCommaLinefeedOutdent = delegate (JSOutputContext context) { context.WriteLine(','); context.WriteOutdent(); };
        public static JSFormatFunc FnSpaceCommaSpace = delegate (JSOutputContext context) { context.Write(" , "); };
        public static JSFormatFunc FnColon = delegate (JSOutputContext context) { context.Write(':'); };
        public static JSFormatFunc FnColonLinefeed = delegate (JSOutputContext context) { context.WriteLine(':'); };
        public static JSFormatFunc FnColonSpace = delegate (JSOutputContext context) { context.Write(": "); };
        public static JSFormatFunc FnSpaceColon = delegate (JSOutputContext context) { context.Write(" :"); };
        public static JSFormatFunc FnSpaceColonLinefeed = delegate (JSOutputContext context) { context.WriteLine(" :"); };
        public static JSFormatFunc FnSpaceColonSpace = delegate (JSOutputContext context) { context.Write(" : "); };
        public static JSFormatFunc FnLinefeedIndent = delegate (JSOutputContext context) { context.WriteLinefeedIndent(); };
        public static JSFormatFunc FnLinefeedOutdent = delegate (JSOutputContext context) { context.WriteLinefeedOutdent(); };

        // empty array
        public static JSFormatFunc FnEmptyArray = delegate (JSOutputContext context) { context.WriteArrayEmpty(); };
        public static JSFormatFunc FnEmptyArrayExpanded = delegate (JSOutputContext context) { context.WriteArrayEmptyExpanded(); };

        public static JSFormatFunc FnArrayOpen = delegate (JSOutputContext context) { context.WriteObjectOpen(); };
        public static JSFormatFunc FnArrayOpenLinefeedIndent = delegate (JSOutputContext context) { context.WriteArrayOpenLinefeedIndent(); };
        public static JSFormatFunc FnArrayOpenLinefeedOutdent = delegate (JSOutputContext context) { context.WriteArrayOpenLinefeedOutdent(); };
        public static JSFormatFunc FnArrayClose = delegate (JSOutputContext context) { context.WriteArrayClose(); };
        public static JSFormatFunc FnArrayCloseLinefeedIndent = delegate (JSOutputContext context) { context.WriteArrayCloseLinefeedIndent(); };
        public static JSFormatFunc FnArrayCloseLinefeedOutdent = delegate (JSOutputContext context) { context.WriteTextLinefeedOutdent(']'); };
        public static JSFormatFunc FnArrayLinefeedIndentClose = delegate (JSOutputContext context) { context.WriteLinefeedIndent(']'); };
        public static JSFormatFunc FnArrayLinefeedOutdentClose = delegate (JSOutputContext context) { context.WriteLinefeedOutdent(']'); };

        public static JSFormatFunc FnEmptyObject = delegate (JSOutputContext context) { context.WriteObjectEmpty(); };
        public static JSFormatFunc FnEmptyObjectExpanded = delegate (JSOutputContext context) { context.WriteObjectEmptyExpanded(); };

        public static JSFormatFunc FnObjectOpen = delegate (JSOutputContext context) { context.WriteObjectOpen(); };
        public static JSFormatFunc FnObjectOpenLinefeed = delegate (JSOutputContext context) { context.WriteObjectOpenLinefeed(); };
        public static JSFormatFunc FnObjectOpenLinefeedIndent = delegate (JSOutputContext context) { context.WriteTextLinefeedIndent('{');  };
        public static JSFormatFunc FnObjectOpenLinefeedOutdent = delegate (JSOutputContext context) { context.WriteTextLinefeedOutdent('{'); };
        public static JSFormatFunc FnWriteObjectClose = delegate (JSOutputContext context) { context.Write('}'); };
        public static JSFormatFunc FnWriteObjectCloseLinefeedIndent = delegate (JSOutputContext context) { context.WriteTextLinefeedIndent('}'); };
        public static JSFormatFunc FnWriteObjectCloseLinefeedOutdent = delegate (JSOutputContext context) { context.WriteTextLinefeedOutdent('}'); };
        public static JSFormatFunc FnWriteObjectLinefeedIndentClose = delegate (JSOutputContext context) { context.WriteLinefeedIndent('}'); };
        public static JSFormatFunc FnWriteObjectLinefeedOutdentClose = delegate (JSOutputContext context) { context.WriteLinefeedOutdent('}'); };
        #endregion

        #region Class Instance Members
        private string mIndentText = "  ";
        private string mArrayEmptyText = String.Empty;
        private string mObjectEmptyText = String.Empty;

        #region Function Based Format Functions

        // Object Values
        private JSFormatFunc mObjectEmptyFunc = FnEmptyObject;
        private JSFormatFunc mObjectOpenFunc = FnObjectOpen;
        private JSFormatFunc mObjectKeyFunc = FnNoOutput;
        private JSFormatFunc mObjectColonFunc = FnColon;
        private JSFormatFunc mObjectValueFunc = FnNoOutput;
        private JSFormatFunc mObjectCommaFunc = FnComma;
        private JSFormatFunc mObjectCloseFunc = FnWriteObjectClose;

        // Array Values
        private JSFormatFunc mArrayEmptyFunc = FnEmptyArray;
        private JSFormatFunc mArrayOpenFunc = FnArrayOpen;
        private JSFormatFunc mArrayValueFunc = FnNoOutput;
        private JSFormatFunc mArrayCommaFunc = FnComma;
        private JSFormatFunc mArrayCloseFunc = FnArrayClose;
        #endregion
 
        private StringBuilder bufExpand = new StringBuilder();
        #endregion

        #region Constructors and Destructors
        public JSFormatter()
        {
            SetFormatKNR();
        }

        public JSFormatter(JSOutputFormat format)
        {
            switch (format)
            {
                case JSOutputFormat.OutputAllman:
                    SetFormatAllman();
                    break;

                case JSOutputFormat.OutputKNR:
                    SetFormatKNR();
                    break;

                case JSOutputFormat.OutputWhitesmith:
                    SetFormatWhitesmith();
                    break;

                case JSOutputFormat.OutputLinear:
                    SetFormatLinear(true);
                    break;

                default:
                    SetFormatCompact();
                    break;
            }
        }

        public JSFormatter(JSOutputFormat format, string IndentText = "  ", bool ExpandEmpty = false, bool BlankBeforeColon = false)
        {
            switch (format)
            {
                case JSOutputFormat.OutputAllman:
                    SetFormatAllman(IndentText, ExpandEmpty, BlankBeforeColon);
                    break;

                case JSOutputFormat.OutputKNR:
                    SetFormatKNR(IndentText, ExpandEmpty, BlankBeforeColon);
                    break;

                case JSOutputFormat.OutputWhitesmith:
                    SetFormatWhitesmith(IndentText, ExpandEmpty, BlankBeforeColon);
                    break;

                default:
                    SetFormatLinear(ExpandEmpty);
                    break;
            }
        }
        #endregion

        private static string NoNull(string value) => string.IsNullOrEmpty(value) ? string.Empty : value;
        public string Format(JSItem Item)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            //JSOutputContext context = new JSOutputContext(writer, mIndentText);
            Write(writer, Item);

            writer.Flush();

            stream.Position = 0;
            StreamReader reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }

        #region Writing to Streams
        public void Write(Stream stream, JSItem item)
        {
            Write(new StreamWriter(stream), item);
        }

        public void Write(StreamWriter writer, JSItem Item)
        {
            JSOutputContext context = new JSOutputContext(writer, mIndentText);
            context.EmptyArrayInnerText = mArrayEmptyText;
            context.EmptyObjectInnerText = mObjectEmptyText;
            ToWriter(context, Item, true);
        }

        private void ToWriter(JSOutputContext context, JSItem Item, bool IsBase = false)
        {
            if (Item.IsObject)
                ToWriterObject(context, (JSObject)Item, IsBase);
            else if (Item.IsArray)
                ToWriterArray(context, (JSArray)Item, IsBase);
            else
                context.Writer.Write(Item.ToString());
        }

        private void ToWriterArray(JSOutputContext context, JSArray Item, bool IsBase = false)
        {
            // If this array is empty the return the empty representation
            if (Item.Count == 0)
            {
                mArrayEmptyFunc(context);
                return;
            }

            int OutCount = 0;

            // push the object on the stack
            context.PushObject(Item);

            // call the open object function
            mArrayOpenFunc(context);

            // for each element in the array
            foreach (JSItem child in Item.Value)
            {
                // increment the item count
                OutCount += 1;

                // if this is not the first object we need to add a comma
                if (OutCount > 1)
                    mArrayCommaFunc(context);

                // prepare for the array value ONLY if non-container
                if (child.IsContainer == false || (child.IsContainer && child.Count == 0))
                    mArrayValueFunc(context);

                // is this item and empty container ?
                if (child.IsContainer && child.Count > 0)
                {
                    // yes - write according to empty container rules
                    ToWriter(context, child);
                }
                else
                {
                    // no - write using array value prefix and suffix
                    ToWriter(context, child);
                }
            }

            mArrayCloseFunc(context);

            context.PopObject();
        }

        private void ToWriterObject(JSOutputContext context, JSObject Item, bool IsBase = false)
        {
            int OutCount = 0;

            // If this object is empty the return the empty representation
            if (Item.Count == 0)
            {
                mObjectEmptyFunc(context);
                return;
            }

             // push the object
            context.PushObject(Item);

            // call the open function
            mObjectOpenFunc(context);

            // for each value in the object
            foreach (string Key in Item.Value.Keys)
            {
                // retrieve the item in question
                JSItem child = Item.Value[Key];

                // increment the count of object output
                OutCount += 1;

                // if not the first item add comm seperator
                if (OutCount > 1)
                {
                    mObjectCommaFunc(context);
                }

                // prepare for the key
                mObjectKeyFunc(context);

                // write the item key
                context.Write('"');
                context.Write(Key);
                context.Write('"');

                // write the colon seperator
                mObjectColonFunc(context);

                // is the current item an empty container ?
                if ((child.IsArray || child.IsObject) && (child.Count == 0))
                {
                    // write empty container as if its just another value
                    ToWriter(context, child);
                }
                else
                {
                    ToWriter(context, child);
                }
            }

            // close the object
            mObjectCloseFunc(context);

            // remove it from the stack
            context.PopObject();
        }
        #endregion

        #region Setting Output Formats

        public void SetFormat(JSOutputFormat format)
        {
            SetFormat(format, "\t");
        }
        public void SetFormat(JSOutputFormat format, bool ExpandEmpty = true, bool BlankBeforeColon = false, bool BlankBeforeComma = false)
        {
            SetFormat(format, "\t", ExpandEmpty, BlankBeforeColon, BlankBeforeComma);
        }
        public void SetFormat(JSOutputFormat format, string IndentText, bool ExpandEmpty = true, bool BlankBeforeColon = false, bool BlankBeforeComma = false)
        {
            switch (format)
            {
                case JSOutputFormat.OutputCompact:
                    SetFormatCompact();
                    break;
                case JSOutputFormat.OutputLinear:
                    SetFormatLinear(ExpandEmpty, BlankBeforeColon, BlankBeforeComma);
                    break;
                case JSOutputFormat.OutputKNR:
                    SetFormatKNR(IndentText, ExpandEmpty, false, BlankBeforeColon, BlankBeforeComma);
                    break;
                case JSOutputFormat.OutputWhitesmith:
                    SetFormatWhitesmith(IndentText, ExpandEmpty, BlankBeforeColon, BlankBeforeComma);
                    break;
                case JSOutputFormat.OutputAllman:
                    SetFormatWhitesmith(IndentText, ExpandEmpty, BlankBeforeColon, BlankBeforeComma);
                    break;
            }
        }
        public void SetFormatCompact()
        {
            mIndentText = string.Empty;
            mArrayEmptyText = string.Empty;
            mObjectEmptyText = string.Empty;

            #region Function Based (FAST)
            mObjectEmptyFunc = FnEmptyObject;

            mObjectOpenFunc = FnObjectOpen;
            mObjectKeyFunc = FnNoOutput;
            mObjectColonFunc = FnColon;
            mObjectValueFunc = FnNoOutput;
            mObjectCommaFunc = FnComma;
            mObjectCloseFunc = FnWriteObjectClose;

            mArrayEmptyFunc = FnEmptyObject;

            mArrayOpenFunc = FnArrayOpen;
            mArrayValueFunc = FnNoOutput;
            mArrayCommaFunc = FnComma;
            mArrayCloseFunc = FnArrayClose;
            #endregion
        }
        public void SetFormatLinear(bool ExpandEmpty = false, bool BlankBeforeColon = false, bool BlankBeforeComma = false)
        {
            mIndentText = string.Empty;
            mArrayEmptyText = ExpandEmpty ? " " : String.Empty;
            mObjectEmptyText = ExpandEmpty ? " " : String.Empty;

            #region Function Based Formatters

            mObjectEmptyFunc = ExpandEmpty ? FnEmptyObjectExpanded : FnEmptyObject;

            mObjectOpenFunc = FnObjectOpen;
            mObjectKeyFunc = FnNoOutput;
            mObjectColonFunc = BlankBeforeColon ? FnSpaceColon : FnColon;
            mObjectValueFunc = FnNoOutput;
            mObjectCommaFunc = BlankBeforeComma ? FnSpaceComma : FnComma;
            mObjectCloseFunc = FnWriteObjectClose;

            mArrayEmptyFunc = ExpandEmpty ? FnEmptyArrayExpanded : FnEmptyArray;

            mArrayOpenFunc = FnArrayOpen;
            mArrayValueFunc = FnNoOutput;
            mArrayCommaFunc = BlankBeforeComma ? FnSpaceComma : FnComma;
            mArrayCloseFunc = FnArrayClose;

            #endregion
        }
        public void SetFormatKNR(string IndentText = "\t", bool ExpandEmpty = false, bool IndentClosure = false, bool BlankBeforeColon = false, bool BlankBeforeComma = false)
        {
            mIndentText = IndentText;
            mArrayEmptyText = ExpandEmpty ? " " : string.Empty;
            mObjectEmptyText = ExpandEmpty ? " " : string.Empty;
            
            #region Function Based Formatters

            mObjectEmptyFunc = ExpandEmpty ? FnEmptyObjectExpanded : FnEmptyObject;
            mObjectOpenFunc = FnObjectOpenLinefeedIndent;
            mObjectKeyFunc = FnNoOutput;
            mObjectColonFunc = BlankBeforeColon ? FnSpaceColonSpace : FnColonSpace;
            mObjectValueFunc = FnNoOutput;
            mObjectCommaFunc = BlankBeforeComma ? FnSpaceCommaLinefeedIndent: FnCommaLinefeedIndent;
            mObjectCloseFunc = IndentClosure ? FnWriteObjectLinefeedIndentClose : FnWriteObjectLinefeedOutdentClose;

            mArrayEmptyFunc = ExpandEmpty ? FnEmptyArrayExpanded : FnEmptyArray;

            mArrayOpenFunc = delegate (JSOutputContext context) {

                /*
                ** This code causes some text editors to improperly
                ** expand/collapse code indented blocks
                JSArray item = (JSArray)context.PeekObject();
                if (item.Count > 0 && item[0].IsContainer && item[0].Count > 0)
                {
                    context.Write("[ ");
                    return;
                }
                */

                FnArrayOpenLinefeedIndent(context);
            };
            mArrayValueFunc = FnNoOutput;
            mArrayCommaFunc = delegate (JSOutputContext context) {

                /*
                ** This code causes some text editors to improperly
                ** expand/collapse code indented blocks
                /*
                JSArray item = (JSArray)context.PeekObject();
                if (item.Count > 0 && item[0].IsContainer && item[0].Count > 0)
                {
                    context.Write(BlankBeforeComma ? " , " : ", ");
                    return;
                }
                */

                if (BlankBeforeComma)
                    context.WriteTextLinefeedIndent(" ,");
                else
                    context.WriteTextLinefeedIndent(',');
            };

            mArrayCloseFunc = IndentClosure ? FnArrayLinefeedIndentClose : FnArrayLinefeedOutdentClose;

            #endregion
       }
        public void SetFormatAllman(string IndentText = "\t", bool ExpandEmpty = false, bool BlankBeforeColon = false, bool BlankBeforeComma = false)
        {
            mIndentText = IndentText;
            mArrayEmptyText = ExpandEmpty ? " " : String.Empty;
            mObjectEmptyText = ExpandEmpty ? " " : String.Empty;

            #region Function Based Formatters
            mObjectEmptyFunc = ExpandEmpty ? FnEmptyObjectExpanded : FnEmptyObject;
            mObjectOpenFunc = delegate (JSOutputContext context) {
                if (context.PeekObject().Parent == null && context.IsTopLevel)
                {
                    context.Write('{');
                    return;
                }
                context.WriteLinefeedOutdent('{');
            };
            mObjectKeyFunc = FnLinefeedIndent;
            mObjectColonFunc = BlankBeforeColon ? FnSpaceColonSpace : FnColonSpace;
            mObjectValueFunc = FnNoOutput;
            mObjectCommaFunc = BlankBeforeComma ? FnSpaceComma : FnComma;
            mObjectCloseFunc = delegate (JSOutputContext context) { context.WriteLinefeedOutdent('}'); };

            mArrayEmptyFunc = ExpandEmpty ? FnEmptyArrayExpanded : FnEmptyArray;
            mArrayOpenFunc = delegate (JSOutputContext context) {
                if (context.PeekObject().Parent == null && context.IsTopLevel)
                {
                    context.Write('[');
                    return;
                }
                context.WriteLinefeedOutdent('[');
            };
            mArrayValueFunc = FnLinefeedIndent;
            mArrayCommaFunc = BlankBeforeComma ? FnSpaceComma : FnComma;
            mArrayCloseFunc = delegate (JSOutputContext context) { context.WriteLinefeedOutdent(']'); };
            #endregion
        }
        public void SetFormatWhitesmith(string IndentText = "\t", bool ExpandEmpty = false, bool BlankBeforeColon = false, bool BlankBeforeComma = false)
        {
            mIndentText = IndentText;
            mArrayEmptyText = ExpandEmpty ? " " : String.Empty;
            mObjectEmptyText = ExpandEmpty ? " " : String.Empty;

            #region Function Based Formatters
            mObjectEmptyFunc = ExpandEmpty ? FnEmptyObjectExpanded : FnEmptyObject;
            mObjectOpenFunc = delegate (JSOutputContext context) {
                if (context.PeekObject().Parent == null && context.IsTopLevel)
                {
                    context.WriteArrayOpen();
                    return;
                }
                context.WriteLinefeedIndent('{');
            };
            mObjectKeyFunc = FnLinefeedIndent;
            mObjectColonFunc = BlankBeforeColon ? FnSpaceColonSpace : FnColonSpace;
            mObjectValueFunc = FnNoOutput;
            mObjectCommaFunc = BlankBeforeComma ? FnSpaceComma : FnComma;
            mObjectCloseFunc = delegate (JSOutputContext context) 
                {
                    if (context.PeekObject().Parent == null && context.IsTopLevel)
                    {
                        context.WriteLinefeed('}');
                        return;
                    }
                    context.WriteLinefeedIndent('}'); 
                };

            mArrayEmptyFunc = ExpandEmpty ? FnEmptyArrayExpanded : FnEmptyArray;
            mArrayOpenFunc = delegate (JSOutputContext context) {
                if (context.PeekObject().Parent == null && context.IsTopLevel)
                {
                    context.WriteArrayOpen();
                    return;
                }
                context.WriteLinefeedIndent('[');
            };
            mArrayValueFunc = FnLinefeedIndent;
            mArrayCommaFunc = BlankBeforeComma ? FnSpaceComma : FnComma;
            mArrayCloseFunc = delegate (JSOutputContext context)
            {
                if (context.PeekObject().Parent == null && context.IsTopLevel)
                {
                    context.WriteLinefeed('}');
                    return;
                    }
                context.WriteLinefeedIndent(']');
            };
            #endregion
        }

        #endregion
    }

}
