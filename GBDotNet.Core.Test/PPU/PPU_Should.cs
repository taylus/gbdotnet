using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GBDotNet.Core.Test.Integration
{
    [TestClass]
    public partial class PPU_Should
    {
        [TestMethod]
        public void Generate_Blank_Screen_When_LCD_Is_Disabled()
        {
            //LCDC bit 7
            Assert.Inconclusive("Test not yet implemented.");
        }

        /// <summary>
        /// Like CollectionAssert.AreEqual(), but reports the 2D pixel coordinates of any difference.
        /// </summary>
        /// <param name="expectedPixels">An array of pixel colors (0-3) stored in row-major order.</param>
        /// <param name="actualPixels">An array of pixel colors (0-3) stored in row-major order.</param>
        /// <param name="width">The width of the 2D-as-1D pixel arrays</param>
        private static void AssertPixelsMatch(byte[] expectedPixels, byte[] actualPixels, int width)
        {
            Assert.AreEqual(expectedPixels.Length, actualPixels.Length, "Pixel arrays are not the same size.");

            for (int i = 0; i < expectedPixels.Length; i++)
            {
                Assert.AreEqual(expectedPixels[i], actualPixels[i], $"Pixels at ({i % width}, {i / width}) did not match.");
            }
        }
    }
}
