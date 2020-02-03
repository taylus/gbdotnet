using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GBDotNet.Core.Test.Integration
{
    public partial class PPU_Should
    {
        [TestMethod]
        public void Generate_Blank_Screen_When_LCD_Is_Disabled()
        {
            byte[] allMemory = File.ReadAllBytes(Path.Combine("PPU", "Input", "mario_land_level_1_paused.dump"));
            var ppu = new PPU(allMemory);

            ppu.Registers.LCDControl.Enabled = false;
            var actualPixels = ppu.ForceRenderScreen();
            var expectedPixels = new byte[PPU.ScreenWidthInPixels * PPU.ScreenHeightInPixels];

            AssertPixelsMatch(expectedPixels, actualPixels, width: PPU.ScreenWidthInPixels);
        }

        [TestMethod]
        public void Generate_Expected_Screen_Pixels_From_Known_VRAM_And_OAM_Dumps_With_Background_And_Sprites()
        {
            var vram = Memory.FromFile(Path.Combine("PPU", "Input", "pokemon_reds_room.vram.dump"));
            var oam = Memory.FromFile(Path.Combine("PPU", "Input", "pokemon_reds_room.oam.dump"));
            //magic numbers in PPU registers collected from bgb at the point the above memory dumps were captured
            var regs = new PPURegisters(lcdc: 0xE3, scrollY: 0xD0, windowX: 0x07, windowY: 0x90, bgPalette: 0xE4, spritePalette0: 0xD0, spritePalette1: 0xE0);
            var ppu = new PPU(regs, vram, oam);

            var actualPixels = ppu.ForceRenderScreen();
            var expectedPixels = ImageHelper.LoadImageAsPaletteIndexedByteArray(Path.Combine("PPU", "Expected", "pokemon_reds_room_expected_screen.png"));

            AssertPixelsMatch(expectedPixels, actualPixels, width: PPU.ScreenWidthInPixels);
        }

        [TestMethod]
        public void Generate_Expected_Screen_Pixels_From_Known_VRAM_And_OAM_Dumps_With_Background_And_8_x_16_Sprites_And_Window()
        {
            var vram = Memory.FromFile(Path.Combine("PPU", "Input", "links_awakening_you_are_on_koholint_island.vram.dump"));
            var oam = Memory.FromFile(Path.Combine("PPU", "Input", "links_awakening_you_are_on_koholint_island.oam.dump"));
            //magic numbers in PPU registers collected from bgb at the point the above memory dumps were captured
            var regs = new PPURegisters(lcdc: 0xE7, windowX: 0x06, windowY: 0x80, bgPalette: 0xE4, spritePalette0: 0x1C, spritePalette1: 0xE4);
            var ppu = new PPU(regs, vram, oam);

            var actualPixels = ppu.ForceRenderScreen();
            var expectedPixels = ImageHelper.LoadImageAsPaletteIndexedByteArray(Path.Combine("PPU", "Expected", "links_awakening_you_are_on_koholint_island_expected_screen.png"));

            AssertPixelsMatch(expectedPixels, actualPixels, width: PPU.ScreenWidthInPixels);
        }

        [TestMethod]
        public void Generate_Expected_Screen_Pixels_From_Known_Memory_Dump_With_Horizontally_And_Vertically_Positioned_Window()
        {
            byte[] allMemory = File.ReadAllBytes(Path.Combine("PPU", "Input", "mario_land_level_1_paused.dump"));
            var ppu = new PPU(allMemory);

            var actualPixels = ppu.ForceRenderScreen();
            var expectedPixels = ImageHelper.LoadImageAsPaletteIndexedByteArray(Path.Combine("PPU", "Expected", "mario_land_level_1_paused_expected_screen.png"));

            AssertPixelsMatch(expectedPixels, actualPixels, width: PPU.ScreenWidthInPixels);
        }

        [TestMethod]
        public void Generate_Expected_Screen_Pixels_From_Known_Memory_Dump_With_Nonstandard_Background_Palette()
        {
            //most games use a background palette of 0xE4, which maps the background colors 1:1
            //(e.g. color #3 is 11 => black, color #2 is 10 => dark gray, color #1 is 01 => light gray, color #0 is 00 => white)
            //so they appear correct even without using the background color palette
            //Donkey Kong Land 2 is one game that uses a different background palette, so test that it renders as expected

            byte[] allMemory = File.ReadAllBytes(Path.Combine("PPU", "Input", "donkey_kong_land_2_level_1.dump"));
            var ppu = new PPU(allMemory);

            var actualPixels = ppu.ForceRenderScreen();
            var expectedPixels = ImageHelper.LoadImageAsPaletteIndexedByteArray(Path.Combine("PPU", "Expected", "donkey_kong_land_2_level_1_expected_screen.png"));

            AssertPixelsMatch(expectedPixels, actualPixels, width: PPU.ScreenWidthInPixels);
        }
    }
}
