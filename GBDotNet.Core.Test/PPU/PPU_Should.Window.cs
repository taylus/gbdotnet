using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GBDotNet.Core.Test.Integration
{
    public partial class PPU_Should
    {
        [TestMethod]
        public void Generate_Expected_Window_Pixels_From_Known_VRAM_Dump()
        {
            //TODO: signed/unsigned test variants?
            //other variants? (see LCDC flags)
            Assert.Inconclusive("Test not yet implemented.");
        }

        [TestMethod]
        public void Generate_Blank_Window_Pixels_When_Window_Drawing_Is_Disabled()
        {
            //LCDC bit 5
            Assert.Inconclusive("Test not yet implemented.");
        }
    }
}
