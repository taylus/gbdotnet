using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GBDotNet.Core.Test.Integration
{
    [TestClass]
    public class Blargg_Test_Roms_Memory_Timing : Blargg_Test_Roms_Base
    {
        [TestMethod, Ignore("Currently fails!")]
        public void Memory_Timing_Test_ROM_01()
        {
            RunTestRom(Path.Combine(GetTestRomsDirectory(), "01-read_timing.gb"));
        }

        [TestMethod, Ignore("Currently fails!")]
        public void Memory_Timing_Test_ROM_02()
        {
            RunTestRom(Path.Combine(GetTestRomsDirectory(), "02-write_timing.gb"));
        }

        [TestMethod, Ignore("Currently fails!")]
        public void Memory_Timing_Test_ROM_03()
        {
            RunTestRom(Path.Combine(GetTestRomsDirectory(), "03-modify_timing.gb"));
        }

        protected override string GetTestRomsDirectory()
        {
            return Path.Combine(base.GetTestRomsDirectory(), "mem_timing", "individual");
        }
    }
}
