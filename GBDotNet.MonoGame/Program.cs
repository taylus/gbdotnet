using System;
using System.IO;
using GBDotNet.Core;
using Microsoft.Extensions.Configuration;

namespace MonoGameBoy
{
    /// <summary>
    /// This program runs the emulator core using MonoGame for a UI.
    /// </summary>
    public static class Program
    {
        private static IConfiguration config;
        private static string romPath;
        private static string logPath;
        private static bool useBootRom = true;

        [STAThread]
        public static void Main(string[] args)
        {
            ParseCommandLineArgs(args, out romPath);
            if (string.IsNullOrWhiteSpace(romPath)) Environment.Exit(-1);
            Configure();

            StreamWriter logWriter = null;
            bool loggingEnabled = !string.IsNullOrWhiteSpace(logPath);
            if (loggingEnabled)
            {
                logWriter = new StreamWriter(logPath);
                if (loggingEnabled) Console.SetOut(logWriter);
            }

            try
            {
                var emulator = Boot();
                using var game = new MonoGameBoy(emulator, romPath, useBootRom, loggingEnabled, config);
                game.Run();
            }
            finally
            {
                if (logWriter != null) logWriter.Dispose();
            }
        }

        private static void ParseCommandLineArgs(string[] args, out string romPath)
        {
            if (args.Length == 1)
            {
                romPath = args[0];
            }
            else
            {
                romPath = null;
                Console.WriteLine("Usage: gbdotnet rom.gb");
            }
        }

        private static void Configure()
        {
            config = BuildConfiguration();
            logPath = config.GetSection("log")?.Value;
            bool.TryParse(config.GetSection("useBootRom")?.Value, out useBootRom);
        }

        private static IConfiguration BuildConfiguration() => new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        private static Emulator Boot()
        {
            var ppuRegs = new PPURegisters();
            var soundRegs = new SoundRegisters();
            var memoryBus = new MemoryBus(ppuRegs, soundRegs) { IsBootRomMapped = useBootRom };
            //memoryBus.Attach(new GameLinkConsole());
            var ppu = new PPU(ppuRegs, memoryBus);
            ppu.Boot();

            var cpu = new CPU(new Registers(), memoryBus);
            if (!useBootRom) cpu.BootWithoutBootRom();

            var rom = Cartridge.LoadFrom(romPath);
            memoryBus.Load(rom);

            var apu = new APU(soundRegs);
            var joypad = new Joypad(memoryBus.JoypadRegister);
            return new Emulator(cpu, ppu, apu, joypad);
        }
    }
}
