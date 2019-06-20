using System;

namespace GBDotNet.Core
{
    /// <summary>
    /// Represents the flag bits of the F register, indicating results from
    /// the most recent instruction which affects flags. E.g. the zero bit
    /// being set to 1 means the most recent instruction resulted in zero.
    /// </summary>
    /// <see cref="http://bgb.bircd.org/pandocs.htm#cpuregistersandflags"/>
    [Flags]
    public enum Flags
    {
        /// <summary>
        /// Indicates that the result of an operation was zero.
        /// Often used for conditional jumps. Also known as the "Z" or "ZF" flag.
        /// </summary>
        Zero = 128,

        /// <summary>
        /// Indicates that an operation was addition (if 0) or subtraction (if 1).
        /// Also known as the "N" flag.
        /// </summary>
        AddSubtract = 64,

        /// <summary>
        /// Indicates that a carry occurred from the lower 4 bits of the result.
        /// Also known as the "H" flag.
        /// </summary>
        /// <see cref="https://robdor.com/2016/08/10/gameboy-emulator-half-carry-flag/"/>
        HalfCarry = 32,

        /// <summary>
        /// Indicates that a carry occurred from the upper 8 bits of the result.
        /// Often used for conditional jumps. Also known as the "C" or "CY" flag.
        /// </summary>
        Carry = 16
    }
}
