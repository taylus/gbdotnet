using System;
using System.Diagnostics;
using System.IO;
using GBDotNet.Core;

namespace GBDotNet.ConsoleApp
{
    public class Program
    {
        private const string vramDumpPath = @"input\tetris.vram.dump";
        private const string oamDumpPath = @"input\tetris.oam.dump";
        private const string pixelBufferOutputPath = "tetris_sprite_pixels.bin";

        public static void Main()
        {
            var ppu = InitializePPU();
            var spritePixels = ppu.RenderSprites(ppu.TileSet);
            Console.WriteLine($"Writing {pixelBufferOutputPath}...");
            File.WriteAllBytes(pixelBufferOutputPath, spritePixels);
            OpenFile(pixelBufferOutputPath);
            //File.Copy(pixelBufferOutputPath, Path.Combine(@"D:\GitHub\monogameboy\input", pixelBufferOutputPath), overwrite: true);
        }

        private static PPU InitializePPU()
        {
            var vram = new Memory(vramDumpPath);
            var oam = new Memory(oamDumpPath);
            return new PPU(new PPURegisters(), vram, oam);
        }

        private static void OpenFile(string path)
        {
            Process.Start(new ProcessStartInfo() { FileName = path, UseShellExecute = true });
        }
    }
}
