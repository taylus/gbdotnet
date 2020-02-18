using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GBDotNet.Core.Test.Integration
{
    [TestClass]
    public class Blargg_Test_Roms_CPU_Instructions : Blargg_Test_Roms_Base
    {
        [TestMethod]
        public void CPU_Instructions_Test_ROM_01()
        {
            RunTestRom(Path.Combine(GetTestRomsDirectory(), "01-special.gb"));
        }

        [TestMethod]
        public void CPU_Instructions_Test_ROM_02()
        {
            RunTestRom(Path.Combine(GetTestRomsDirectory(), "02-interrupts.gb"));
        }

        [TestMethod]
        public void CPU_Instructions_Test_ROM_03()
        {
            RunTestRom(Path.Combine(GetTestRomsDirectory(), "03-op sp,hl.gb"));
        }

        [TestMethod]
        public void CPU_Instructions_Test_ROM_04()
        {
            RunTestRom(Path.Combine(GetTestRomsDirectory(), "04-op r,imm.gb"));
        }

        [TestMethod]
        public void CPU_Instructions_Test_ROM_05()
        {
            RunTestRom(Path.Combine(GetTestRomsDirectory(), "05-op rp.gb"));
        }

        [TestMethod]
        public void CPU_Instructions_Test_ROM_06()
        {
            RunTestRom(Path.Combine(GetTestRomsDirectory(), "06-ld r,r.gb"));
        }

        [TestMethod]
        public void CPU_Instructions_Test_ROM_07()
        {
            RunTestRom(Path.Combine(GetTestRomsDirectory(), "07-jr,jp,call,ret,rst.gb"));
        }

        [TestMethod]
        public void CPU_Instructions_Test_ROM_08()
        {
            RunTestRom(Path.Combine(GetTestRomsDirectory(), "08-misc instrs.gb"));
        }

        [TestMethod]
        public void CPU_Instructions_Test_ROM_09()
        {
            RunTestRom(Path.Combine(GetTestRomsDirectory(), "09-op r,r.gb"));
        }

        [TestMethod]
        public void CPU_Instructions_Test_ROM_10()
        {
            RunTestRom(Path.Combine(GetTestRomsDirectory(), "10-bit ops.gb"));
        }

        [TestMethod, Ignore("Currently fails!")]
        public void CPU_Instructions_Test_ROM_11()
        {
            RunTestRom(Path.Combine(GetTestRomsDirectory(), "11-op a,(hl).gb"));
        }

        protected override string GetTestRomsDirectory()
        {
            return Path.Combine(base.GetTestRomsDirectory(), "cpu_instrs", "individual");
        }
    }
}
