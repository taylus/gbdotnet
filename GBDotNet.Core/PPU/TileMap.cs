namespace GBDotNet.Core
{
    /// <summary>
    /// Tile maps are 32 x 32 (1K) blocks of tile numbers which define a background or window.
    /// </summary>
    /// <remarks>
    /// They exist in VRAM at address ranges $9800 - $9BFF and $9C00 - $9FFF and which map goes
    /// with the window and which goes with the background is determined by bits 6 and 3 of
    /// <see cref="LCDControlRegister"/>, respectively.
    /// </remarks>
    /// <see cref="http://gameboy.mongenel.com/dmg/asmmemmap.html"/>
    public class TileMap
    {
        public const int Size = 1024;
    }
}
