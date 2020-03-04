﻿using System;
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
        //private const string romPath = @"C:\roms\gb\Dr. Mario (World).gb";
        //private const string romPath = @"C:\roms\gb\hello-brandon.gb";
        //private const string romPath = @"C:\roms\gb\Super Mario Land.gb";
        private const string romPath = @"C:\roms\gb\Super Mario Land 2.gb";
        //private const string romPath = @"D:\GitHub\gbdotnet\gb-test-roms\cpu_instrs\cpu_instrs.gb";
        private const string logPath = "monogameboy.log";
        private const bool useBootRom = false;
        private const bool loggingEnabled = true;

        [STAThread]
        public static void Main()
        {
            using (var log = new StreamWriter(logPath))
            {
                try
                {
                    if (loggingEnabled) Console.SetOut(log);
                    var emulator = Boot();
                    using (var game = new MonoGameBoy(emulator, romPath, useBootRom, loggingEnabled))
                        game.Run();
                }
                finally
                {
                    if (loggingEnabled) Process.Start(new ProcessStartInfo() { FileName = logPath, UseShellExecute = true });
                }
            }
        }

        private static Emulator Boot()
        {
            var ppuRegs = new PPURegisters();
            var memoryBus = new MemoryBus(ppuRegs) { IsBootRomMapped = useBootRom };
            //memoryBus.Attach(new GameLinkConsole());
            var ppu = new PPU(ppuRegs, memoryBus);
            ppu.Boot();

            var cpu = new CPU(new Registers(), memoryBus);
            if (!useBootRom) cpu.BootWithoutBootRom();

            var rom = Cartridge.LoadFrom(romPath);
            memoryBus.Load(rom);

            var joypad = new Joypad(memoryBus.JoypadRegister);
            return new Emulator(cpu, ppu, joypad);
        }
    }
}
