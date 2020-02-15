using System;
using System.IO;

namespace GBDotNet.Core
{
    /// <summary>
    /// Used for Blargg's test ROMs: prints serial port bytes to a stream
    /// (by default, the console/stdout).
    /// </summary>
    /// <remarks>
    /// Game Link Cable... Game Link Console... get it? Haha.
    /// </remarks>
    public class GameLinkConsole
    {
        private readonly Stream outputStream;

        public GameLinkConsole() : this(Console.OpenStandardOutput())
        {

        }

        public GameLinkConsole(Stream outputStream)
        {
            this.outputStream = outputStream;
        }

        public void Print(byte b)
        {
            outputStream.WriteByte(b);
        }
    }
}
