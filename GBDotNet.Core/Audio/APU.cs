using System.Collections.Generic;

namespace GBDotNet.Core
{
    //https://emudev.de/gameboy-emulator/bleeding-ears-time-to-add-audio/
    public class APU
    {
        private const int samplesToGenerateBeforePlaying = 100;
        private readonly PulseChannel channel1 = new PulseChannel(samplesToGenerateBeforePlaying);
        private readonly PulseChannel channel2 = new PulseChannel(samplesToGenerateBeforePlaying);
        //TODO: wave channel 3, noise channel 4

        private readonly IList<float> mixedBuffer = new List<float>(samplesToGenerateBeforePlaying);
        //TODO: global sound control registers http://bgb.bircd.org/pandocs.htm#soundcontrolregisters

        private int cyclesSinceLastFrameSequencerStep;
        private const int cyclesPerFrameSequencerStep = 8192;   //512 Hz (4194304 Hz / 512 Hz = 8192 cycles)
        private int frameSequencerStep;                         //0-7, different things update on different steps

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
            //channel1.Tick();
            channel2.Tick();

            TickFrameSequencer(elapsedCycles);
            SampleChannels(elapsedCycles);
        }

        private void TickFrameSequencer(int elapsedCycles)
        {
            cyclesSinceLastFrameSequencerStep += elapsedCycles;
            if (cyclesSinceLastFrameSequencerStep >= cyclesPerFrameSequencerStep)
            {
                cyclesSinceLastFrameSequencerStep = 0;
                StepFrameSequencer();
            }
        }

        private void StepFrameSequencer()
        {
            if (frameSequencerStep % 2 == 0)
            {
                //TODO: tick length counter every other step
            }

            if (frameSequencerStep == 7)
            {
                //TODO: tick volume envelope every 7th step
            }

            if (frameSequencerStep == 2 || frameSequencerStep == 6)
            {
                //TODO: tick sweep every 2nd and 6th steps
            }

            frameSequencerStep++;
            if (frameSequencerStep > 7) frameSequencerStep = 0;
        }

        private void SampleChannels(int elapsedCycles)
        {
            cyclesSinceLastSample += elapsedCycles;
            if (cyclesSinceLastSample >= cyclesPerSample)
            {
                cyclesSinceLastSample = 0;
                float sample = 0;
                //sample += channel1.Sample();
                sample += channel2.Sample();
                mixedBuffer.Add(sample);
            }
        }
    }
}
