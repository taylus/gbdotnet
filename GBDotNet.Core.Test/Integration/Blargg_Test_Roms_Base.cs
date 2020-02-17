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
