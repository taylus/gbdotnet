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

        public static void Main()
        {
            var stderr = Console.Error; //capture stderr to log exceptions both to log file and console

            using (var log = new StreamWriter(logPath))
            {
                try
                {
                    Console.SetOut(log);
                    var cpu = Start(romPath);
                    Run(cpu);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    stderr.WriteLine(ex);
                }
                finally
                {
                    Process.Start(new ProcessStartInfo() { FileName = logPath, UseShellExecute = true });
                }
            }
        }

        private static CPU Start(string romPath)
        {
            var memoryBus = new MemoryBus();
            var cpu = new CPU(new Registers(), memoryBus);
            cpu.Boot();

            var rom = new RomFile(romPath);
            memoryBus.LoadRom(rom);

            return cpu;
        }

        private static void Run(CPU cpu)
        {
            do
            {
                Console.WriteLine(cpu);
                //Console.WriteLine("Press enter to single step...");
                cpu.Tick();
                //Console.ReadLine();
            } while (!cpu.IsHalted);

            Console.WriteLine("CPU halted.");
        }
    }
}
