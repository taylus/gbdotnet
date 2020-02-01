using System;
using System.IO;
using GBDotNet.Core;

namespace GBDotNet.ConsoleApp
{
    public class Program
    {
        private const string vramDumpPath = @"input\links_awakening_you_are_on_koholint_island.vram.dump";
        private const string oamDumpPath = @"input\links_awakening_you_are_on_koholint_island.oam.dump";
        private const string tilesetPixelsOutputPath = "links_awakening_you_are_on_koholint_island.tileset.bin";
        private const string bgMapPixelsOutputPath = "links_awakening_you_are_on_koholint_island.bgmap.bin";
        private const string spritePixelsOutputPath = "links_awakening_you_are_on_koholint_island.sprites.bin";
        private const string screenPixelsOutputPath = "links_awakening_you_are_on_koholint_island.screen.bin";

        public static void Main()
        {
            var ppu = InitializePPU();
            Console.WriteLine(ppu.Registers.LCDControl + Environment.NewLine);

            var tilesetPixels = ppu.RenderTileSet();
            var bgmapPixels = ppu.RenderBackgroundMap(ppu.TileSet);
            var spritePixels = ppu.RenderSprites(ppu.TileSet);
            var screenPixels = ppu.ForceRenderScreen();

            string pathRoot = Environment.ExpandEnvironmentVariables("%userprofile%");
            WriteFile(tilesetPixels, Path.Combine(pathRoot, "GitHub", "monogameboy", "input", tilesetPixelsOutputPath));
            WriteFile(bgmapPixels, Path.Combine(pathRoot, "GitHub", "monogameboy", "input", bgMapPixelsOutputPath));
            WriteFile(spritePixels, Path.Combine(pathRoot, "GitHub", "monogameboy", "input", spritePixelsOutputPath));
            WriteFile(screenPixels, Path.Combine(pathRoot, "GitHub", "monogameboy", "input", screenPixelsOutputPath));
        }

        private static PPU InitializePPU()
        {
            var vram = new Memory(vramDumpPath);
            var oam = new Memory(oamDumpPath);
            //magic numbers in registers captured from bgb at the same point the memory dumps were saved
            var regs = new PPURegisters(lcdc: 0xE7, bgPalette: 0xE4, spritePalette0: 0x1C, spritePalette1: 0xE4);
            return new PPU(regs, vram, oam);
        }

        private static void WriteFile(byte[] data, string outputPath)
        {
            Console.WriteLine($"Writing {outputPath}...");
            File.WriteAllBytes(outputPath, data);
        }
    }
}
