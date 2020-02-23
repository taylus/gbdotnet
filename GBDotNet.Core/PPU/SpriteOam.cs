namespace GBDotNet.Core
{
    /// <summary>
    /// Represents the set of all 40 sprites in Object Attribute Memory.
    /// </summary>
    /// <see cref="Sprite"/>
    public class SpriteOam
    {
        public const int TotalSprites = 40;
        public const int SizeInBytes = TotalSprites * Sprite.SizeInBytes;

        public PPU PPU { get; set; }
        public Sprite[] Sprites { get; private set; } = new Sprite[TotalSprites];

        public SpriteOam(PPU ppu)
        {
            PPU = ppu;
            UpdateAllSpritesFromOam(ppu.MemoryBus.ObjectAttributeMemory);
        }

        public void UpdateAllSpritesFromOam(IMemory oam)
        {
            for (int i = 0; i < TotalSprites; i++)
            {
                UpdateSpriteFromOam(i, oam);
            }
        }

        public void UpdateSpriteFromOam(int spriteNumber, IMemory oam)
        {
            var spriteAddress = spriteNumber * Sprite.SizeInBytes;
            Sprites[spriteNumber] = new Sprite
            (
                positionY: oam[spriteAddress],
                positionX: oam[spriteAddress + 1],
                tileNumber: oam[spriteAddress + 2],
                attributes: oam[spriteAddress + 3],
                PPU.Registers
            );
        }

        public byte[] Render()
        {
            var spriteLayer = new byte[PPU.ScreenWidthInPixels * PPU.ScreenHeightInPixels];
            for (int i = 0; i < SizeInBytes; i += Sprite.SizeInBytes)
            {
                var sprite = new Sprite(positionY: PPU.ObjectAttributeMemory[i],
                    positionX: PPU.ObjectAttributeMemory[i + 1],
                    tileNumber: PPU.ObjectAttributeMemory[i + 2],
                    attributes: PPU.ObjectAttributeMemory[i + 3],
                    PPU.Registers);

                if (!sprite.Visible) continue;

                //TODO: sprite priority logic, see: http://bgb.bircd.org/pandocs.htm#vramspriteattributetableoam
                sprite.Render(PPU.TileSet, ref spriteLayer);
            }

            return spriteLayer;
        }

        public byte[] RenderScanline(int y, ref byte[] screenPixels)
        {
            //TODO: for performance, don't do this every scanline! update only on memory writes like Tileset does
            UpdateAllSpritesFromOam(PPU.ObjectAttributeMemory);
            for (int x = 0; x < PPU.ScreenWidthInPixels; x++)
            {
                foreach (var sprite in Sprites)
                {
                    if (!sprite.OverlapsCoordinates(x, y)) continue;
                    //TODO: sprite priority logic, see: http://bgb.bircd.org/pandocs.htm#vramspriteattributetableoam
                    //TODO: implement 10-sprite-per-scanline limit, I think this will help performance a lot (160 x 40 sprite checks per scanline is way too many!)
                    byte? spritePixel = sprite.GetPixel(PPU.TileSet, x, y);
                    if (!spritePixel.HasValue) continue;    //transparency
                    screenPixels[y * PPU.ScreenWidthInPixels + x] = spritePixel.Value;
                }
            }

            return screenPixels;
        }
    }
}
