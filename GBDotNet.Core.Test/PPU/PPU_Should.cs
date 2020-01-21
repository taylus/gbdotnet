using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GBDotNet.Core.Test
{
    [TestClass]
    public class PPU_Should
    {
        [TestMethod]
        public void Generate_Expected_Tileset_Pixels_From_Known_VRAM_Dump()
        {
            var vram = Memory.FromFile(Path.Combine("PPU", "Input", "tetris.tileset.dump"));
            var ppu = new PPU(new PPURegisters(), vram);

            var actualPixels = ppu.RenderTileSet();
            var expectedPixels = ImageHelper.LoadImageAsPaletteIndexedByteArray(Path.Combine("PPU", "Expected", "tetris.tileset.png"));

            CollectionAssert.AreEqual(expectedPixels, actualPixels, "Rendered tileset does not match expected image.");
        }
    }
}
