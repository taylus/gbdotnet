using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace GBDotNet.Core.Test
{
    public static class ImageHelper
    {
        private const int maxPaletteColors = 4;

        /// <summary>
        /// Extracts a 4-color palette from the image at the given path and returns
        /// a byte array in row-major order where each byte is the palette index of
        /// that pixel's color (sorted darkest to lightest by average RGB value).
        /// </summary>
        public static byte[] LoadImageAsPaletteIndexedByteArray(string path)
        {
            using var image = Image.Load<Rgba32>(path);
            var palette = GetPalette();
            if (palette.Count > maxPaletteColors)
            {
                throw new AssertFailedException($"Test setup error: {path} must be " +
                    $"{maxPaletteColors} colors or less, but {palette.Count} colors were found.");
            }
            return LoadImageAsPaletteIndexedByteArray(image, palette);
        }

        private static byte[] LoadImageAsPaletteIndexedByteArray(Image<Rgba32> image, IList<Rgba32> palette)
        {
            var bytes = new byte[image.Width * image.Height];
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    var color = image[x, y];
                    var paletteIndex = (byte)palette.IndexOf(color);
                    bytes[y * image.Width + x] = paletteIndex;
                }
            }
            return bytes;
        }

        private static IList<Rgba32> GetPalette()
        {
            //TODO: take in a palette byte for ordering, but for now
            //just hardcode what the Tetris title screen uses: 11|10|01|00 => light-to-dark
            //where each pair of bits are the colors numbered 3 - 0 (feels backwards, but that's how it ism)
            //and the color values are: 00 = lightest, 01 = light, 10 = dark, 11 = darkest
            //see: http://bgb.bircd.org/pandocs.htm#lcdmonochromepalettes
            return new List<Rgba32>()
            {
                new Rgba32(224, 248, 208),
                new Rgba32(136, 192, 112),
                new Rgba32(52, 104, 86),
                new Rgba32(8, 24, 32),
            };
        }
    }
}
