namespace GBDotNet.Core
{
    /// <summary>
    /// Implement's the Game Boy's Picture Processing Unit, which produces video
    /// based off data in VRAM, OAM, and various memory-mapped I/O registers. 
    /// </summary>
    /// <remarks>
    /// The PPU is a state machine which spends a very specific number of cycles
    /// in each state. Timing w/ the CPU is implemented by counting cycles.
    /// </remarks>
    /// <see cref="https://mgba-emu.github.io/gbdoc/#ppu"/>
    public class PPU
    {
        private PPUMode currentMode
        {
            get => Registers.LCDStatus.ModeFlag;
            set => Registers.LCDStatus.ModeFlag = value;
        }
        private int currentLineBeingDrawn;
        private int cyclesSinceLastModeChange;

        public PPURegisters Registers { get; private set; }
        public IMemory Memory { get; private set; }

        public PPU(IMemory memory)
        {
            Memory = memory;
        }

        public void Tick(int elapsedCycles)
        {
            cyclesSinceLastModeChange += elapsedCycles;

            //TODO: implement per:
            //http://bgb.bircd.org/pandocs.htm#lcdstatusregister
            //http://imrannazar.com/GameBoy-Emulation-in-JavaScript:-GPU-Timings

            if (currentMode == PPUMode.HBlank)
            {

            }
            else if (currentMode == PPUMode.VBlank)
            {

            }
            else if (currentMode == PPUMode.OamScan)
            {

            }
            else if (currentMode == PPUMode.HDraw)
            {

            }
        }
    }
}
