using System;
using System.Collections;
using System.Collections.Generic;

namespace GBDotNet.Core
{
    /// <summary>
    /// Implements a region of memory as an addressable byte array.
    /// </summary>
    public class Memory : IEnumerable<byte>
    {
        private const int size = ushort.MaxValue;
        private readonly byte[] memory;

        public Memory(params byte[] bytes)
        {
            Array.Resize(ref bytes, size);
            memory = bytes;
        }

        public byte this[int i]
        {
            get => memory[i];
            set => memory[i] = value;
        }

        /// <summary>
        /// Prints the first N bytes of memory to the console.
        /// </summary>
        public void HexDump(int bytesPerLine = 16, int? stopAfterBytes = null)
        {
            HexDump(memory, bytesPerLine, stopAfterBytes);
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

        public IEnumerator<byte> GetEnumerator()
        {
            return ((IEnumerable<byte>)memory).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<byte>)memory).GetEnumerator();
        }
    }
}
