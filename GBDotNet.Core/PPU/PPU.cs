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

        private byte currentLine
        {
            get => Registers.CurrentScanline;
            set => Registers.CurrentScanline = value;
        }

        private int cycleCounter;

        public PPURegisters Registers { get; private set; }
        public IMemory Memory { get; private set; } //VRAM
        public TileSet TileSet { get => new TileSet(Memory); }

        public PPU(PPURegisters registers, IMemory memory)
        {
            Registers = registers;
            Memory = memory;
        }

        public void Tick(int elapsedCycles)
        {
            cycleCounter += elapsedCycles;
            if (currentMode == PPUMode.HBlank) HBlank();
            else if (currentMode == PPUMode.VBlank) VBlank();
            else if (currentMode == PPUMode.OamScan) OamScan();
            else if (currentMode == PPUMode.HDraw) HDraw();
        }

        private void HBlank()
        {
            //wait 204 cycles and then draw the screen on the last line
            if (cycleCounter >= 204)
            {
                cycleCounter = 0;
                currentLine++;

                if (currentLine == 143)
                {
                    currentMode = PPUMode.VBlank;
                    //RenderScreen();
                }
            }
        }

        private void VBlank()
        {
            //wait 10 scanlines then restart at OAM scan
            if (cycleCounter >= 456)
            {
                cycleCounter = 0;
                currentLine++;

                if (currentLine > 153)
                {
                    currentMode = PPUMode.OamScan;
                    currentLine = 0;
                }
            }
        }

        private void OamScan()
        {
            //wait 80 cycles then advance to HDraw
            if (cycleCounter >= 80)
            {
                cycleCounter = 0;
                currentMode = PPUMode.HDraw;
            }
        }

        private void HDraw()
        {
            //draw a scanline every 172 cycles
            if (cycleCounter >= 172)
            {
                cycleCounter = 0;
                currentMode = PPUMode.HBlank;
                //RenderScanline(currentLine);
            }
        }

        internal byte[] RenderBackgroundMap(TileSet tileset)
        {
            var baseAddress = Registers.LCDControl.BackgroundTileMapBaseAddress;
            var tilemap = new TileMap(baseAddress, tileset, Memory);
            return tilemap.Render();
        }

        internal byte[] RenderTileSet()
        {
            var tileset = new TileSet(Memory);
            return tileset.Render();
        }
    }
}
