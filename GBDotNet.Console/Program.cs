using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GBDotNet.Core;

namespace GBDotNet.ConsoleApp
{
    public class Program
    {
        //TODO: load from command-line args
        private const string vramDumpPath = @"C:\roms\gb\dev\tetris.vram.dump";
        private const string logPath = "gbdotnet.log";
        private const string pixelBufferPath = "tetris_tileset_pixels.bin";
        private static readonly int? TestCycleLimit = 1024 * 1024 * 4;

        public static void Main()
        {
            var stderr = Console.Error; //capture stderr to log exceptions both to log file and console

            using (var log = new StreamWriter(logPath))
            {
                //try
                {
                    Console.SetOut(log);
                    var system = Start();
                    //Run(system.CPU);
                    var tilesetPixels = system.PPU.RenderTileSet();
                    Console.WriteLine($"Writing {pixelBufferPath}...");
                    File.WriteAllBytes(pixelBufferPath, tilesetPixels);
                    OpenFile(pixelBufferPath);
                    File.Copy(pixelBufferPath, Path.Combine(@"D:\GitHub\monogameboy\input", pixelBufferPath), overwrite: true);
                }
                /*
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    stderr.WriteLine(ex);
                }
                finally
                {
                    //OpenFile(logPath);
                }
                */
            }
        }

        private static (CPU CPU, PPU PPU) Start()
        {
            var memoryBus = new MemoryBus();
            var cpu = new CPU(new Registers(), memoryBus);
            cpu.Boot();

            var rom = CreateInfiniteLoopTestRom();
            memoryBus.LoadRom(rom);

            var vram = new Memory(vramDumpPath);
            memoryBus.LoadVram(vram);

            var ppu = new PPU(new PPURegisters(), memoryBus.Vram);

            return (cpu, ppu);
        }

        private static RomFile CreateInfiniteLoopTestRom()
        {
            var header = Enumerable.Repeat<byte>(0x00, 0x100);
            var program = header.Concat(new byte[] { 0x18, 0xFE }); //jr, -2
            return new RomFile(program.ToArray());
        }

        private static void Run(CPU cpu)
        {
            do
            {
                Console.WriteLine(cpu);
                cpu.Tick();
                if (cpu.TotalElapsedCycles >= TestCycleLimit)
                    throw new EmulationException($"Test cycle limit of {TestCycleLimit} has been reached (possible infinite loop?)");
            } while (!cpu.IsHalted);

            Console.WriteLine("CPU halted.");
        }

        private static void OpenFile(string path)
        {
            Process.Start(new ProcessStartInfo() { FileName = path, UseShellExecute = true });
        }
    }
}
