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

        public static void Main()
        {
            var ppu = InitializePPU();
            Console.WriteLine(ppu.Registers.LCDControl + Environment.NewLine);

            var tilesetPixels = ppu.RenderTileSet();
            var bgmapPixels = ppu.RenderBackgroundMap(ppu.TileSet);
            var spritePixels = ppu.RenderSprites(ppu.TileSet);

            WriteFile(tilesetPixels, tilesetPixelsOutputPath);
            WriteFile(bgmapPixels, bgMapPixelsOutputPath);
            WriteFile(spritePixels, spritePixelsOutputPath);
        }

        private static PPU InitializePPU()
        {
            var vram = new Memory(vramDumpPath);
            var oam = new Memory(oamDumpPath);
            return new PPU(new PPURegisters(lcdc: 0xE3), vram, oam);
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
