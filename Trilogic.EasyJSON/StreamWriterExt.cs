using System.IO;

namespace Trilogic.EasyJSON
{
    #region StreamWriter Extensions
    internal static class StreamWriterExt
    {
        internal static void WriteWithEscapes(this StreamWriter writer, string source)
        {
            JSTools.WriteWithEscapes(writer, source);
        }

    }
    #endregion
}
