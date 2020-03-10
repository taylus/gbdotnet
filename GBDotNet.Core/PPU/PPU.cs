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
        public const double FramesPerSecond = 59.7;

        private PPUMode CurrentMode
        {
            get => Registers.LCDStatus.ModeFlag;
            set
            {
                Registers.LCDStatus.ModeFlag = value;
                if (value == PPUMode.HBlank && Registers.LCDStatus.InterruptOnMode0HBlank ||
                    value == PPUMode.VBlank && Registers.LCDStatus.InterruptOnMode1VBlank ||
                    value == PPUMode.OamScan && Registers.LCDStatus.InterruptOnMode2OamScan)
                {
                    RequestLCDStatInterrupt();
                }
            }
        }

        internal byte CurrentLine
        {
            get => Registers.CurrentScanline;
            set
            {
                Registers.CurrentScanline = value;
                Registers.LCDStatus.ScanlineConcidence = Registers.CurrentScanline == Registers.CompareScanline;
                if (Registers.LCDStatus.InterruptOnScanlineCoincidence && Registers.LCDStatus.ScanlineConcidence)
                {
                    RequestLCDStatInterrupt();
                }
            }
        }

        private readonly byte[] screenPixels = new byte[ScreenWidthInPixels * ScreenHeightInPixels];
        private int cycleCounter;

        public long RenderedFrameCount { get; private set; }
        public byte[] ScreenBackBuffer { get; } = new byte[ScreenWidthInPixels * ScreenHeightInPixels];
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

        public TileSet TileSet => MemoryBus.TileSet;
        public SpriteOam Sprites => MemoryBus.SpriteOam;
        private readonly BackgroundMap bgMap;
        private readonly Window window;

        public PPU(PPURegisters registers, MemoryBus memoryBus)
        {
            Registers = registers;
            MemoryBus = memoryBus;
            bgMap = new BackgroundMap(this);
            window = new Window(this);
        }

        internal PPU(byte[] memory)
        {
            if (memory.Length != Memory.Size)
                throw new ArgumentException($"Expected entire {Memory.Size} byte memory map, but got {memory.Length} bytes.", nameof(memory));

            Registers = new PPURegisters(new ArraySegment<byte>(memory, offset: 0xFF40, count: 12));
            MemoryBus = new MemoryBus(Registers, new SoundRegisters());
            VideoMemory = new Memory(new ArraySegment<byte>(memory, offset: 0x8000, count: 8192));
            TileSet.UpdateFrom(VideoMemory);
            ObjectAttributeMemory = new Memory(new ArraySegment<byte>(memory, offset: 0xFE00, count: 160));
            Sprites.UpdateFrom(ObjectAttributeMemory);
            bgMap = new BackgroundMap(this);
            window = new Window(this);
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
            //if (!Registers.LCDControl.Enabled) return;
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

        public byte[] RenderSprites() => Sprites.Render();
        public byte[] RenderBackgroundMap() => bgMap.Render();
        public byte[] RenderTileSet() => TileSet.Render();
        public byte[] RenderWindow() => window.Render();

        public byte[] RenderScreen()
        {
            Array.Copy(screenPixels, ScreenBackBuffer, screenPixels.Length);
            RenderedFrameCount++;
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

        public byte[] RenderScanline()
        {
            if (!Registers.LCDControl.Enabled) return screenPixels;
            //if (CurrentLine > ScreenHeightInPixels) return screenPixels; //?

            var bgMapY = (byte)(CurrentLine + Registers.ScrollY);
            for (int x = 0; x < ScreenWidthInPixels; x++)
            {
                if (Registers.LCDControl.BackgroundDisplayEnabled)
                {
                    var bgMapX = (byte)(x + Registers.ScrollX);
                    screenPixels[CurrentLine * ScreenWidthInPixels + x] = bgMap.GetPixelAt(bgMapX, bgMapY);
                }
                if (Registers.LCDControl.WindowDisplayEnabled)
                {
                    window.DrawOntoScanline(screenPixels, x, CurrentLine);
                }
            }

            if (Registers.LCDControl.SpriteDisplayEnabled) Sprites.RenderScanline(CurrentLine, screenPixels);
            return screenPixels;
        }

        public override string ToString() => $"{CurrentMode} LY: {CurrentLine}";

        private void RequestLCDStatInterrupt() => MemoryBus.InterruptFlags.LCDStatInterruptRequested = true;
    }
}
