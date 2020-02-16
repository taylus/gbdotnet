using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GBDotNet.Core.Test.Integration
{
    /// <summary>
    /// Run Blargg's test ROMs headlessly and confirm they pass by examining the text they write to the serial port.
    /// Note that most (all?) of these ROMs don't halt when their tests complete, but go into an infinite loop.
    /// So these tests have to stop running after so many cycles in order to avoid infinitely looping, themselves.
    /// </summary>
    /// <see cref="https://github.com/retrio/gb-test-roms"/>
    [TestClass]
    public class Blargg_Test_Roms : Program_Tests_Base
    {
        [TestMethod]
        public void Run_All_Test_Roms()
        {
            //Parallel.ForEach(GetTestRoms(), (rom) => RunTestRom(rom.FullName));
            RunTestRom(@"D:\GitHub\gbdotnet\gb-test-roms\cpu_instrs\individual\01-special.gb");
        }

        private static void RunTestRom(string romPath)
        {
            var (cpu, outputStream) = Arrange(romPath);

            while (!cpu.IsHalted && cpu.TotalElapsedCycles <= maxCycles)
            {
                cpu.Tick();
            }

            string output = Encoding.ASCII.GetString(outputStream.ToArray());
            Console.WriteLine(output);

            if (output.Contains("Failed"))
            {
                Assert.Fail($"Test ROM {Path.GetFileName(romPath)} failed, see output for more details.");
            }
            else if (!output.Contains("Passed"))
            {
                Assert.Inconclusive($"Test ROM {Path.GetFileName(romPath)} did not pass, but did not report failure. (Does it need to run longer?)");
            }
        }

        private static (CPU cpu, MemoryStream outputStream) Arrange(string romPath)
        {
            var outputStream = new MemoryStream();
            var memoryBus = new MemoryBus(new PPURegisters()) { IsBootRomMapped = false };
            memoryBus.Attach(new GameLinkConsole(outputStream));
            memoryBus.LoadRom(new RomFile(romPath));

            var cpu = new CPU(new Registers(), memoryBus);
            cpu.BootWithoutBootRom();   //no PPU => can't run boot ROM (it will hang waiting for blank)

            return (cpu, outputStream);
        }

        /*
        private static FileInfo[] GetTestRoms()
        {
            var dir = new DirectoryInfo(GetTestRomDirectory());
            return dir.GetFiles("*.gb", SearchOption.AllDirectories);
        }

        private static string GetTestRomDirectory()
        {
            return Path.Combine(GetSolutionDirectory(), "gb-test-roms");
        }

        private static string GetSolutionDirectory()
        {
            var thisAssembly = Assembly.GetExecutingAssembly();
            var thisDir = new DirectoryInfo(thisAssembly.Location);
            return thisDir.Parent?.Parent?.Parent?.Parent?.Parent?.FullName ?? "";
        }
        */
    }
}
