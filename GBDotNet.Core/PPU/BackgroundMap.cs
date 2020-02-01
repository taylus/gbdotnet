namespace GBDotNet.Core
{
    public class BackgroundMap : TileMap
    {
        public override int BaseAddress => Registers.LCDControl.BackgroundTileMapBaseAddress;

        public BackgroundMap(PPURegisters registers, TileSet tileset, IMemory vram) : base(registers, tileset, vram)
        {

        }
    }
}
