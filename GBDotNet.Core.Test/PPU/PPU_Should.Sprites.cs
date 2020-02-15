using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GBDotNet.Core.Test.Integration
{
    public partial class PPU_Should
    {
        [TestMethod]
        public void Generate_Expected_Sprite_Pixels_From_Known_VRAM_And_OAM_Dumps()
        {
            var vram = Memory.FromFile(Path.Combine("PPU", "Input", "tetris_title_screen.vram.dump"));
            var oam = Memory.FromFile(Path.Combine("PPU", "Input", "tetris_title_screen.oam.dump"));
            var regs = new PPURegisters(lcdc: 0xD3, spritePalette0: 0xFF, spritePalette1: 0xFF);
            var memBus = new MemoryBus(regs) { VideoMemory = vram, ObjectAttributeMemory = oam };
            var ppu = new PPU(regs, memBus);
            ppu.TileSet.UpdateFrom(ppu.VideoMemory);

            var actualPixels = ppu.RenderSprites();
            var expectedPixels = ImageHelper.LoadImageAsPaletteIndexedByteArray(Path.Combine("PPU", "Expected", "tetris_title_screen_expected_sprites.png"));

            AssertPixelsMatch(expectedPixels, actualPixels, width: PPU.ScreenWidthInPixels);
        }

        [TestMethod]
        public void Generate_Expected_Sprite_Pixels_From_Known_VRAM_And_OAM_Dumps_With_Horizontally_Flipped_Sprites()
        {
            var vram = Memory.FromFile(Path.Combine("PPU", "Input", "pokemon_reds_room.vram.dump"));
            var oam = Memory.FromFile(Path.Combine("PPU", "Input", "pokemon_reds_room.oam.dump"));
            var regs = new PPURegisters(lcdc: 0xE3, spritePalette0: 0xE4, spritePalette1: 0xE4);
            var memBus = new MemoryBus(regs) { VideoMemory = vram, ObjectAttributeMemory = oam };
            var ppu = new PPU(regs, memBus);
            ppu.TileSet.UpdateFrom(ppu.VideoMemory);

            var actualPixels = ppu.RenderSprites();
            var expectedPixels = ImageHelper.LoadImageAsPaletteIndexedByteArray(Path.Combine("PPU", "Expected", "pokemon_reds_room_expected_sprites.png"));

            AssertPixelsMatch(expectedPixels, actualPixels, width: PPU.ScreenWidthInPixels);
        }

        [TestMethod]
        public void Generate_Expected_Sprite_Pixels_From_Known_VRAM_And_OAM_Dumps_With_Vertically_Flipped_Sprites()
        {
            Assert.Inconclusive("Test not yet implemented.");
        }

        [TestMethod]
        public void Generate_Expected_Sprite_Pixels_From_Known_VRAM_And_OAM_Dumps_With_Sprites_Behind_Background()
        {
            Assert.Inconclusive("Test not yet implemented.");
        }

        [TestMethod]
        public void Generate_Blank_Sprite_Pixels_When_Sprite_Drawing_Is_Disabled()
        {
            var vram = Memory.FromFile(Path.Combine("PPU", "Input", "pokemon_reds_room.vram.dump"));
            var oam = Memory.FromFile(Path.Combine("PPU", "Input", "pokemon_reds_room.oam.dump"));
            var regs = new PPURegisters(lcdc: 0xE3, spritePalette0: 0xE4, spritePalette1: 0xE4);
            var memBus = new MemoryBus(regs) { VideoMemory = vram, ObjectAttributeMemory = oam };
            var ppu = new PPU(regs, memBus);

            ppu.Registers.LCDControl.SpriteDisplayEnabled = false;
            var actualPixels = ppu.RenderSprites();
            var expectedPixels = new byte[PPU.ScreenWidthInPixels * PPU.ScreenHeightInPixels];

            AssertPixelsMatch(expectedPixels, actualPixels, width: PPU.ScreenWidthInPixels);
        }
    }
}
