using System;

namespace GBDotNet.Core
{
    public class APU
    {
        private readonly PulseChannel channel1;
        private readonly PulseChannel channel2;
        public byte[] SampleBuffer { get; } = new byte[3000];
        private int nextSampleIndex = 0;

        public const int PlaybackSampleRate = 44100;   //Hz
        private int cyclesSinceLastSample;
        private const int cyclesPerSample = (int)(CPU.ClockSpeed / PlaybackSampleRate);

        public SoundRegisters Registers { get; private set; }

        public APU(SoundRegisters registers)
        {
            channel1 = new PulseChannel(this, 1);
            channel2 = new PulseChannel(this, 2);
            Registers = registers;
        }

        public void Tick(int elapsedCycles)
        {
            byte sample = channel1.Tick(elapsedCycles);
            sample += channel2.Tick(elapsedCycles);

            cyclesSinceLastSample += elapsedCycles;
            if (cyclesSinceLastSample >= cyclesPerSample)
            {
                cyclesSinceLastSample = 0;
                SampleBuffer[nextSampleIndex++] = sample;
                if (nextSampleIndex >= SampleBuffer.Length) nextSampleIndex = 0;
            }
        }

        public void RestartSampleBuffer() => nextSampleIndex = 0;
    }
}
