using System;
using System.Linq;

namespace GBDotNet.Core
{
    /// <summary>
    /// A block of 3 x 128 (384) tiles stored in VRAM at address range $8000 - $97FF.
    /// </summary>
    /// <remarks>
    /// The tile set is located at address range $8000 - $97FF.
    /// Each 16 bytes makes up one tile as each pixel is 2 bits (4 colors).
    /// </remarks>
    /// <see cref="http://gameboy.mongenel.com/dmg/asmmemmap.html"/>
    public class TileSet
    {
        public const int WidthInTiles = 16;
        public const int HeightInTiles = 24;
        public const int WidthInPixels = WidthInTiles * Tile.WidthInPixels;
        public const int HeightInPixels = HeightInTiles * Tile.HeightInPixels;
        public const int NumTiles = WidthInTiles * HeightInTiles;
        public Tile[] Tiles { get; private set; } = new Tile[NumTiles];

        public Tile this[int i] => Tiles[i];
        private Tile this[int x, int y] => Tiles[y * WidthInTiles + x];

        public TileSet(IMemory vram)
        {
            UpdateFrom(vram);
        }

        /// <summary>
        /// Loads the entire tileset from the given video memory.
        /// </summary>
        public void UpdateFrom(IMemory vram)
        {
            for (int i = 0; i < NumTiles; i++)
            {
                var tileBytes = vram.Skip(Tile.BytesPerTile * i).Take(Tile.BytesPerTile);
                Tiles[i] = new Tile(tileBytes.ToArray());
            }
        }

        /// <summary>
        /// Updates the (single) relevant tile for the given video memory update.
        /// </summary>
        public void UpdateFromMemoryWrite(IMemory vram, int vramAddress)
        {
            var updatedTileIndex = vramAddress / Tile.BytesPerTile;
            var tileBytes = new ArraySegment<byte>(vram.Data, offset: Tile.BytesPerTile * updatedTileIndex, count: Tile.BytesPerTile);
            Tiles[updatedTileIndex] = new Tile(tileBytes);  //TODO: would it be better to *update* the tile based only on this byte instead?
        }

        /// <summary>
        /// Returns a 128 x 192 pixel (16 x 24 tile) image of the tileset.
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
                    int tilePixelX = x % Tile.WidthInPixels;
                    int tilePixelY = y % Tile.HeightInPixels;
                    pixels[y * widthInPixels + x] = this[tileX, tileY][tilePixelX, tilePixelY];
                }
            }

            return pixels;
        }
    }
}
