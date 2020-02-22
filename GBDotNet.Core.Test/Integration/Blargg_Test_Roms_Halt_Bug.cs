using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GBDotNet.Core.Test.Integration
{
    [TestClass]
    public class Blargg_Test_Roms_Halt_Bug : Blargg_Test_Roms_Base
    {
        [TestMethod]
        public void Halt_Bug_Test_ROM()
        {
            RunHaltBugTestRom(Path.Combine(GetTestRomsDirectory(), "halt_bug.gb"));
        }
    }
}
