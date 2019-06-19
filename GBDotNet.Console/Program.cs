using System;
using GBDotNet.Core;

namespace ConsoleApp1
{
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("Memory");
            var memory = new Memory(0x03, 0x03, 0x76);
            memory.HexDump(bytesPerLine: 16, stopAfterBytes: 64);

            Console.WriteLine($"{Environment.NewLine}CPU");
            CPU cpu = new CPU(new Registers(), memory);

            do
            {
                Console.WriteLine(cpu);
                Console.WriteLine("Press enter to single step...");
                cpu.Tick();
                Console.ReadLine();
            } while (!cpu.IsHalted);

            Console.WriteLine("CPU halted.");
        }
    }
}
