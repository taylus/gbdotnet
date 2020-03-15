using System.Collections.Generic;

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
        public bool Enabled { get; set; }

        /// <summary>
        /// A measure of how long to keep playing this sound channel.
        /// Decremented periodically, and when zero, the channel is silenced.
        /// </summary>
        /// <remarks>
        /// Stored in varying bits of NRx1:
        /// Channels 1, 2, and 4 use bits 5-0 or NRx1.
        /// Channel 3 uses all 8 bits of NR31.
        /// </remarks>
        public int Length { get; set; }

        /// <summary>
        /// Length counter enable: when false, a length of zero does not silence the channel.
        /// </summary>
        /// <remarks>
        /// Stored in bit 6 of register NRx4.
        /// </remarks>
        public bool LengthEnabled { get; set; }

        /// <summary>
        /// Current volume of the channel expressed as a 4 bit value (except 
        /// the wave channel which is only 2 bits). A value of 0 means silence.
        /// </summary>
        /// <remarks>
        /// Stored in varying bits of NRx2.
        /// </remarks>
        public int Volume { get; set; }
    }

    /// <summary>
    /// Represents sound channels 1 and 2 of the Game Boy, which play oscillating pulse/square waves.
    /// </summary>
    /// <see cref="http://bgb.bircd.org/pandocs.htm#soundchannel1tonesweep"/>
    /// <see cref="http://bgb.bircd.org/pandocs.htm#soundchannel2tone"/>
    public class PulseChannel : SoundChannel
    {
        /// <summary>
        /// How fast the pulse waves are oscillating.
        /// Higher frequency = higher pitched sound.
        /// </summary>
        /// <remarks>
        /// Stored in bits 2-0 of NRx4 + all bits in NRx3
        /// (making it an 11 bit number ranging 0-2047).
        /// </remarks>
        public int Frequency { get; set; }

        /// <summary>
        /// Calculate the actual frequency to play based on the Frequency bits.
        /// </summary>
        /// <see cref="http://bgb.bircd.org/pandocs.htm#soundchannel2tone"/>
        public int CalculateActualFrequency() => 131072 / (2048 - Frequency);

        /// <summary>
        /// The current shape of the pulse wave (how wide are the pulses?)
        /// </summary>
        /// <see cref="dutyCyclePatterns"/>
        public int CurrentDutyCycle { get; set; }

        /// <summary>
        /// The current bit position in the current duty cycle
        /// (determines if the next sound sample should be a 1 or a 0).
        /// </summary>
        private int dutyCyclePosition;

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

        public IList<float> SampleBuffer { get; private set; }

        public PulseChannel(int sampleBufferSize)
        {
            SampleBuffer = new List<float>(sampleBufferSize);
        }

        public void Tick()
        {
            if (timer > 0) timer--;
            if (timer == 0)
            {
                dutyCyclePosition++;
                if (dutyCyclePosition > 7) dutyCyclePosition = 0;
                //SampleBuffer.Add(Sample());
                ResetTimer();
            }
        }

        public int Sample()
        {
            if (!Enabled) return 0;
            var pattern = dutyCyclePatterns[CurrentDutyCycle];
            var sampleBit = pattern.IsBitSet(7 - dutyCyclePosition);
            return sampleBit ? Volume : 0;
        }

        private void ResetTimer() => timer = (2048 - Frequency) * 4;
    }
}
