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

        public PPURegisters()
        {
            LCDControl = new LCDControlRegister();
            LCDStatus = new LCDStatusRegister();
            BackgroundPalette = new Palette();
            SpritePalette0 = new Palette();
            SpritePalette1 = new Palette();
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
        public int WindowTileMapStartAddress => Data.IsBitSet(6) ? 0x9C00 : 0x9800;
        public bool WindowDisplayEnabled => Data.IsBitSet(5);
        /// <summary>
        /// Which two of the three 128-tile "blocks" of tiles are the background
        /// and window tile maps referencing? (sprites always use $8000-$8FFF)
        /// </summary>
        /// <see cref="https://gbdev.gg8.se/wiki/articles/Video_Display#VRAM_Tile_Data"/>
        /// <see cref="https://github.com/taylus/gameboy-graphics/blob/master/building_a_rom.md#an-aside-about-game-boy-video-memory"/>
        public int BackgroundAndWindowTileDataStartAddress => Data.IsBitSet(4) ? 0x8000 : 0x8800;
        public bool BackgroundAndWindowTileNumbersAreSigned => BackgroundAndWindowTileDataStartAddress == 0x8800;
        public int BackgroundTileMapBaseAddress => Data.IsBitSet(3) ? 0x9C00 : 0x9800;
        public bool AreSprites8x16 => Data.IsBitSet(2);
        public bool AreSprites8x8 => !AreSprites8x16;
        public bool SpriteDisplayEnabled => Data.IsBitSet(1);
        public bool BackgroundDisplayEnabled => Data.IsBitSet(0);
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
        public int Color3 => (Data & 0b1100_0000) >> 6;
        public int Color2 => (Data & 0b0011_0000) >> 4;
        public int Color1 => (Data & 0b0000_1100) >> 2;
        public int Color0 => (Data & 0b0000_0011);
    }
}
