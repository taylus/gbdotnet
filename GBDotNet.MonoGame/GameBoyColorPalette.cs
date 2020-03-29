using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MonoGameBoy
{
    public class GameBoyColorPalette
    {
        public IList<Color> Colors { get; private set; }

        public GameBoyColorPalette(params Color[] colors)
        {
            Colors = colors;
        }

        public GameBoyColorPalette(params string[] hexColors)
        {
            Colors = hexColors.Select(c => FromHex(c)).ToList();
        }

        public Color this[int i]
        {
            get { return Colors[i]; }
            set { Colors[i] = value; }
        }

        /// <summary>
        /// The original Game Boy (aka DMG for "Dot Matrix Game") greenscale color palette.
        /// </summary>
        public static GameBoyColorPalette Dmg => new GameBoyColorPalette(new Color(224, 248, 208),
            new Color(136, 192, 112), new Color(52, 104, 86), new Color(8, 24, 32));

        /// <summary>
        /// The Game Boy Pocket grayscale color palette.
        /// </summary>
        public static GameBoyColorPalette Pocket = new GameBoyColorPalette(new Color(232, 232, 232),
            new Color(160, 160, 160), new Color(88, 88, 88), new Color(16, 16, 16));

        private static Color FromHex(string hexString)
        {
            if (hexString.StartsWith("#")) hexString = hexString.Substring(1);
            if (hexString.Length != 6) throw new ArgumentException("RGB color must be 6 hex digits.", nameof(hexString));
            uint hex = uint.Parse(hexString, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            return new Color
            {
                R = (byte)(hex >> 16),
                G = (byte)(hex >> 8),
                B = (byte)hex,
                A = byte.MaxValue
            };
        }
    }
}
