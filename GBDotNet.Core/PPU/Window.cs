namespace GBDotNet.Core
{
    public class Window : TileMap
    {
        public const int OffsetX = 7;

        public override int BaseAddress => PPU.Registers.LCDControl.WindowTileMapBaseAddress;

        public Window(PPU ppu) : base(ppu)
        {

        }

        public override byte[] Render()
        {
            if (!PPU.Registers.LCDControl.WindowDisplayEnabled)
                return new byte[WidthInPixels * HeightInPixels];

            return base.Render();
        }

        public void DrawOntoScanline(ref byte[] screenPixels, int x, int y)
        {
            if (!PPU.Registers.LCDControl.WindowDisplayEnabled) return;
            if (!OverlapsCoordinates(x, y)) return;
            var windowX = (byte)(x - PPU.Registers.WindowX + OffsetX);
            var windowY = (byte)(y - PPU.Registers.WindowY);
            screenPixels[y * PPU.ScreenWidthInPixels + x] = GetPixelAt(windowX, windowY);
        }

        private bool OverlapsColumn(int x) => (x + OffsetX >= PPU.Registers.WindowX) && (x + OffsetX < (PPU.Registers.WindowX + WidthInPixels));
        private bool OverlapsScanline(int y) => (y >= PPU.Registers.WindowY) && (y < (PPU.Registers.WindowY + HeightInPixels));
        private bool OverlapsCoordinates(int x, int y) => OverlapsColumn(x) && OverlapsScanline(y);
    }
}
