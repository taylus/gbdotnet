using System;

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
        public const int ScreenWidthInPixels = 160;
        public const int ScreenHeightInPixels = 144;

        private PPUMode CurrentMode
        {
            get => Registers.LCDStatus.ModeFlag;
            set => Registers.LCDStatus.ModeFlag = value;
        }

        internal byte CurrentLine
        {
            get => Registers.CurrentScanline;
            set => Registers.CurrentScanline = value;
        }

        private byte[] screenPixels = new byte[ScreenWidthInPixels * ScreenHeightInPixels];
        private int cycleCounter;

        public PPURegisters Registers { get; private set; }
        public IMemory VideoMemory { get; private set; } //VRAM
        public IMemory ObjectAttributeMemory { get; private set; }  //OAM
        public TileSet TileSet { get => new TileSet(VideoMemory); }

        public PPU(PPURegisters registers, IMemory vram, IMemory oam)
        {
            Registers = registers;
            VideoMemory = vram;
            ObjectAttributeMemory = oam;
        }

        public void Tick(int elapsedCycles)
        {
            cycleCounter += elapsedCycles;
            if (CurrentMode == PPUMode.HBlank) HBlank();
            else if (CurrentMode == PPUMode.VBlank) VBlank();
            else if (CurrentMode == PPUMode.OamScan) OamScan();
            else if (CurrentMode == PPUMode.HDraw) HDraw();
        }

        private void HBlank()
        {
            //wait 204 cycles and then draw the screen on the last line
            if (cycleCounter >= 204)
            {
                cycleCounter = 0;
                CurrentLine++;

                if (CurrentLine == 143)
                {
                    CurrentMode = PPUMode.VBlank;
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
                CurrentLine++;

                if (CurrentLine > 153)
                {
                    CurrentMode = PPUMode.OamScan;
                    CurrentLine = 0;
                }
            }
        }

        private void OamScan()
        {
            //wait 80 cycles then advance to HDraw
            if (cycleCounter >= 80)
            {
                cycleCounter = 0;
                CurrentMode = PPUMode.HDraw;
            }
        }

        private void HDraw()
        {
            //draw a scanline every 172 cycles
            if (cycleCounter >= 172)
            {
                cycleCounter = 0;
                CurrentMode = PPUMode.HBlank;
                RenderScanline();
            }
        }

        internal byte[] RenderSprites(TileSet tileset)
        {
            //TODO: make and move this into a class representing all 40 sprites in OAM?
            var spriteLayer = new byte[ScreenWidthInPixels * ScreenHeightInPixels];
            const int oamSize = Sprite.BytesPerSprite * Sprite.TotalSprites;
            for (int i = 0; i < oamSize; i += Sprite.BytesPerSprite)
            {
                var sprite = new Sprite(positionY: ObjectAttributeMemory[i],
                    positionX: ObjectAttributeMemory[i + 1],
                    tileNumber: ObjectAttributeMemory[i + 2],
                    attributes: ObjectAttributeMemory[i + 3],
                    Registers);

                if (!sprite.Visible) continue;

                //TODO: sprite priority logic, see: http://bgb.bircd.org/pandocs.htm#vramspriteattributetableoam
                sprite.Render(tileset, ref spriteLayer);
            }

            return spriteLayer;
        }

        internal byte[] RenderBackgroundMap(TileSet tileset)
        {
            var tilemap = new TileMap(Registers, tileset, VideoMemory);
            return tilemap.Render();
        }

        internal byte[] RenderTileSet()
        {
            var tileset = new TileSet(VideoMemory);
            return tileset.Render();
        }

        internal byte[] RenderWindow()
        {
            throw new NotImplementedException();
        }

        internal byte[] RenderScanline()
        {
            //cache and update these as needed instead of new-ing up every scanline?
            var tileSet = new TileSet(VideoMemory);
            var tileMap = new TileMap(Registers, tileSet, VideoMemory);

            var tileMapY = (byte)(CurrentLine + Registers.ScrollY);
            for (int x = 0; x < ScreenWidthInPixels; x++)
            {
                var tileMapX = (byte)(x + Registers.ScrollX);
                screenPixels[CurrentLine * ScreenWidthInPixels + x] = tileMap.GetPixelAt(tileMapX, tileMapY);
                DrawSpritesOntoScanline(tileSet, ref screenPixels, x);
                //TODO: render window
            }

            return screenPixels;
        }

        private void DrawSpritesOntoScanline(TileSet tileset, ref byte[] screenPixels, int x)
        {
            //TODO: move this into a class representing all sprites in OAM which has
            //      methods to render all sprites (for debugging) + a single scanline?
            const int oamSize = Sprite.BytesPerSprite * Sprite.TotalSprites;
            for (int i = 0; i < oamSize; i += Sprite.BytesPerSprite)
            {
                var sprite = new Sprite(positionY: ObjectAttributeMemory[i],
                    positionX: ObjectAttributeMemory[i + 1],
                    tileNumber: ObjectAttributeMemory[i + 2],
                    attributes: ObjectAttributeMemory[i + 3],
                    Registers);

                if (!sprite.OverlapsCoordinates(x, y: CurrentLine)) continue;

                //TODO: sprite priority logic, see: http://bgb.bircd.org/pandocs.htm#vramspriteattributetableoam
                //TODO: 10 sprites per scanline limit?
                byte? spritePixel = sprite.GetPixel(tileset, x, y: CurrentLine);
                if (!spritePixel.HasValue) continue;    //transparency
                screenPixels[CurrentLine * ScreenWidthInPixels + x] = spritePixel.Value;
            }
        }

        internal byte[] RenderScreen()
        {
            return screenPixels;
        }

        internal byte[] ForceRenderScreen()
        {
            for (int i = 0; i < ScreenHeightInPixels; i++)
            {
                RenderScanline();
                CurrentLine++;
            }
            return RenderScreen();
        }
    }
}
