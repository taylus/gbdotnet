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
    public class TileMap
    {
        public const int WidthInTiles = 32;
        public const int HeightInTiles = 32;
        public const int NumTiles = WidthInTiles * HeightInTiles;
        public byte[] Tiles { get; private set; }       //tile index numbers
        public int BaseAddress { get; private set; }    //either $9800 or $9C00 based on LCD control register
        public TileSet TileSet { get; private set; }

        public TileMap(int baseAddress, TileSet tileset, IMemory vram)
        {
            BaseAddress = baseAddress;
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
        public byte[] Render()
        {
            const int widthInPixels = WidthInTiles * Tile.WidthInPixels;
            const int heightInPixels = HeightInTiles * Tile.HeightInPixels;

            var pixels = new byte[widthInPixels * heightInPixels];

            for (int x = 0; x < widthInPixels; x++)
            {
                for (int y = 0; y < heightInPixels; y++)
                {
                    int tileX = x / Tile.WidthInPixels;
                    int tileY = y / Tile.HeightInPixels;
                    byte tileNumber = Tiles[tileY * WidthInTiles + tileX];
                    int tilePixelX = x % Tile.WidthInPixels;
                    int tilePixelY = y % Tile.HeightInPixels;
                    pixels[y * widthInPixels + x] = TileSet[tileNumber][tilePixelX, tilePixelY];
                }
            }

            return pixels;
        }
    }
}
