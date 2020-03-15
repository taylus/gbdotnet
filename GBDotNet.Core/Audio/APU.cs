using System.Collections.Generic;

namespace GBDotNet.Core
{
    //https://emudev.de/gameboy-emulator/bleeding-ears-time-to-add-audio/
    public class APU
    {
        private const int samplesToGenerateBeforePlaying = 100;
        private readonly PulseChannel channel2 = new PulseChannel(samplesToGenerateBeforePlaying);
        private readonly IList<float> mixedBuffer = new List<float>(samplesToGenerateBeforePlaying);

        private const int sampleRate = 44100;   //Hz
        private int cyclesSinceLastSample;
        private const int cyclesPerSample = (int)(CPU.ClockSpeed / sampleRate);

        public SoundRegisters Registers { get; private set; }

        public APU(SoundRegisters registers)
        {
            Registers = registers;
        }

        public void Tick(int elapsedCycles)
        {
            channel2.Tick();
            SampleChannels(elapsedCycles);
        }

        private void SampleChannels(int elapsedCycles)
        {
            cyclesSinceLastSample += elapsedCycles;
            if (cyclesSinceLastSample >= cyclesPerSample)
            {
                cyclesSinceLastSample = 0;
                float sample = 0;
                sample += channel2.Sample();
                mixedBuffer.Add(sample);
            }
        }
    }
}
