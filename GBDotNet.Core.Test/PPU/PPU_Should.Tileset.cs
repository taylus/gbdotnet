using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GBDotNet.Core.Test.Integration
{
    public partial class PPU_Should
    {
        [TestMethod]
        public void Generate_Expected_Tileset_Pixels_From_Known_VRAM_Dump()
        {
            var vram = Memory.FromFile(Path.Combine("PPU", "Input", "tetris.tileset.dump"));
            var regs = new PPURegisters();
            var memBus = new MemoryBus(regs) { VideoMemory = vram };
            var ppu = new PPU(regs, memBus);
            ppu.TileSet.UpdateFrom(vram);

            var actualPixels = ppu.RenderTileSet();
            var expectedPixels = ImageHelper.LoadImageAsPaletteIndexedByteArray(Path.Combine("PPU", "Expected", "tetris.tileset.png"));

            AssertPixelsMatch(expectedPixels, actualPixels, width: TileSet.WidthInPixels);
        }
    }
}
