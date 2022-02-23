using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trilogic.EasyJSON
{
    public class JSException : Exception
    {
        public JSException(string message) : base(message)
        {
        }
    }
}
