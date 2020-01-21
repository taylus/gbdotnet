using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace GBDotNet.Core
{
    /// <summary>
    /// Implements a region of RAM as an addressable byte array.
    /// </summary>
    public class Memory : IMemory
    {
        private const int size = ushort.MaxValue + 1;
        protected readonly byte[] data;

        public Memory(params byte[] bytes)
        {
            Array.Resize(ref bytes, size);
            data = bytes;
        }

        public Memory(string path)
        {
            data = File.ReadAllBytes(path);
        }

        public static Memory FromFile(string path)
        {
            return new Memory(path);
        }

        public virtual byte this[int i]
        {
            get => data[i];
            set => data[i] = value;
        }

        /// <summary>
        /// Prints the first N bytes of memory to the console.
        /// </summary>
        public void HexDump(int bytesPerLine = 16, int? stopAfterBytes = null)
        {
            HexDump(data, bytesPerLine, stopAfterBytes);
        }

        /// <summary>
        /// Prints the first N bytes of the given buffer to the console.
        /// </summary>
        private static void HexDump(byte[] bytes, int bytesPerLine = 16, int? stopAfterBytes = null)
        {
            int length = stopAfterBytes ?? bytes.Length;
            for (int i = 0; i < length; i++)
            {
                Console.Write("{0:x2} ", bytes[i]);
                if (i % bytesPerLine == bytesPerLine - 1) Console.WriteLine();
            }
        }

        public virtual IEnumerator<byte> GetEnumerator()
        {
            return ((IEnumerable<byte>)data).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<byte>)data).GetEnumerator();
        }
    }
}
