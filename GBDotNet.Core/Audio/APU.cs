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

        public SoundRegisters Registers { get; private set; }

        public APU(SoundRegisters registers)
        {
            Registers = registers;
        }

        public void Tick(int elapsedCycles)
        {
            TickChannel1(elapsedCycles);
            TickChannel2(elapsedCycles);
            TickChannel3(elapsedCycles);
            TickChannel4(elapsedCycles);

            if (channel2.SampleBuffer.Count >= samplesToGenerateBeforePlaying)
            {
                Mix();
            }
        }

        private void TickChannel1(int elapsedCycles)
        {
            //TODO: do pulse channel things based off registers
        }

        private void TickChannel2(int elapsedCycles)
        {
            //TODO: do pulse channel things based off registers
        }

        private void TickChannel3(int elapsedCycles)
        {
            //TODO: do wave channel things based off registers
        }

        private void TickChannel4(int elapsedCycles)
        {
            //TODO: do noise channel things based off registers
        }

        private void Mix()
        {
            for (int i = 0; i < samplesToGenerateBeforePlaying; i++)
            {
                float result = 0;
                result += channel1.SampleBuffer[i];
                result += channel2.SampleBuffer[i];
                mixedBuffer[i] = result;
            }
        }
    }
}
