namespace GBDotNet.Core
{
    public class Window : TileMap
    {
        public const int OffsetX = 7;

        public override int BaseAddress => Registers.LCDControl.WindowTileMapBaseAddress;

        public Window(PPURegisters registers, TileSet tileset, IMemory vram) : base(registers, tileset, vram)
        {

        }

        public override byte[] Render()
        {
            if (!Registers.LCDControl.WindowDisplayEnabled)
                return new byte[WidthInPixels * HeightInPixels];

            return base.Render();
        }

        public void DrawOntoScanline(ref byte[] screenPixels, int x, int y)
        {
            if (!Registers.LCDControl.WindowDisplayEnabled) return;
            if (!OverlapsCoordinates(x, y)) return;
            var windowX = (byte)(x + Registers.WindowX - OffsetX);
            var windowY = (byte)(y + Registers.WindowY);
            screenPixels[y * PPU.ScreenWidthInPixels + x] = GetPixelAt(windowX, windowY);
        }

        private bool OverlapsColumn(int x) => (x + OffsetX >= Registers.WindowX) && (x + OffsetX < (Registers.WindowX + WidthInPixels));
        private bool OverlapsScanline(int y) => (y >= Registers.WindowY) && (y < (Registers.WindowY + HeightInPixels));
        private bool OverlapsCoordinates(int x, int y) => OverlapsColumn(x) && OverlapsScanline(y);
    }
}
