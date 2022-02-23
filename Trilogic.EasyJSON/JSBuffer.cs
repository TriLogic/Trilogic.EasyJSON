using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trilogic.EasyJSON
{
    internal class JSBuffer
    {
        #region Class Members
        dynamic _buffer = string.Empty;
        int _index = 0;
        #endregion

        #region Constructors and Destructors
        public JSBuffer()
        {
            Reset(string.Empty);
        }
        public JSBuffer(string input)
        {
            Reset(input);
        }
        public JSBuffer(StringBuilder input)
        {
            Reset(input);
        }
        #endregion

        #region Reset
        public void Reset(string input)
        {
            _buffer = string.IsNullOrEmpty(input) ? string.Empty : input;
            _index = 0;
        }
        public void Reset(StringBuilder input)
        {
            _buffer = input ?? new StringBuilder();
            _index = 0;
        }
        #endregion

        public int Index
        {
            get => _index;
        }

        public dynamic Buffer
        {
            get => _buffer;
        }

        public char PeekC()
        {
            return _buffer[_index];
        }

        public int Length
        {
            get => _buffer.Length;
        }

        public char GetC()
        {

            if (_index >= _buffer.Length)
                throw new JSException("Parse buffer overrun");

            char c = _buffer[_index];
            _index++;
            return c;
        }

        public void UngetC(int count = 1)
        {
            if (_index - count < 0)
                throw new JSException("Parse buffer underflow");
            _index -= count; ;
        }

        public bool EOF
        {
            get => _index >= _buffer.Length;
        }
    }
}
