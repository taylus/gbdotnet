namespace GBDotNet.Core
{
    /// <summary>
    /// Lowest common denominator for all four of the Game Boy's sound channels.
    /// </summary>
    public abstract class SoundChannel
    {
        /// <summary>
        /// Sound channel on/off.
        /// </summary>
        /// <remarks>
        /// Stored in bit 7 of register NRx4.
        /// </remarks>
        public abstract bool Enabled { get; }

        /// <summary>
        /// A measure of how long to keep playing this sound channel.
        /// Decremented periodically, and when zero, the channel is silenced.
        /// </summary>
        /// <remarks>
        /// Stored in varying bits of NRx1:
        /// Channels 1, 2, and 4 use bits 5-0 or NRx1.
        /// Channel 3 uses all 8 bits of NR31.
        /// </remarks>
        public abstract int Length { get; }

        /// <summary>
        /// Length counter enable: when false, a length of zero does not silence the channel.
        /// </summary>
        /// <remarks>
        /// Stored in bit 6 of register NRx4.
        /// </remarks>
        public abstract bool LengthEnabled { get; }

        /// <summary>
        /// Current volume of the channel expressed as a 4 bit value (except 
        /// the wave channel which is only 2 bits). A value of 0 means silence.
        /// </summary>
        /// <remarks>
        /// Stored in varying bits of NRx2.
        /// </remarks>
        public abstract byte Volume { get; }
    }
}
