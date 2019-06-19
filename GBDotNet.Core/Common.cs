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
        /// Turns the given two bytes into a little-endian 16-bit number.
        /// (The second byte makes up the most significant bits of the new number)
        /// E.g. $cd, $ab => $abcd
        /// </summary>
        public static ushort ToLittleEndian(byte byte1, byte byte2)
        {
            return ToBigEndian(byte2, byte1);
        }
    }
}
