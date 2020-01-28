using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GBDotNet.Core.Test.Integration
{
    [TestClass]
    public partial class PPU_Should
    {
        // Note: if these tests fail, they will report the index # in the byte arrays that differ.
        // To translate that back into 2D pixel coordinates, use this formula: (x = i % width, y = i / width)
        // Maybe write my own pixel array comparison routine that does this, instead of using CollectionAssert.AreEqual()...?

        [TestMethod]
        public void Generate_Blank_Screen_When_LCD_Is_Disabled()
        {
            //LCDC bit 7
            Assert.Inconclusive("Test not yet implemented.");
        }
    }
}
