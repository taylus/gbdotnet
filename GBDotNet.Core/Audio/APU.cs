using System.Collections.Generic;

namespace GBDotNet.Core
{
    //https://emudev.de/gameboy-emulator/bleeding-ears-time-to-add-audio/
    public class APU
    {
        private const int samplesToGenerateBeforePlaying = 100;
        private readonly IList<float> channel1Buffer = new List<float>(samplesToGenerateBeforePlaying);
        private readonly IList<float> channel2Buffer = new List<float>(samplesToGenerateBeforePlaying);
        private readonly IList<float> channel3Buffer = new List<float>(samplesToGenerateBeforePlaying);
        private readonly IList<float> channel4Buffer = new List<float>(samplesToGenerateBeforePlaying);
        private readonly IList<float> mixedBuffer = new List<float>(samplesToGenerateBeforePlaying);

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

            if (channel1Buffer.Count >= samplesToGenerateBeforePlaying &&
                channel2Buffer.Count >= samplesToGenerateBeforePlaying &&
                channel3Buffer.Count >= samplesToGenerateBeforePlaying &&
                channel4Buffer.Count >= samplesToGenerateBeforePlaying)
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
                result += channel1Buffer[i];
                result += channel2Buffer[i];
                result += channel3Buffer[i];
                result += channel4Buffer[i];
                mixedBuffer[i] = result;
            }
        }
    }
}
