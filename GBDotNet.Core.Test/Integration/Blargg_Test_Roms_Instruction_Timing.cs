using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GBDotNet.Core.Test.Integration
{
    [TestClass]
    public class Blargg_Test_Roms_Instruction_Timing : Blargg_Test_Roms_Base
    {
        [TestMethod]
        public void Instructions_Timing_Test_ROM()
        {
            RunTestRom(Path.Combine(GetTestRomsDirectory(), "instr_timing.gb"));
        }

        protected override string GetTestRomsDirectory()
        {
            return Path.Combine(base.GetTestRomsDirectory(), "instr_timing");
        }
    }
}
