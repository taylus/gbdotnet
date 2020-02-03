using System.Linq;

namespace GBDotNet.Core
{
    /// <summary>
    /// Tile maps are 32 x 32 (1K) blocks of tile numbers which define a background or window.
    /// </summary>
    /// <remarks>
    /// They exist in VRAM at address ranges $9800 - $9BFF and $9C00 - $9FFF and which map goes
    /// with the window and which goes with the background is determined by bits 6 and 3 of
    /// <see cref="LCDControlRegister"/>, respectively.
    /// </remarks>
    /// <see cref="http://gameboy.mongenel.com/dmg/asmmemmap.html"/>
    public abstract class TileMap
    {
        public const int WidthInTiles = 32;
        public const int HeightInTiles = 32;
        public const int NumTiles = WidthInTiles * HeightInTiles;
        public const int WidthInPixels = WidthInTiles * Tile.WidthInPixels;
        public const int HeightInPixels = HeightInTiles * Tile.HeightInPixels;

        public PPURegisters Registers { get; private set; }
        public byte[] Tiles { get; private set; }       //tile index numbers
        public abstract int BaseAddress { get; }
        public TileSet TileSet { get; private set; }

        /// <summary>
        /// Gets/sets the tileset tile # stored at the given tile coordinates.
        /// </summary>
        /// <param name="x">A tile x coordinate between 0-31 inclusive.</param>
        /// <param name="y">A tile y coordinate between 0-31 inclusive.</param>
        protected byte this[int x, int y]
        {
            get => Tiles[y * WidthInTiles + x];
            set => Tiles[y * WidthInTiles + x] = value;
        }

        public TileMap(PPURegisters registers, TileSet tileset, IMemory vram)
        {
            Registers = registers;
            TileSet = tileset;
            UpdateFrom(vram);
        }

        public void UpdateFrom(IMemory vram)
        {
            Tiles = vram.Skip(BaseAddress - 0x8000).Take(NumTiles).ToArray();
        }

        /// <summary>
        /// Returns a 256 x 256 pixel (32 x 32 tile) image of the tilemap built
        /// by indexing tile numbers into the current tileset.
        /// </summary>
        public virtual byte[] Render()
        {
            var pixels = new byte[WidthInPixels * HeightInPixels];

            for (int x = 0; x < WidthInPixels; x++)
            {
                for (int y = 0; y < HeightInPixels; y++)
                {
                    pixels[y * WidthInPixels + x] = GetPixelAt(x, y);
                }
            }

            return pixels;
        }

        /// <summary>
        /// Gets the color (0-3) at the given pixel coordinates.
        /// </summary>
        public byte GetPixelAt(int x, int y)
        {
            int tileX = x / Tile.WidthInPixels;
            int tileY = y / Tile.HeightInPixels;
            int tileNumber = this[tileX, tileY];
            if (Registers.LCDControl.AreBackgroundAndWindowTileNumbersSigned && tileNumber < 128)
            {
                tileNumber += 256;
            }
            int tilePixelX = x % Tile.WidthInPixels;
            int tilePixelY = y % Tile.HeightInPixels;
            var paletteIndex = TileSet[tileNumber][tilePixelX, tilePixelY];
            return Registers.BackgroundPalette[paletteIndex];
        }
    }
}
