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
        public MemoryBus MemoryBus { get; private set; }

        public IMemory VideoMemory
        { 
            get => MemoryBus.VideoMemory;
            set => MemoryBus.VideoMemory = value;
        } 

        public IMemory ObjectAttributeMemory
        { 
            get => MemoryBus.ObjectAttributeMemory;
            set => MemoryBus.ObjectAttributeMemory = value;
        }

        public TileSet TileSet { get => new TileSet(VideoMemory); }

        public PPU(PPURegisters registers, MemoryBus memoryBus)
        {
            Registers = registers;
            MemoryBus = memoryBus;
        }

        public PPU(byte[] memory)
        {
            if (memory.Length != Memory.Size)
                throw new ArgumentException($"Expected entire {Memory.Size} byte memory map, but got {memory.Length} bytes.", nameof(memory));

            Registers = new PPURegisters(new ArraySegment<byte>(memory, offset: 0xFF40, count: 12));
            MemoryBus = new MemoryBus(Registers);
            VideoMemory = new Memory(new ArraySegment<byte>(memory, offset: 0x8000, count: 8192));
            ObjectAttributeMemory = new Memory(new ArraySegment<byte>(memory, offset: 0xFE00, count: 160));
        }

        /// <summary>
        /// Initializes the PPU's internal state to what it would be immediately
        /// after executing the boot ROM.
        /// </summary>
        /// <see cref="https://gbdev.gg8.se/wiki/articles/Gameboy_Bootstrap_ROM"/>
        public void Boot()
        {
            Registers.LCDControl.Data = 0x91;
            Registers.LCDStatus.Data = 0x85;
            Registers.BackgroundPalette.Data = 0xFC;
            Registers.SpritePalette0.Data = 0xFF;
            Registers.SpritePalette1.Data = 0xFF;
        }

        public void Tick(int elapsedCycles)
        {
            if (!Registers.LCDControl.Enabled) return;
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

                if (CurrentLine == 144)
                {
                    CurrentMode = PPUMode.VBlank;
                    MemoryBus.InterruptFlags.VBlankInterruptRequested = true;
                    RenderScreen();
                }
                else
                {
                    CurrentMode = PPUMode.OamScan;
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

        public byte[] RenderSprites(TileSet tileset)
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

        public byte[] RenderBackgroundMap(TileSet tileset)
        {
            var bgMap = new BackgroundMap(Registers, tileset, VideoMemory);
            return bgMap.Render();
        }

        public byte[] RenderTileSet()
        {
            var tileset = new TileSet(VideoMemory);
            return tileset.Render();
        }

        public byte[] RenderWindow(TileSet tileset)
        {
            var window = new Window(Registers, tileset, VideoMemory);
            return window.Render();
        }

        public byte[] RenderScanline()
        {
            if (!Registers.LCDControl.Enabled)
            {
                RenderBlankLine();
                return screenPixels;
            }

            //cache and update these as needed instead of new-ing up every scanline?
            var tileset = new TileSet(VideoMemory);
            var bgMap = new BackgroundMap(Registers, tileset, VideoMemory);
            var window = new Window(Registers, tileset, VideoMemory);
            //var sprites = new SpriteSet(Registers, tileset, VideoMemory)?

            var bgMapY = (byte)(CurrentLine + Registers.ScrollY);
            for (int x = 0; x < ScreenWidthInPixels; x++)
            {
                var bgMapX = (byte)(x + Registers.ScrollX);
                screenPixels[CurrentLine * ScreenWidthInPixels + x] = bgMap.GetPixelAt(bgMapX, bgMapY);
                window.DrawOntoScanline(ref screenPixels, x, CurrentLine);
                DrawSpritesOntoScanline(tileset, x);
            }

            return screenPixels;
        }

        private void RenderBlankLine()
        {
            for (int x = 0; x < ScreenWidthInPixels; x++)
            {
                screenPixels[CurrentLine * ScreenWidthInPixels + x] = 0;
            }
        }

        private void DrawSpritesOntoScanline(TileSet tileset, int x)
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

                if (!sprite.OverlapsCoordinates(x, CurrentLine)) continue;

                //TODO: sprite priority logic, see: http://bgb.bircd.org/pandocs.htm#vramspriteattributetableoam
                //TODO: 10 sprites per scanline limit?
                byte? spritePixel = sprite.GetPixel(tileset, x, y: CurrentLine);
                if (!spritePixel.HasValue) continue;    //transparency
                screenPixels[CurrentLine * ScreenWidthInPixels + x] = spritePixel.Value;
            }
        }

        public byte[] RenderScreen()
        {
            return screenPixels;
        }

        public byte[] ForceRenderScreen()
        {
            for (CurrentLine = 0; CurrentLine < ScreenHeightInPixels; CurrentLine++)
            {
                RenderScanline();
            }
            return RenderScreen();
        }

        public override string ToString() => $"{CurrentMode} LY: {CurrentLine}";
    }
}
