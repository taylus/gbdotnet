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
        private int[,] backgroundMap = new int[256, 256];  //pixels, palette index numbers 0-3

        public const int TileWidthInPixels = 8;
        public const int TileHeightInPixels = 8;
        public const int ScreenWidthInTiles = 20;
        public const int ScreenHeightInTiles = 18;
        public const int BackgroundMapWidthInTiles = 32;
        public const int BackgroundMapHeightInTiles = 32;
        public const int ScreenWidthInPixels = ScreenWidthInTiles * TileWidthInPixels;
        public const int ScreenHeightInPixels = ScreenHeightInTiles * TileHeightInPixels;
        public const int BackgroundMapWidthInPixels = BackgroundMapWidthInTiles * TileWidthInPixels;
        public const int BackgroundMapHeightInPixels = BackgroundMapHeightInTiles * TileHeightInPixels;

        public PPURegisters Registers { get; private set; }
        public IMemory Memory { get; private set; }

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
                    RenderScreen();
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

        private void RenderScreen()
        {
            RenderBackgroundMap();
            //TODO: render window
            //TODO: render sprites
        }

        private void RenderBackgroundMap()
        {

        }
    }
}
