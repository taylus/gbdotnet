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
        public const int Size = ushort.MaxValue + 1;
        public byte[] Data { get; protected set; }

        public Memory(params byte[] bytes)
        {
            Array.Resize(ref bytes, Size);
            Data = bytes;
        }

        public Memory(ArraySegment<byte> bytes)
        {
            Data = bytes.ToArray();
        }

        public Memory(string path)
        {
            Data = File.ReadAllBytes(path);
        }

        public static Memory FromFile(string path)
        {
            return new Memory(path);
        }

        public virtual byte this[int i]
        {
            get => Data[i];
            set => Data[i] = value;
        }

        public void Reset()
        {

        }

        public void Tick(int elapsedCycles)
        {

        }

        /// <summary>
        /// Prints the first N bytes of memory to the console.
        /// </summary>
        public void HexDump(int bytesPerLine = 16, int? stopAfterBytes = null)
        {
            HexDump(Data, bytesPerLine, stopAfterBytes);
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
            return ((IEnumerable<byte>)Data).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<byte>)Data).GetEnumerator();
        }
    }
}
