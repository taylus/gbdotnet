using System;
using System.Diagnostics;
using System.IO;
using GBDotNet.Core;

namespace GBDotNet.ConsoleApp
{
    public class Program
    {
        private const string vramDumpPath = @"input\pokemon_reds_room_vram.dump";
        private const string oamDumpPath = @"input\pokemon_reds_room_oam.dump";
        private const string tilesetPixelsOutputPath = "pokemon_reds_room_tileset.bin";
        private const string bgMapPixelsOutputPath = "pokemon_reds_room_bgmap.bin";
        private const string spritePixelsOutputPath = "pokemon_reds_room_sprites.bin";
        private const string screenPixelsOutputPath = "pokemon_reds_room_screen.bin";

        public static void Main()
        {
            var ppu = InitializePPU();
            Console.WriteLine(ppu.Registers.LCDControl + Environment.NewLine);

            var tilesetPixels = ppu.RenderTileSet();
            var bgmapPixels = ppu.RenderBackgroundMap(ppu.TileSet);
            var spritePixels = ppu.RenderSprites(ppu.TileSet);
            var screenPixels = ppu.ForceRenderScreen();

            WriteFile(tilesetPixels, tilesetPixelsOutputPath);
            WriteFile(bgmapPixels, bgMapPixelsOutputPath);
            WriteFile(spritePixels, spritePixelsOutputPath);
            WriteFile(screenPixels, screenPixelsOutputPath);
        }

        private static PPU InitializePPU()
        {
            var vram = new Memory(vramDumpPath);
            var oam = new Memory(oamDumpPath);
            //magic numbers coming from bgb at the point the above memory dumps were captured
            var regs = new PPURegisters(lcdc: 0xE3, scrollY: 0xD0, bgPalette: 0xE4, spritePalette0: 0xD0, spritePalette1: 0xE0);
            return new PPU(regs, vram, oam);
        }

        private static void WriteFile(byte[] data, string outputPath)
        {
            Console.WriteLine($"Writing {outputPath}...");
            File.WriteAllBytes(outputPath, data);
            //OpenFile(pixelBufferOutputPath);
            string pathRoot = Environment.ExpandEnvironmentVariables("%userprofile%");
            File.Copy(outputPath, Path.Combine(pathRoot, "GitHub", "monogameboy", "input", outputPath), overwrite: true);
        }

        private static void OpenFile(string path)
        {
            Process.Start(new ProcessStartInfo() { FileName = path, UseShellExecute = true });
        }
    }
}
