using System.Linq;

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
        private int[,] screen = new int[256, 256];  //pixels, palette index numbers 0-3

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

        public PPU(IMemory memory)
        {
            Memory = memory;
        }

        public void Tick(int elapsedCycles)
        {
            cycleCounter += elapsedCycles;

            //TODO: implement per:
            //http://bgb.bircd.org/pandocs.htm#lcdstatusregister
            //http://imrannazar.com/GameBoy-Emulation-in-JavaScript:-GPU-Timings

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
                RenderScanline(currentLine);
            }
        }

        private void RenderScanline(int lineNumber)
        {
            // How to render a single line of background tiles on the screen:
            // - Determine which tilemap we're using based on bit 3 of the LCD control register.
            // - Find the tile index #s in the tilemap containing the line we're rendering based on lineNumber and the scroll registers.
            // 
            // Once we have the appropriate tile index numbers, we can look them up in the tileset to get their pixel data.
            // Each tile corresponds to 16 consecutive bytes in the address range $8000 - $97FF, where the tile # is either
            // an unsigned offset from $8000 or a signed offset from $9000 based on bit 4 of the LCD control register.
            // Each pair of bytes is a single row in the tile, and each bit is a column, where the first byte is all the
            // least significant bits of each pixel, and the second byte is all the most significant bits of each pixel.
            //
            // So, depending on the scroll registers, we may need to render up to 21 x 19 tiles, with only the bottom right
            // corner pixel of the first tile being rendered, corresponding to only the LSBs of its last two bytes.
            // 
            // - Find the x, y pixel offsets into the tiles to render based on lineNumber and the scroll registers.
            // - Read the appropriate bytes and bits and render them as numbers 0-3 representing pixels/palette colors.

            int bgTilemapBaseAddress = Registers.LCDControl.BackgroundTileMapBaseAddress;

            //determine where in the background map to start reading tile numbers
            var tileY = (byte)(lineNumber + Registers.ScrollY) / TileHeightInPixels;
            var tileX = Registers.ScrollX / TileWidthInPixels;
            var bgTileStartAddress = bgTilemapBaseAddress + tileX + (tileY * BackgroundMapWidthInTiles);

            int tileNumber = Memory[bgTileStartAddress];
            if (Registers.LCDControl.BackgroundAndWindowTileNumbersAreSigned && tileNumber < 128) tileNumber += 256;

            for (int x = 0; x < ScreenWidthInPixels; x++)
            {
                var pixelY = (byte)(currentLine + Registers.ScrollY) % TileHeightInPixels;
                var pixelX = Registers.ScrollX % TileWidthInPixels;
            }
        }

        private void RenderScreen()
        {

        }
    }
}
