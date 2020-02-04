using System;

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

        /// <summary>
        /// Returns true if the bit at the given position in the given value is 1.
        /// </summary>
        /// <param name="value">The value whose bits to test.</param>
        /// <param name="position">A bit numbered 0 to 7 where 0 is the least significant bit and 7 is the most significant bit.</param>
        public static bool IsBitSet(this byte value, int position)
        {
            if (position < 0 || position > 7) throw new ArgumentOutOfRangeException(nameof(position),
                "Bit position must be between 0 (least significant bit) and 7 (most significant bit).");
            return (value & (1 << position)) != 0;
        }

        /// <summary>
        /// Returns the given byte with the bit in the given position set to 1.
        /// </summary>
        /// <param name="value">The byte whose bit to set</param>
        /// <param name="position">A bit numbered 0 to 7 where 0 is the least significant bit and 7 is the most significant bit.</param>
        public static byte SetBit(this byte value, int position)
        {
            if (position < 0 || position > 7) throw new ArgumentOutOfRangeException(nameof(position),
                "Bit position must be between 0 (least significant bit) and 7 (most significant bit).");
            return (byte)(value | (1 << position));
        }

        /// <summary>
        /// Returns the given byte with the bit in the given position set to 0.
        /// </summary>
        /// <param name="value">The byte whose bit to set</param>
        /// <param name="position">A bit numbered 0 to 7 where 0 is the least significant bit and 7 is the most significant bit.</param>
        public static byte ClearBit(this byte value, int position)
        {
            if (position < 0 || position > 7) throw new ArgumentOutOfRangeException(nameof(position),
                "Bit position must be between 0 (least significant bit) and 7 (most significant bit).");
            return (byte)(value & ~(1 << position));
        }

        /// <summary>
        /// Sets the bit in the given position in the given byte to the given bool (true => 1, false => 0).
        /// </summary>
        /// <param name="value">The byte whose bit to set</param>
        /// <param name="position">A bit numbered 0 to 7 where 0 is the least significant bit and 7 is the most significant bit.</param>
        /// <param name="bit">If true, set the bit to 1, else set the bit to 0.</param>
        public static byte SetBitTo(this byte value, int position, bool bit)
        {
            if (bit) return value.SetBit(position);
            return value.ClearBit(position);
        }
    }
}
