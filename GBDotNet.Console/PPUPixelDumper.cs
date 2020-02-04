using System;
using System.IO;
using GBDotNet.Core;

namespace GBDotNet.ConsoleApp
{
    /// <summary>
    /// This program reads in a memory dump captured from bgb, feeds it through the PPU
    /// to generate graphics, and writes those graphics to monogameboy for display.
    /// </summary>
    public class PPUPixelDumper
    {
        private const string memoryDumpPath = @"input\metroid_2.dump";

        public static void Main()
        {
            var ppu = new PPU(File.ReadAllBytes(memoryDumpPath));
            Console.WriteLine(ppu.Registers.LCDControl + Environment.NewLine);

            var tilesetPixels = ppu.RenderTileSet();
            var bgMapPixels = ppu.RenderBackgroundMap(ppu.TileSet);
            var windowPixels = ppu.RenderWindow(ppu.TileSet);
            var spritePixels = ppu.RenderSprites(ppu.TileSet);
            var screenPixels = ppu.ForceRenderScreen();

            WritePixelDataFiles(tilesetPixels, bgMapPixels, windowPixels, spritePixels, screenPixels);
        }

        private static void WritePixelDataFiles(byte[] tilesetPixels, byte[] bgMapPixels, byte[] windowPixels, byte[] spritePixels, byte[] screenPixels)
        {
            string sceneName = Path.GetFileName(memoryDumpPath);
            string tilesetPixelsOutputFile = Path.ChangeExtension(sceneName, ".tileset.bin");
            string bgMapPixelsOutputFile = Path.ChangeExtension(sceneName, ".bgmap.bin");
            string windowPixelsOutputFile = Path.ChangeExtension(sceneName, ".window.bin");
            string spritePixelsOutputFile = Path.ChangeExtension(sceneName, ".sprites.bin");
            string screenPixelsOutputFile = Path.ChangeExtension(sceneName, ".screen.bin");

            string userprofileDir = Environment.ExpandEnvironmentVariables("%userprofile%");
            string outputPath = Path.Combine(userprofileDir, "GitHub", "monogameboy", "input");
            WriteFile(tilesetPixels, Path.Combine(outputPath, tilesetPixelsOutputFile));
            WriteFile(bgMapPixels, Path.Combine(outputPath, bgMapPixelsOutputFile));
            WriteFile(windowPixels, Path.Combine(outputPath, windowPixelsOutputFile));
            WriteFile(spritePixels, Path.Combine(outputPath, spritePixelsOutputFile));
            WriteFile(screenPixels, Path.Combine(outputPath, screenPixelsOutputFile));
        }

        private static void WriteFile(byte[] data, string outputPath)
        {
            Console.WriteLine($"Writing {outputPath}...");
            File.WriteAllBytes(outputPath, data);
        }
    }
}
