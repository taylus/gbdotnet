﻿using System;
using System.Collections.Generic;

namespace GBDotNet.Core
{
    /// <summary>
    /// A single 8 x 8 pixel tile. At 2 bits per pixel, tiles are 16 bytes long,
    /// with each consecutive pair of bytes being the low and high bits of a row, respectively.
    /// </summary>
    /// <see cref="https://www.huderlem.com/demos/gameboy2bpp.html"/>
    public class Tile
    {
        public const int WidthInPixels = 8;
        public const int HeightInPixels = 8;
        public const int BytesPerTile = 16;

        public byte[] Pixels { get; } = new byte[WidthInPixels * HeightInPixels];

        public byte this[int x, int y]
        {
            //use "row major order" (rows stored contiguously) since that's what MonoGame does
            get => Pixels[y * WidthInPixels + x];
            set => Pixels[y * WidthInPixels + x] = value;
        }

        public Tile(IList<byte> bytes)
        {
            if (bytes.Count != BytesPerTile)
                throw new ArgumentException($"Tile data must be {BytesPerTile} bytes.");

            //parse bytes into pixels one row (two bytes) at a time
            for (int y = 0; y < HeightInPixels; y++)
            {
                byte lowByte = bytes[2 * y];
                byte highByte = bytes[2 * y + 1];

                //each pixel in each row is the corresponding bit of the above bytes combined, e.g.
                //pixel 0 is the 7th of the high byte and the 7th of the low byte,
                //pixel 1 is the 6th bit of the high byte and the 6th bit of the low byte,
                //...
                //pixel 7 is the 0th bit of the high byte and the 0th bit of the low byte
                for (int x = 0; x < WidthInPixels; x++)
                {
                    int highBit = (highByte & (0b10000000 >> x)) > 0 ? 1 : 0;
                    int lowBit = (lowByte & (0b10000000 >> x)) > 0 ? 1 : 0;
                    this[x, y] = (byte)(2 * highBit + lowBit);
                }
            }
        }
    }
}
