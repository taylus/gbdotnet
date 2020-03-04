using System;

namespace GBDotNet.Core
{
    public class Emulator
    {
        public CPU CPU { get; private set; }
        public PPU PPU { get; private set; }
        public Joypad Joypad { get; private set; }

        public Emulator(CPU cpu, PPU ppu, Joypad joypad)
        {
            CPU = cpu;
            PPU = ppu;
            Joypad = joypad;
        }

        public void Tick()
        {
            CPU.Tick();
            PPU.Tick(CPU.CyclesLastTick);
        }

        public void Restart(bool useBootRom)
        {
            if (useBootRom) CPU.Reset();
            else CPU.BootWithoutBootRom();
            PPU.Boot();
        }

        public double CalculateAverageFramesPerSecond(TimeSpan elapsedTimeSinceBoot)
        {
            return PPU.RenderedFrameCount / elapsedTimeSinceBoot.TotalSeconds;
        }

        public override string ToString() => $"{CPU} {PPU} {CPU.Memory}".Trim();
    }
}
