using System;

namespace GBDotNet.Core
{
    /// <summary>
    /// Future design notes: <see cref="MemoryBus"/> should direct reads/writes to the appropriate addresses here.
    /// </summary>
    public class PPURegisters
    {
        public LCDControlRegister LCDControl { get; set; }
        public LCDStatusRegister LCDStatus { get; set; }
        public byte ScrollY { get; set; }           //$ff42, http://bgb.bircd.org/pandocs.htm#lcdpositionandscrolling
        public byte ScrollX { get; set; }           //$ff43, http://bgb.bircd.org/pandocs.htm#lcdpositionandscrolling
        public byte CurrentScanline { get; set; }   //$ff44, http://bgb.bircd.org/pandocs.htm#lcdpositionandscrolling
        public byte CompareScanline { get; set; }   //$ff45, http://bgb.bircd.org/pandocs.htm#lcdpositionandscrolling
        public Palette BackgroundPalette { get; set; }
        public Palette SpritePalette0 { get; set; }
        public Palette SpritePalette1 { get; set; }
        public byte WindowY { get; set; }           //$ff4a, http://bgb.bircd.org/pandocs.htm#lcdpositionandscrolling
        public byte WindowX { get; set; }           //$ff4b, http://bgb.bircd.org/pandocs.htm#lcdpositionandscrolling

        public PPURegisters(byte lcdc = 0, byte lcdstat = 0, byte scrollX = 0, byte scrollY = 0,
            byte bgPalette = 0, byte spritePalette0 = 0, byte spritePalette1 = 0)
        {
            LCDControl = new LCDControlRegister() { Data = lcdc };
            LCDStatus = new LCDStatusRegister() { Data = lcdstat };
            BackgroundPalette = new Palette() { Data = bgPalette };
            SpritePalette0 = new Palette() { Data = spritePalette0 };
            SpritePalette1 = new Palette() { Data = spritePalette1 };
            ScrollX = scrollX;
            ScrollY = scrollY;
        }
    }

    /// <summary>
    /// $FF40 http://bgb.bircd.org/pandocs.htm#lcdcontrolregister
    /// aka LCDC or LCDCONT
    /// </summary>
    public class LCDControlRegister
    {
        public byte Data { get; set; }
        public bool Enabled => Data.IsBitSet(7);
        public int WindowTileMapBaseAddress => Data.IsBitSet(6) ? 0x9C00 : 0x9800;
        public bool WindowDisplayEnabled => Data.IsBitSet(5);
        /// <summary>
        /// Which two of the three 128-tile "blocks" of tiles are the background
        /// and window tile maps referencing? (sprites always use $8000-$8FFF)
        /// </summary>
        /// <see cref="https://gbdev.gg8.se/wiki/articles/Video_Display#VRAM_Tile_Data"/>
        /// <see cref="https://github.com/taylus/gameboy-graphics/blob/master/building_a_rom.md#an-aside-about-game-boy-video-memory"/>
        public int BackgroundAndWindowTileDataBaseAddress => Data.IsBitSet(4) ? 0x8000 : 0x8800;
        public bool AreBackgroundAndWindowTileNumbersSigned => BackgroundAndWindowTileDataBaseAddress == 0x8800;
        public int BackgroundTileMapBaseAddress => Data.IsBitSet(3) ? 0x9C00 : 0x9800;
        public bool AreSprites8x16 => Data.IsBitSet(2);
        public bool AreSprites8x8 => !AreSprites8x16;
        public bool SpriteDisplayEnabled => Data.IsBitSet(1);
        public bool BackgroundDisplayEnabled => Data.IsBitSet(0);

        public override string ToString()
        {
            return $"LCD Control: {Data:X2}" + Environment.NewLine +
                   $"  Bit 7 is {(Data.IsBitSet(7) ? 1 : 0)} => LCD is {(Enabled ? "ON" : "OFF")}" + Environment.NewLine +
                   $"  Bit 6 is {(Data.IsBitSet(6) ? 1 : 0)} => Window tilemap base address is ${WindowTileMapBaseAddress:X4}" + Environment.NewLine +
                   $"  Bit 5 is {(Data.IsBitSet(5) ? 1 : 0)} => Window display is {(WindowDisplayEnabled ? "ON" : "OFF")}" + Environment.NewLine +
                   $"  Bit 4 is {(Data.IsBitSet(4) ? 1 : 0)} => Background and window tileset base address is ${BackgroundAndWindowTileDataBaseAddress:X4}" +
                   $" (tile numbers are {(AreBackgroundAndWindowTileNumbersSigned ? "signed" : "unsigned")})" + Environment.NewLine +
                   $"  Bit 3 is {(Data.IsBitSet(3) ? 1 : 0)} => Background tilemap base address is ${BackgroundTileMapBaseAddress:X4}" + Environment.NewLine +
                   $"  Bit 2 is {(Data.IsBitSet(2) ? 1 : 0)} => Sprites are {(AreSprites8x16 ? "8x16" : "8x8")}" + Environment.NewLine +
                   $"  Bit 1 is {(Data.IsBitSet(1) ? 1 : 0)} => Sprite display is {(SpriteDisplayEnabled ? "ON" : "OFF")}" + Environment.NewLine +
                   $"  Bit 0 is {(Data.IsBitSet(0) ? 1 : 0)} => Background display is {(BackgroundDisplayEnabled ? "ON" : "OFF")}";
        }
    }

    /// <summary>
    /// $FF41 http://bgb.bircd.org/pandocs.htm#lcdstatusregister
    /// aka STAT or LCDSTAT
    /// </summary>
    public class LCDStatusRegister
    {
        public byte Data { get; set; }
        public bool InterruptOnScanlineCoincidence => Data.IsBitSet(6);
        public bool InterruptOnMode2OamScan => Data.IsBitSet(5);
        public bool InterruptOnMode1VBlank => Data.IsBitSet(4);
        public bool InterruptOnMode0HBlank => Data.IsBitSet(3);
        /// <summary>
        /// True if <see cref="PPURegisters.CurrentScanline"/> equals <see cref="PPURegisters.CompareScanline"/>.
        /// </summary>
        public bool ScanlineConcidence => Data.IsBitSet(2);
        public PPUMode ModeFlag
        {
            get
            {
                int highBit = Data.IsBitSet(1) ? 1 : 0;
                int lowBit = Data.IsBitSet(0) ? 1 : 0;
                int modeValue = 2 * highBit + lowBit;
                return (PPUMode)modeValue;
            }
            set
            {
                //set last two bits of register to match given mode
                //TODO: do this more concisely w/ bitwise operators
                if (value == PPUMode.HBlank)
                {
                    Data = Data.ClearBit(1);
                    Data = Data.ClearBit(0);
                }
                else if (value == PPUMode.VBlank)
                {
                    Data = Data.ClearBit(1);
                    Data = Data.SetBit(0);
                }
                else if (value == PPUMode.OamScan)
                {
                    Data = Data.SetBit(1);
                    Data = Data.ClearBit(0);
                }
                else if (value == PPUMode.HDraw)
                {
                    Data = Data.SetBit(1);
                    Data = Data.SetBit(0);
                }
            }
        }
    }

    /// <summary>
    /// $FF47 (background palette)
    /// $FF48 (sprite palette #0)
    /// $FF49 (sprite palette #1)
    /// http://bgb.bircd.org/pandocs.htm#lcdmonochromepalettes
    /// </summary>
    public class Palette
    {
        public byte Data { get; set; }
        public byte Color3 => (byte)((Data & 0b1100_0000) >> 6);
        public byte Color2 => (byte)((Data & 0b0011_0000) >> 4);
        public byte Color1 => (byte)((Data & 0b0000_1100) >> 2);
        public byte Color0 => (byte)(Data & 0b0000_0011);
        public byte this[int i]
        {
            get
            {
                if (i == 0) return Color0;
                if (i == 1) return Color1;
                if (i == 2) return Color2;
                if (i == 3) return Color3;
                throw new ArgumentOutOfRangeException(nameof(i), $"Invalid palette index {i}. Palettes are 4 colors (0-3).");
            }
        }
    }
}
