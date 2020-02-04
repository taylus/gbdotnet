using System;
using System.Diagnostics;
using System.IO;
using GBDotNet.Core;

namespace GBDotNet.ConsoleApp
{
    /// <summary>
    /// This program runs the emulator core headlessly, executing instructions until
    /// it either crashes or loops forever waiting for input, interrupts, or something
    /// that isn't implemented yet.
    /// </summary>
    public class Program
    {
        //TODO: load from command-line args
        private const string romPath = @"C:\roms\gb\Tetris (World) (Rev A).gb";
        private const string logPath = "gbdotnet.log";

        private static CPU cpu;
        private static PPU ppu;

        public static void Main()
        {
            using (var log = new StreamWriter(logPath))
            {
                var stderr = Console.Error; //capture stderr to log exceptions both to log file and console
                try
                {
                    Console.SetOut(log);
                    Start(romPath);
                    //cpu.Breakpoints.Add(0x0223);
                    Run();
                }
                catch (Exception ex)
                {
                    var emuEx = new EmulationException($"Error executing instruction at address ${cpu.Registers.LastPC:x4}, see inner exception for details.", ex);
                    Console.WriteLine(emuEx);
                    stderr.WriteLine(emuEx);
                }
                finally
                {
                    Process.Start(new ProcessStartInfo() { FileName = logPath, UseShellExecute = true });
                }
            }
        }

        private static void Start(string romPath)
        {
            var ppuRegs = new PPURegisters();
            var memoryBus = new MemoryBus(ppuRegs);
            ppu = new PPU(ppuRegs, memoryBus.VideoMemory, memoryBus.ObjectAttributeMemory);
            cpu = new CPU(new Registers(), memoryBus);
            cpu.Boot();

            var rom = new RomFile(romPath);
            memoryBus.LoadRom(rom);
        }

        private static void Run()
        {
            do
            {
                Console.WriteLine(cpu);
                //Console.WriteLine("Press enter to single step...");
                cpu.Tick();
                ppu.Tick(cpu.CyclesLastTick);
                //Console.ReadLine();
            } while (!cpu.IsHalted);

            Console.WriteLine("CPU halted.");
        }
    }
}
