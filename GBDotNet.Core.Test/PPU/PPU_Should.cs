using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GBDotNet.Core.Test.Integration
{
    [TestClass]
    public class PPU_Should
    {
        // Note: if these tests fail, they will report the index # in the byte arrays that differ.
        // To translate that back into 2D pixel coordinates, use this formula: (x = i % width, y = i / width)
        // Maybe write my own pixel array comparison routine that does this, instead of using CollectionAssert.AreEqual()...?

        [TestMethod]
        public void Generate_Blank_Screen_When_LCD_Is_Disabled()
        {
            //LCDC bit 7
            Assert.Inconclusive("Test not yet implemented.");
        }

        [TestMethod]
        public void Generate_Expected_Tileset_Pixels_From_Known_VRAM_Dump()
        {
            var vram = Memory.FromFile(Path.Combine("PPU", "Input", "tetris.tileset.dump"));
            var ppu = new PPU(new PPURegisters(), vram, oam: new Memory());

            var actualPixels = ppu.RenderTileSet();
            var expectedPixels = ImageHelper.LoadImageAsPaletteIndexedByteArray(Path.Combine("PPU", "Expected", "tetris.tileset.png"));

            CollectionAssert.AreEqual(expectedPixels, actualPixels, "Rendered tileset does not match expected image.");
        }

        [TestMethod]
        public void Generate_Expected_Background_Map_Pixels_From_Known_VRAM_Dump_Using_Unsigned_Tile_Numbers()
        {
            var vram = Memory.FromFile(Path.Combine("PPU", "Input", "tetris_title_screen.vram.dump"));
            var ppu = new PPU(new PPURegisters(lcdc: 0x10), vram, oam: new Memory());

            var actualPixels = ppu.RenderBackgroundMap(ppu.TileSet);
            var expectedPixels = ImageHelper.LoadImageAsPaletteIndexedByteArray(Path.Combine("PPU", "Expected", "tetris_title_screen_expected_bgmap.png"));

            CollectionAssert.AreEqual(expectedPixels, actualPixels, "Rendered background map does not match expected image.");
        }

        [TestMethod]
        public void Generate_Expected_Background_Map_Pixels_From_Known_VRAM_Dump_Using_Signed_Tile_Numbers()
        {
            var vram = Memory.FromFile(Path.Combine("PPU", "Input", "pokemon_reds_room_vram.dump"));
            var ppu = new PPU(new PPURegisters(lcdc: 0xE3), vram, oam: new Memory());

            var actualPixels = ppu.RenderBackgroundMap(ppu.TileSet);
            var expectedPixels = ImageHelper.LoadImageAsPaletteIndexedByteArray(Path.Combine("PPU", "Expected", "pokemon_reds_room_expected_bgmap.png"));

            CollectionAssert.AreEqual(expectedPixels, actualPixels, "Rendered background map does not match expected image.");
        }

        public void Generate_Blank_Background_Map_Pixels_When_Background_Map_Drawing_Is_Disabled()
        {
            //LCDC bit 0
            Assert.Inconclusive("Test not yet implemented.");
        }

        [TestMethod]
        public void Generate_Expected_Window_Pixels_From_Known_VRAM_Dump()
        {
            //TODO: signed/unsigned test variants?
            //other variants? (see LCDC flags)
            Assert.Inconclusive("Test not yet implemented.");
        }

        [TestMethod]
        public void Generate_Blank_Window_Pixels_When_Window_Drawing_Is_Disabled()
        {
            //LCDC bit 5
            Assert.Inconclusive("Test not yet implemented.");
        }

        [TestMethod]
        public void Generate_Expected_Sprite_Pixels_From_Known_VRAM_And_OAM_Dumps()
        {
            var vram = Memory.FromFile(Path.Combine("PPU", "Input", "tetris_title_screen.vram.dump"));
            var oam = Memory.FromFile(Path.Combine("PPU", "Input", "tetris_title_screen.oam.dump"));
            var ppu = new PPU(new PPURegisters(), vram, oam);

            var actualPixels = ppu.RenderSprites(ppu.TileSet);
            var expectedPixels = ImageHelper.LoadImageAsPaletteIndexedByteArray(Path.Combine("PPU", "Expected", "tetris_title_screen_expected_sprites.png"));

            CollectionAssert.AreEqual(expectedPixels, actualPixels, "Rendered background map does not match expected image.");
        }

        [TestMethod]
        public void Generate_Expected_Sprite_Pixels_From_Known_VRAM_And_OAM_Dumps_With_Horizontally_Flipped_Sprites()
        {
            var vram = Memory.FromFile(Path.Combine("PPU", "Input", "pokemon_reds_room_vram.dump"));
            var ppu = new PPU(new PPURegisters(lcdc: 0xE3), vram, oam: new Memory());

            var actualPixels = ppu.RenderSprites(ppu.TileSet);
            var expectedPixels = ImageHelper.LoadImageAsPaletteIndexedByteArray(Path.Combine("PPU", "Expected", "pokemon_reds_room_expected_sprites.png"));

            CollectionAssert.AreEqual(expectedPixels, actualPixels, "Rendered background map does not match expected image.");
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
        
        public void Generate_Blank_Sprite_Pixels_When_Sprite_Drawing_Is_Disabled()
        {
            //LCDC bit 1
            Assert.Inconclusive("Test not yet implemented.");
        }
    }
}
