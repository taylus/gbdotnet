using System;
using System.Linq;

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

        public PPURegisters PPURegisters { get; private set; }
        public IMemory ObjectAttributeMemory { get; private set; }
        public TileSet Tileset { get; private set; }
        public Sprite[] Sprites { get; private set; } = new Sprite[TotalSprites];

        public SpriteOam(PPURegisters ppuRegisters, IMemory oam, TileSet tileset)
        {
            PPURegisters = ppuRegisters;
            ObjectAttributeMemory = oam;
            Tileset = tileset;
            UpdateFrom(ObjectAttributeMemory);
        }

        public void UpdateFrom(IMemory oam)
        {
            for (int i = 0; i < TotalSprites; i++)
            {
                UpdateSpriteFromOam(i, oam);
            }
        }

        private void UpdateSpriteFromOam(int spriteNumber, IMemory oam)
        {
            var spriteAddress = spriteNumber * Sprite.SizeInBytes;
            Sprites[spriteNumber] = new Sprite
            (
                positionY: oam[spriteAddress],
                positionX: oam[spriteAddress + 1],
                tileNumber: oam[spriteAddress + 2],
                attributes: oam[spriteAddress + 3],
                PPURegisters
            );
        }

        public void UpdateFromMemoryWrite(IMemory oam, int oamAddress)
        {
            var updatedSpriteNumber = oamAddress / Sprite.SizeInBytes;
            UpdateSpriteFromOam(updatedSpriteNumber, oam);
        }

        public byte[] Render()
        {
            var spriteLayer = new byte[PPU.ScreenWidthInPixels * PPU.ScreenHeightInPixels];
            foreach (var sprite in Sprites)
            {
                //TODO: sprite priority logic, see: http://bgb.bircd.org/pandocs.htm#vramspriteattributetableoam
                sprite.Render(Tileset, ref spriteLayer);
            }
            return spriteLayer;
        }

        public byte[] RenderScanline(int y, ref byte[] screenPixels)
        {
            var spritesOnScanline = Sprites.Where(s => s.Visible && s.OverlapsScanline(y));
            foreach (var sprite in spritesOnScanline)
            {
                //TODO: sprite priority logic, see: http://bgb.bircd.org/pandocs.htm#vramspriteattributetableoam
                //TODO: implement 10-sprite-per-scanline limit (toggleable?)
                //draw the sprite's individual pixels onto this scanline (TODO: move into Sprite)
                for (int x = 0; x < Tile.WidthInPixels; x++)
                {
                    var spriteX = sprite.TruePositionX + x;
                    if (spriteX >= PPU.ScreenWidthInPixels) continue;
                    byte? spritePixel = sprite.GetPixel(Tileset, spriteX, y);
                    if (!spritePixel.HasValue) continue;    //transparency
                    screenPixels[y * PPU.ScreenWidthInPixels + spriteX] = spritePixel.Value;
                }
            }

            return screenPixels;
        }
    }
}
