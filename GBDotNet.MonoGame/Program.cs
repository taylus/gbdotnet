using System;
using System.Diagnostics;
using System.IO;
using GBDotNet.Core;

namespace MonoGameBoy
{
    /// <summary>
    /// This program runs the emulator core using MonoGame for a UI.
    /// </summary>
    public static class Program
    {
        //TODO: load from command-line args
        //private const string romPath = @"C:\roms\gb\Tetris.gb";
        //private const string romPath = @"C:\roms\gb\hello-brandon.gb";
        //private const string romPath = @"D:\GitHub\gbdotnet\gb-test-roms\cpu_instrs\cpu_instrs.gb";
        //private const string romPath = @"D:\GitHub\gbdotnet\gb-test-roms\cpu_instrs\individual\11-op a,(hl).gb"; //reports 9E failed
        private const string romPath = @"D:\GitHub\gbdotnet\gb-test-roms\cpu_instrs\individual\01-special.gb";
        private const string logPath = "monogameboy.log";

        [STAThread]
        public static void Main()
        {
            using (var log = new StreamWriter(logPath))
            {
                try
                {
                    Console.SetOut(log);
                    var (cpu, ppu) = BootEmulator();
                    using (var game = new MonoGameBoy(cpu, ppu, romPath))
                        game.Run();
                }
                finally
                {
                    //Process.Start(new ProcessStartInfo() { FileName = logPath, UseShellExecute = true });
                }
            }
        }

        private static (CPU cpu, PPU ppu) BootEmulator()
        {
            var ppuRegs = new PPURegisters();
            var memoryBus = new MemoryBus(ppuRegs);
            memoryBus.Attach(new GameLinkConsole());
            var ppu = new PPU(ppuRegs, memoryBus);
            ppu.Boot();

            var cpu = new CPU(new Registers(), memoryBus);
            //cpu.Boot();

            var rom = new RomFile(romPath);
            memoryBus.LoadRom(rom);

            return (cpu, ppu);
        }
    }
}
