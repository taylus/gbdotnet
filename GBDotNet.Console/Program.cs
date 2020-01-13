using System;
using GBDotNet.Core;

namespace ConsoleApp1
{
    public class Program
    {
        //TODO: load from command-line args
        private const string romPath = @"C:\roms\gb\Tetris (World) (Rev A).gb";

        public static void Main()
        {
            var memoryBus = new MemoryBus();
            var cpu = new CPU(new Registers(), memoryBus);
            cpu.Boot();

            var rom = new RomFile(romPath);
            memoryBus.LoadRom(rom);

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
