namespace GBDotNet.Core
{
    public static class Common
    {
        /// <summary>
        /// Turns the given two bytes into a big-endian 16-bit number.
        /// (The first byte makes up the most significant bits of the new number)
        /// E.g. $ab, $cd => $abcd
        /// </summary>
        public static ushort ToBigEndian(byte byte1, byte byte2)
        {
            return (ushort)((byte1 << 8) | byte2);
        }

        /// <summary>
        /// Swaps the given bytes to turn it from a little-endian (low byte first)
        /// into a big-endian number (high byte first).
        /// E.g. $cd, $ab => $abcd
        /// </summary>
        public static ushort FromLittleEndian(byte byte1, byte byte2)
        {
            return ToBigEndian(byte2, byte1);
        }

        /// <summary>
        /// Returns true if the given bits are set to 1 in this byte.
        /// </summary>
        public static bool AreBitsSet(this byte value, byte bits)
        {
            return (value & bits) == bits;
        }

        /// <summary>
        /// Returns true if the given flags are set in this byte.
        /// </summary>
        public static bool AreBitsSet(this byte value, Flags flags)
        {
            return AreBitsSet(value, (byte)flags);
        }
    }
}
