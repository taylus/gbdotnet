using System.Collections.Generic;
using System.Linq;
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
            using (var image = Image.Load<Rgba32>(path))
            {
                var palette = GetPalette(image);
                if (palette.Count > maxPaletteColors)
                {
                    throw new AssertFailedException($"Test setup error: {path} must be " +
                        $"{maxPaletteColors} colors or less, but {palette.Count} colors were found.");
                }
                return LoadImageAsPaletteIndexedByteArray(image, palette);
            }
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

        private static IList<Rgba32> GetPalette(Image<Rgba32> image)
        {
            var palette = new SortedSet<Rgba32>(new DarkToLight());
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    palette.Add(image[x, y]);
                }
            }
            return palette.ToList();
        }

        /// <summary>
        /// Sorts palette colors dark to light, so that color #0 is the darkest, color #1 is lighter, etc.
        /// Corresponds to a background palette data register ($FF47) of 11100100.
        /// </summary>
        /// <see cref="http://bgb.bircd.org/pandocs.htm#lcdmonochromepalettes"/>
        private class DarkToLight : IComparer<Rgba32>
        {
            public int Compare(Rgba32 a, Rgba32 b)
            {
                //a color is lighter if its average rgb value is less
                var avgA = (a.R + a.G + a.B) / 2;
                var avgB = (b.R + b.G + b.B) / 2;

                if (avgA < avgB) return 1;
                if (avgA > avgB) return -1;
                return 0;
            }
        }
    }
}
