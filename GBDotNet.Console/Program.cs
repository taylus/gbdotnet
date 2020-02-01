using System;
using System.IO;
using GBDotNet.Core;

namespace GBDotNet.ConsoleApp
{
    public class Program
    {
        private const string sceneName = "links_awakening_you_are_on_koholint_island";
        private const string vramDumpPath = @"input\" + sceneName + ".vram.dump";
        private const string oamDumpPath = @"input\" + sceneName + ".oam.dump";
        private const string tilesetPixelsOutputPath = sceneName + ".tileset.bin";
        private const string bgMapPixelsOutputPath = sceneName + ".bgmap.bin";
        private const string windowPixelsOutputPath = sceneName + ".window.bin";
        private const string spritePixelsOutputPath = sceneName + ".sprites.bin";
        private const string screenPixelsOutputPath = sceneName + ".screen.bin";

        public static void Main()
        {
            var ppu = InitializePPU();
            Console.WriteLine(ppu.Registers.LCDControl + Environment.NewLine);

            var tilesetPixels = ppu.RenderTileSet();
            var bgmapPixels = ppu.RenderBackgroundMap(ppu.TileSet);
            var windowPixels = ppu.RenderWindow(ppu.TileSet);
            var spritePixels = ppu.RenderSprites(ppu.TileSet);
            var screenPixels = ppu.ForceRenderScreen();

            string userprofileDir = Environment.ExpandEnvironmentVariables("%userprofile%");
            string pathRoot = Path.Combine(userprofileDir, "GitHub", "monogameboy", "input");
            WriteFile(tilesetPixels, Path.Combine(pathRoot, tilesetPixelsOutputPath));
            WriteFile(bgmapPixels, Path.Combine(pathRoot, bgMapPixelsOutputPath));
            WriteFile(windowPixels, Path.Combine(pathRoot, windowPixelsOutputPath));
            WriteFile(spritePixels, Path.Combine(pathRoot, spritePixelsOutputPath));
            WriteFile(screenPixels, Path.Combine(pathRoot, screenPixelsOutputPath));
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
