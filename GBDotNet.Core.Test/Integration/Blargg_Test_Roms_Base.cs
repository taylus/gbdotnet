using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GBDotNet.Core.Test.Integration
{
    /// <summary>
    /// Run Blargg's test ROMs headlessly and confirm they pass by examining the text they write to the serial port.
    /// Note that most (all?) of these ROMs don't halt when their tests complete, but go into an infinite loop.
    /// So these tests have to stop running after so many cycles in order to avoid infinitely looping, themselves.
    /// </summary>
    /// <see cref="https://github.com/retrio/gb-test-roms"/>
    public abstract class Blargg_Test_Roms_Base : Program_Tests_Base
    {
        protected static void RunTestRom(string romPath)
        {
            var outputStream = new MemoryStream();
            var gameLinkConsole = new GameLinkConsole(outputStream);
            var memoryBus = new MemoryBus(new PPURegisters()) { IsBootRomMapped = false };
            memoryBus.Attach(gameLinkConsole);
            memoryBus.LoadRom(new RomFile(romPath));

            var cpu = new CPU(new Registers(), memoryBus);
            cpu.BootWithoutBootRom();   //no PPU => can't run boot ROM (it will hang waiting for blank)

            while (cpu.TotalElapsedCycles <= maxCycles)
            {
                cpu.Tick();

                if (gameLinkConsole.HasFreshUpdate)
                {
                    string output = Encoding.ASCII.GetString(outputStream.ToArray());
                    if (output.Contains("Failed"))
                    {
                        Console.WriteLine(output);
                        Assert.Fail($"Test ROM {Path.GetFileName(romPath)} failed, see output for more details.");
                    }
                    else if (output.Contains("Passed"))
                    {
                        Console.WriteLine(output);
                        return;
                    }

                    gameLinkConsole.HasFreshUpdate = false;
                }
            }

            Assert.Inconclusive($"Test ROM {Path.GetFileName(romPath)} did not pass, but did not report failure. (Does it need to run longer?)");
        }

        protected static void RunHaltBugTestRom(string romPath)
        {
            var memoryBus = new MemoryBus(new PPURegisters()) { IsBootRomMapped = false };
            memoryBus.LoadRom(new RomFile(romPath));
            var ppu = new PPU(memoryBus.PPURegisters, memoryBus);   //this rom requires a PPU or it'll infinite loop waiting for LY
            var cpu = new CPU(new Registers(), memoryBus);
            cpu.BootWithoutBootRom();

            const int maxCycles = 7_000_000;
            while (cpu.TotalElapsedCycles <= maxCycles)
            {
                cpu.Tick();
                ppu.Tick(cpu.CyclesLastTick);
            }

            if (ScanBackgroundMapForString(cpu.Memory, "Failed"))
            {
                Console.WriteLine(BackgroundMapToString(cpu.Memory));
                Assert.Fail($"Test ROM {Path.GetFileName(romPath)} failed, see output for more details.");
            }
            else if (ScanBackgroundMapForString(cpu.Memory, "Passed"))
            {
                Console.WriteLine(BackgroundMapToString(cpu.Memory));
                return;
            }
            else
            {
                Assert.Inconclusive($"Test ROM {Path.GetFileName(romPath)} did not pass, but did not report failure. (Does it need to run longer?)");
            }
        }

        private static bool ScanBackgroundMapForString(IMemory memory, string searchString) => ScanMemoryForString(memory, 0x9800, 0x9BFF, searchString) >= 0;

        private static int ScanMemoryForString(IMemory memory, int startAddress, int endAddressInclusive, string searchString)
        {
            var searchBytes = Encoding.ASCII.GetBytes(searchString);
            var searchStringLength = searchBytes.Length;
            for (int i = startAddress; i <= endAddressInclusive - searchStringLength; i++)
            {
                int k = 0;
                for (; k < searchStringLength; k++)
                {
                    if (searchBytes[k] != memory[i + k]) break;
                }
                if (k == searchStringLength) return i;
            }
            return -1;
        }

        private static string BackgroundMapToString(IMemory memory) => MemoryRegionToString(memory, 0x9800, 0x9BFF);

        private static string MemoryRegionToString(IMemory memory, int startAddress, int endAddressInclusive)
        {
            var stringBuilder = new StringBuilder();
            for (int i = startAddress; i < endAddressInclusive; i++)
            {
                stringBuilder.Append((char)memory[i]);
                if (i % 16 == 15) stringBuilder.Append(Environment.NewLine);
            }
            return stringBuilder.ToString().Trim();
        }

        protected virtual string GetTestRomsDirectory()
        {
            return Path.Combine(GetSolutionDirectory(), "gb-test-roms");
        }

        protected static string GetSolutionDirectory()
        {
            var thisAssembly = Assembly.GetExecutingAssembly();
            var thisDir = new DirectoryInfo(thisAssembly.Location);
            return thisDir.Parent?.Parent?.Parent?.Parent?.Parent?.FullName ?? "";
        }
    }
}
