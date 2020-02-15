namespace GBDotNet.Core
{
    public class BackgroundMap : TileMap
    {
        public override int BaseAddress => PPU.Registers.LCDControl.BackgroundTileMapBaseAddress;

        public BackgroundMap(PPU ppu) : base(ppu)
        {

        }

        public override byte[] Render()
        {
            if (!PPU.Registers.LCDControl.BackgroundDisplayEnabled)
                return new byte[WidthInPixels * HeightInPixels];

            return base.Render();
        }
    }
}
