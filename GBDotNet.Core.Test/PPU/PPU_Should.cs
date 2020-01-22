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
        public void Generate_Expected_Tileset_Pixels_From_Known_VRAM_Dump()
        {
            var vram = Memory.FromFile(Path.Combine("PPU", "Input", "tetris.tileset.dump"));
            var ppu = new PPU(new PPURegisters(), vram);

            var actualPixels = ppu.RenderTileSet();
            var expectedPixels = ImageHelper.LoadImageAsPaletteIndexedByteArray(Path.Combine("PPU", "Expected", "tetris.tileset.png"));

            CollectionAssert.AreEqual(expectedPixels, actualPixels, "Rendered tileset does not match expected image.");
        }

        [TestMethod]
        public void Generate_Expected_Background_Map_Pixels_From_Known_VRAM_Dump_Using_Unsigned_Tile_Numbers()
        {
            var vram = Memory.FromFile(Path.Combine("PPU", "Input", "tetris_title_screen.vram.dump"));
            var ppu = new PPU(new PPURegisters(), vram);

            var actualPixels = ppu.RenderBackgroundMap(ppu.TileSet);
            var expectedPixels = ImageHelper.LoadImageAsPaletteIndexedByteArray(Path.Combine("PPU", "Expected", "tetris_title_screen_expected_bgmap.png"));

            CollectionAssert.AreEqual(expectedPixels, actualPixels, "Rendered background map does not match expected image.");
        }

        [TestMethod]
        public void Generate_Expected_Background_Map_Pixels_From_Known_VRAM_Dump_Using_Signed_Tile_Numbers()
        {
            //TODO: need to find (or create) a ROM that uses signed background tile numbers
        }
    }
}
