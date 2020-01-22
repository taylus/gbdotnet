using System;
using System.Diagnostics;
using System.IO;
using GBDotNet.Core;

namespace GBDotNet.ConsoleApp
{
    public class Program
    {
        private const string vramDumpPath = @"input\tetris.vram.dump";
        private const string pixelBufferOutputPath = "tetris_bgmap_pixels.bin";

        public static void Main()
        {
            var ppu = InitializePPU();
            var bgMapPixels = ppu.RenderBackgroundMap(ppu.TileSet);
            Console.WriteLine($"Writing {pixelBufferOutputPath}...");
            File.WriteAllBytes(pixelBufferOutputPath, bgMapPixels);
            OpenFile(pixelBufferOutputPath);
            File.Copy(pixelBufferOutputPath, Path.Combine(@"D:\GitHub\monogameboy\input", pixelBufferOutputPath), overwrite: true);
        }

        private static PPU InitializePPU()
        {
            var vram = new Memory(vramDumpPath);
            return new PPU(new PPURegisters(), vram);
        }

        private static void OpenFile(string path)
        {
            Process.Start(new ProcessStartInfo() { FileName = path, UseShellExecute = true });
        }
    }
}
