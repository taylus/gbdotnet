using System;

namespace GBDotNet.Core
{
    public class Memory
    {
        private const int size = ushort.MaxValue;
        private byte[] memory;

        public Memory(params byte[] bytes)
        {
            Array.Resize(ref bytes, size);
            memory = bytes;
        }

        public byte this[ushort i]
        {
            get => memory[i];
            set => memory[i] = value;
        }

        /// <summary>
        /// Prints the first N bytes of the ROM to the console.
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
    }
}
