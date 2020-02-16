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
        public Stream OutputStream { get; private set; }
        public bool HasFreshUpdate { get; set; }    //so tests know when to check the output stream

        public GameLinkConsole() : this(Console.OpenStandardOutput())
        {

        }

        public GameLinkConsole(Stream outputStream)
        {
            OutputStream = outputStream;
        }

        public void Print(byte b)
        {
            OutputStream.WriteByte(b);
            HasFreshUpdate = true;
        }
    }
}
