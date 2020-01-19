using System.Linq;

namespace GBDotNet.Core
{
    /// <summary>
    /// The tile set is a block of 3 x 128 (384) tiles which are 8 x 8 pixels each.
    /// </summary>
    /// <remarks>
    /// The tile set is located at address range $8000 - $97FF.
    /// Each 16 bytes makes up one tile as each pixel is 2 bits (4 colors).
    /// </remarks>
    /// <see cref="http://gameboy.mongenel.com/dmg/asmmemmap.html"/>
    /// <see cref="https://www.huderlem.com/demos/gameboy2bpp.html"/>
    public class TileSet
    {
        public const int BaseAddress = 0x8000;
        public const int NumTiles = 384;
        public Tile[] Tiles { get; private set; } = new Tile[NumTiles];

        public TileSet(IMemory memory)
        {
            UpdateFrom(memory);
        }

        public void UpdateFrom(IMemory memory)
        {
            for(int i = 0; i < NumTiles; i++)
            {
                var tileBytes = memory.Skip(Tile.BytesPerTile * i).Take(Tile.BytesPerTile);
                Tiles[i] = new Tile(tileBytes.ToArray());
            }
        }

        //TODO: add a means of rendering the tileset for debug display in the emulator
    }
}
