using System;

namespace GBDotNet.Core
{
    /// <summary>
    /// Represents sound channels 1 and 2 of the Game Boy, which play oscillating pulse/square waves.
    /// </summary>
    /// <see cref="http://bgb.bircd.org/pandocs.htm#soundchannel1tonesweep"/>
    /// <see cref="http://bgb.bircd.org/pandocs.htm#soundchannel2tone"/>
    public class PulseChannel : SoundChannel
    {
        /// <summary>
        /// Sound channel on/off, stored in bit 7 of register NRx4.
        /// </summary>
        public override bool Enabled => channelNumber == 1 ? apu.Registers.NR14.IsBitSet(7) : apu.Registers.NR24.IsBitSet(7);

        public override int Length { get => throw new NotImplementedException(); }
        public override bool LengthEnabled { get => throw new NotImplementedException(); }

        /// <summary>
        /// Current volume of the channel expressed as a 4 bit value.
        /// A value of 0 means silence. Stored in bits 7-4 of NRx2.
        /// </summary>
        public override byte Volume
        {
            get
            {
                if (channelNumber == 1) return (byte)((apu.Registers.NR12 & 0b1111_0000) >> 4);
                return (byte)((apu.Registers.NR22 & 0b1111_0000) >> 4);
            }
        }

        /// <summary>
        /// How fast the pulse waves are oscillating.
        /// Higher frequency = higher pitched sound.
        /// </summary>
        /// <remarks>
        /// Stored in bits 2-0 of NRx4 + all bits in NRx3
        /// (making it an 11 bit number ranging 0-2047).
        /// </remarks>
        public int Frequency
        {
            get
            {
                if (channelNumber == 1) return ((apu.Registers.NR14 & 0b0000_0111) << 8) | (apu.Registers.NR13);
                return ((apu.Registers.NR24 & 0b0000_0111) << 8) | (apu.Registers.NR23);
            }
        }

        /// <summary>
        /// The current shape of the pulse wave (how wide are the pulses?) stored in bits 7-6 of NRx1.
        /// </summary>
        /// <see cref="dutyCyclePatterns"/>
        public int CurrentDutyCycle
        {
            get
            {
                if (channelNumber == 1) return (apu.Registers.NR11 & 0b1100_0000) >> 6;
                return (apu.Registers.NR21 & 0b1100_0000) >> 6;
            }
        }

        /// <summary>
        /// The current bit position in the current duty cycle
        /// (determines if the next sound sample should be a 1 or a 0).
        /// </summary>
        private int dutyCyclePosition;

        /// <summary>
        /// Is this channel number 1 or 2? (different capabilities and register usage)
        /// </summary>
        private readonly int channelNumber;

        /// <summary>
        /// Which bits to output as a sound sample to create the pulse waveform.
        /// </summary>
        /// <see cref="http://bgb.bircd.org/pandocs.htm#soundchannel2tone"/>
        private static readonly byte[] dutyCyclePatterns = new byte[]
        {
            0b00000001,     //12.5%
            0b10000001,     //25%
            0b10000111,     //50% (square wave)
            0b01111110      //75%
        };

        /// <summary>
        /// Initialized to (2048 - Frequency) * 4.
        /// Decremented each CPU cycle.
        /// When zero, reset back and push a sound sample.
        /// </summary>
        private int timer;

        private readonly APU apu;

        public PulseChannel(APU apu, int channelNumber)
        {
            this.apu = apu;
            this.channelNumber = channelNumber;
        }

        public byte Tick(int elapsedCycles)
        {
            if (timer > 0) timer -= elapsedCycles;
            if (timer <= 0)
            {
                ResetTimer();
                dutyCyclePosition++;
                if (dutyCyclePosition > 7) dutyCyclePosition = 0;
            }
            return Sample();
        }

        public byte Sample()
        {
            if (!Enabled) return 0;
            var pattern = dutyCyclePatterns[CurrentDutyCycle];
            var sampleBit = pattern.IsBitSet(7 - dutyCyclePosition);
            return sampleBit ? Volume : (byte)0;
        }

        private void ResetTimer() => timer = (2048 - Frequency) * 4;
    }
}
