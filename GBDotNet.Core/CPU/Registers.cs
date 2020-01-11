using System;

namespace GBDotNet.Core
{
    /// <summary>
    /// Implements the CPU's registers and the ability to "combine" some of the
    /// 8-bit registers into wider 16-bit registers.
    /// </summary>
    public class Registers
    {
        public byte A { get; set; }
        public byte F { get; set; }
        public byte B { get; set; }
        public byte C { get; set; }
        public byte D { get; set; }
        public byte E { get; set; }
        public byte H { get; set; }
        public byte L { get; set; }
        public ushort SP { get; set; }
        public ushort PC { get; set; }

        public ushort AF
        {
            get => Common.ToBigEndian(A, F);
            set
            {
                A = (byte)(value >> 8);
                F = (byte)value;
            }
        }

        public ushort BC
        {
            get => Common.ToBigEndian(B, C);
            set
            {
                B = (byte)(value >> 8);
                C = (byte)value;
            }
        }

        public ushort DE
        {
            get => Common.ToBigEndian(D, E);
            set
            {
                D = (byte)(value >> 8);
                E = (byte)value;
            }
        }

        public ushort HL
        {
            get => Common.ToBigEndian(H, L);
            set
            {
                H = (byte)(value >> 8);
                L = (byte)value;
            }
        }

        private Flags Flags => (Flags)F;

        public void SetFlag(Flags flag)
        {
            F |= (byte)flag;
        }

        /// <summary>
        /// Sets the given flag if the given condition is true.
        /// </summary>
        /// <param name="flag">The flag to set (make true).</param>
        /// <param name="condition">The condition under which the flag should be set.</param>
        public void SetFlagTo(Flags flag, bool condition)
        {
            if (condition)
            {
                SetFlag(flag);
            }
            else
            {
                ClearFlag(flag);
            }
        }

        public void ClearFlag(Flags flag)
        {
            F &= (byte)~flag;
        }

        public bool HasFlag(Flags flag)
        {
            return Flags.HasFlag(flag);
        }

        public override string ToString()
        {
            return
                $"AF: {AF:x4}{Environment.NewLine}" +
                $"BC: {BC:x4}{Environment.NewLine}" +
                $"DE: {DE:x4}{Environment.NewLine}" +
                $"HL: {HL:x4}{Environment.NewLine}" +
                $"SP: {SP:x4}{Environment.NewLine}" +
                $"PC: {PC:x4}";
        }
    }
}
