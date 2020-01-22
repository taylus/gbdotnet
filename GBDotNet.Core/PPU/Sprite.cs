namespace GBDotNet.Core
{
    /// <summary>
    /// A single 8 x 8 (or 8 x 16) pixel tile that can move independently of the
    /// background and renders with a transparent palette color 0. Sprites are
    /// stored in object attribute memory (OAM), a 160-byte memory region for up
    /// to 40 sprites, though only 10 sprites can appear per scanline.
    /// </summary>
    /// <see cref="http://bgb.bircd.org/pandocs.htm#vramspriteattributetableoam"/>
    /// <see cref="http://bgb.bircd.org/pandocs.htm#lcdmonochromepalettes"/>
    /// <see cref="https://github.com/taylus/gameboy-graphics/blob/master/building_a_rom.md#an-aside-about-game-boy-video-memory"/>
    public class Sprite
    {
        public byte PositionY { get; set; }     //vertical position (plus 16) -- an offscreen value (e.g. y = 0 or y >= 160) hides the sprite
        public byte TruePositionY
        {
            get => (byte)(PositionY - 16);
            set => PositionY = (byte)(value + 16);
        }

        public byte PositionX { get; set; }     //horizontal position (plus 8)
        public byte TruePositionX
        {
            get => (byte)(PositionX - 8);
            set => PositionX = (byte)(value + 8);
        }

        public byte TileNumber { get; set; }    //always unsigned and references tile data at $8000 - $8FFF (tile banks 0 and 1)

        public byte Attributes { get; set; }    //interpreted by flags below

        public bool IsBehindBackground => Attributes.IsBitSet(7);
        public bool IsFlippedVertically => Attributes.IsBitSet(6);
        public bool IsFlippedHorizontally => Attributes.IsBitSet(5);
        public int PaletteNumber => Attributes.IsBitSet(4) ? 1 : 0;
        //bits 3-0 of the Attributes byte are only used in Game Boy Color mode, so they are ignored here
    }
}
