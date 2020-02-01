﻿namespace GBDotNet.Core
{
    public class Window : TileMap
    {
        public override int BaseAddress => Registers.LCDControl.WindowTileMapBaseAddress;

        public Window(PPURegisters registers, TileSet tileset, IMemory vram) : base(registers, tileset, vram)
        {

        }
    }
}
