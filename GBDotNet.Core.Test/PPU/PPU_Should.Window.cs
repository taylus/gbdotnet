using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GBDotNet.Core.Test.Integration
{
    public partial class PPU_Should
    {
        [TestMethod]
        public void Generate_Expected_Window_Pixels_From_Known_VRAM_Dump_Using_Unsigned_Tile_Numbers()
        {
            Assert.Inconclusive("Test not yet implemented.");
        }

        [TestMethod]
        public void Generate_Expected_Window_Pixels_From_Known_VRAM_Dump_Using_Signed_Tile_Numbers()
        {
            var vram = Memory.FromFile(Path.Combine("PPU", "Input", "links_awakening_you_are_on_koholint_island.vram.dump"));
            var ppu = new PPU(new PPURegisters(lcdc: 0xE7, bgPalette: 0xE4), vram, oam: new Memory());

            var actualPixels = ppu.RenderWindow(ppu.TileSet);
            var expectedPixels = ImageHelper.LoadImageAsPaletteIndexedByteArray(Path.Combine("PPU", "Expected", "links_awakening_you_are_on_koholint_island_expected_window.png"));

            AssertPixelsMatch(expectedPixels, actualPixels, width: 256);
        }

        [TestMethod]
        public void Generate_Blank_Window_Pixels_When_Window_Drawing_Is_Disabled()
        {
            //LCDC bit 5
            Assert.Inconclusive("Test not yet implemented.");
        }
    }
}
