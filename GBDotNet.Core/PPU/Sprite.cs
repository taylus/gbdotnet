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
        public const int BytesPerSprite = 4;
        public const int TotalSprites = 40;

        public byte PositionY { get; set; }     //vertical position (plus 16) -- an offscreen value (e.g. y = 0 or y >= 160) hides the sprite
        public byte TruePositionY
        {
            get => (byte)(PositionY - 16);
            set => PositionY = (byte)(value + 16);
        }

        public byte PositionX { get; set; }     //horizontal position (plus 8) -- an offscreen value (e.g. x = 0 or x >= 168) hides the sprite,
        public byte TruePositionX               //but it will still count against the 10 sprites per scanline limit, so set Y offscreen instead
        {
            get => (byte)(PositionX - 8);
            set => PositionX = (byte)(value + 8);
        }

        public bool Visible => PPURegisters.LCDControl.SpriteDisplayEnabled && TruePositionX < PPU.ScreenWidthInPixels && TruePositionY < PPU.ScreenHeightInPixels;

        public byte TileNumber { get; set; }    //always unsigned and references tile data at $8000 - $8FFF (tile banks 0 and 1)

        public byte Attributes { get; set; }    //interpreted by flags below

        public bool IsBehindBackground => Attributes.IsBitSet(7);
        public bool IsFlippedVertically => Attributes.IsBitSet(6);
        public bool IsFlippedHorizontally => Attributes.IsBitSet(5);
        public int PaletteNumber => Attributes.IsBitSet(4) ? 1 : 0;
        //bits 3-0 of the Attributes byte are only used in Game Boy Color mode, so they are ignored here

        public PPURegisters PPURegisters { get; private set; }
        public Palette Palette => PaletteNumber == 0 ? PPURegisters.SpritePalette0 : PPURegisters.SpritePalette1;
        public int HeightInPixels => PPURegisters.LCDControl.AreSprites8x16 ? Tile.HeightInPixels * 2 : Tile.HeightInPixels;
        public int WidthInPixels => Tile.WidthInPixels;

        public Sprite(byte positionY, byte positionX, byte tileNumber, byte attributes, PPURegisters ppuRegisters)
        {
            PositionY = positionY;
            PositionX = positionX;
            TileNumber = tileNumber;
            Attributes = attributes;
            PPURegisters = ppuRegisters;
        }

        public void Render(TileSet tileset, ref byte[] spriteLayer)
        {
            if (!Visible) return;
            var tiles = GetTiles(tileset);
            for (int y = 0; y < HeightInPixels; y++)
            {
                for (int x = 0; x < WidthInPixels; x++)
                {
                    byte? spritePixel = GetPixel(tiles[y / Tile.HeightInPixels], x, y % Tile.HeightInPixels);
                    if (!spritePixel.HasValue) continue; //transparency
                    var screenCoordinates = LocalToScreenCoordinates(x, y);
                    spriteLayer[screenCoordinates.y * PPU.ScreenWidthInPixels + screenCoordinates.x] = spritePixel.Value;
                    //TODO: implement IsBehindBackground attribute flag
                }
            }
        }

        /// <summary>
        /// Returns the two tiles used by this sprite in 8x16 mode (the first should be drawn above the second).
        /// If the sprite is in 8x8 mode, returns the sprite's single tile.
        /// </summary>
        /// <see cref="http://bgb.bircd.org/pandocs.htm#vramspriteattributetableoam"/>
        private Tile[] GetTiles(TileSet tileset)
        {
            if (PPURegisters.LCDControl.AreSprites8x8)
            {
                return new Tile[] { tileset[TileNumber] };
            }
            else
            {
                //clear and set the 0 bit so each pair is always even on top, odd on bottom
                return new Tile[] { tileset[TileNumber & 0xFE], tileset[TileNumber | 0x01] };
            }
        }

        public bool OverlapsColumn(int x) => (x >= TruePositionX) && (x < (TruePositionX + WidthInPixels));
        public bool OverlapsScanline(int y) => (y >= TruePositionY) && (y < (TruePositionY + HeightInPixels));
        public bool OverlapsCoordinates(int x, int y) => OverlapsColumn(x) && OverlapsScanline(y);

        /// <summary>
        /// Returns the pixel color (0-3) at the given screen coordinates.
        /// </summary>
        public byte? GetPixel(TileSet tileset, int x, int y)
        {
            var tiles = GetTiles(tileset);
            (x, y) = ScreenToLocalCoordinates(x, y);
            return GetPixel(tiles[y / Tile.HeightInPixels], x, y);
        }

        private (int x, int y) LocalToScreenCoordinates(int x, int y) => (x + TruePositionX, y + TruePositionY);
        private (int x, int y) ScreenToLocalCoordinates(int x, int y) => (x - TruePositionX, y - TruePositionY);

        /// <summary>
        /// Returns the pixel color (0-3) at the given sprite-local coordinates.
        /// </summary>
        private byte? GetPixel(Tile tile, int x, int y)
        {
            y %= Tile.HeightInPixels;
            if (IsFlippedHorizontally) x = Tile.WidthInPixels - x - 1;
            if (IsFlippedVertically) y = Tile.HeightInPixels - y - 1;
            var paletteIndex = tile[x, y];
            if (paletteIndex == 0) return null;
            return Palette[paletteIndex];
        }
    }
}
