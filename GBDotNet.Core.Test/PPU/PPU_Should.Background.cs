using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GBDotNet.Core.Test.Integration
{
    public partial class PPU_Should
    {
        [TestMethod]
        public void Generate_Expected_Background_Map_Pixels_From_Known_VRAM_Dump_Using_Unsigned_Tile_Numbers()
        {
            var vram = Memory.FromFile(Path.Combine("PPU", "Input", "tetris_title_screen.vram.dump"));
            var ppu = new PPU(new PPURegisters(lcdc: 0xD3, bgPalette: 0xE4), vram, oam: new Memory());

            var actualPixels = ppu.RenderBackgroundMap(ppu.TileSet);
            var expectedPixels = ImageHelper.LoadImageAsPaletteIndexedByteArray(Path.Combine("PPU", "Expected", "tetris_title_screen_expected_bgmap.png"));

            AssertPixelsMatch(expectedPixels, actualPixels, width: 256);
        }

        [TestMethod]
        public void Generate_Expected_Background_Map_Pixels_From_Known_VRAM_Dump_Using_Signed_Tile_Numbers()
        {
            var vram = Memory.FromFile(Path.Combine("PPU", "Input", "pokemon_reds_room.vram.dump"));
            var ppu = new PPU(new PPURegisters(lcdc: 0xE3, bgPalette: 0xE4), vram, oam: new Memory());

            var actualPixels = ppu.RenderBackgroundMap(ppu.TileSet);
            var expectedPixels = ImageHelper.LoadImageAsPaletteIndexedByteArray(Path.Combine("PPU", "Expected", "pokemon_reds_room_expected_bgmap.png"));

            AssertPixelsMatch(expectedPixels, actualPixels, width: 256);
        }

        [TestMethod]
        public void Generate_Blank_Background_Map_Pixels_When_Background_Map_Drawing_Is_Disabled()
        {
            var vram = Memory.FromFile(Path.Combine("PPU", "Input", "pokemon_reds_room.vram.dump"));
            var ppu = new PPU(new PPURegisters(lcdc: 0xE0), vram, oam: new Memory());   //LCDC bit 0 = 0 => background is disabled

            Assert.IsFalse(ppu.Registers.LCDControl.BackgroundDisplayEnabled);

            var actualPixels = ppu.RenderBackgroundMap(ppu.TileSet);
            var expectedPixels = new byte[TileMap.WidthInPixels * TileMap.HeightInPixels];

            AssertPixelsMatch(expectedPixels, actualPixels, width: 256);
        }
    }
}
