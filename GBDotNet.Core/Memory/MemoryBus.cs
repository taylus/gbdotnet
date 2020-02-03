using System;
using System.Collections;
using System.Collections.Generic;

namespace GBDotNet.Core
{
    /// <summary>
    /// Implements the Game Boy's address space, routing addresses to
    /// RAM, ROM, or I/O devices according to the Game Boy's memory map.
    /// </summary>
    /// <see cref="http://gameboy.mongenel.com/dmg/asmmemmap.html"/>
    public class MemoryBus : IMemory
    {
        private RomFile rom;
        private readonly IMemory wram = new Memory();
        private readonly IMemory zram = new Memory();
        private byte interruptEnableFlag = 0;

        public IMemory Vram { get; set; } = new Memory();
        public PPURegisters PPURegisters { get; set; }

        public void LoadRom(RomFile rom)
        {
            this.rom = rom;
        }

        public void LoadVram(Memory vram)
        {
            Vram = vram;
        }

        public byte this[int index]
        {
            get
            {
                if (index < 0)
                {
                    throw new ArgumentOutOfRangeException("Address cannot be negative.");
                }
                if (index < 0x4000)
                {
                    return rom[index];  //ROM bank 0 (16K)
                }
                else if (index < 0x8000)
                {
                    return rom[index];  //ROM bank 1 (16K) -- TODO: bank switching
                }
                else if (index < 0xA000)
                {
                    //VRAM (8K)
                    return Vram[index - 0x8000];
                }
                else if (index < 0xC000)
                {
                    //external RAM (8K)
                    throw new NotImplementedException($"Unsupported read of address ${index:X4}: Cartridge RAM is not yet implemented.");
                }
                else if (index < 0xE000)
                {
                    //internal work RAM (8K)
                    return wram[index - 0xC000];
                }
                else if (index < 0xFE00)
                {
                    //internal work RAM shadow
                    return wram[index - 0xE000];
                }
                else if (index < 0xFEA0)
                {
                    //sprite OAM (160 bytes)
                    throw new NotImplementedException($"Unsupported read of address ${index:X4}: Sprite OAM is not yet implemented.");
                }
                else if (index < 0xFF00)
                {
                    //unusable memory range, should always read as zero
                    return 0;
                }
                else if (index < 0xFF80)
                {
                    //various hardware I/O registers (PPU, APU, joypad, etc)
                    if (index == 0xFF40) return PPURegisters.LCDControl.Data;
                    else if (index == 0xFF41) return PPURegisters.LCDStatus.Data;
                    else if (index == 0xFF42) return PPURegisters.ScrollY;
                    else if (index == 0xFF43) return PPURegisters.ScrollX;
                    else if (index == 0xFF44) return PPURegisters.CurrentScanline;
                    else if (index == 0xFF45) return PPURegisters.CompareScanline;
                    else if (index == 0xFF47) return PPURegisters.BackgroundPalette.Data;
                    else if (index == 0xFF48) return PPURegisters.SpritePalette0.Data;
                    else if (index == 0xFF49) return PPURegisters.SpritePalette1.Data;
                    else if (index == 0xFF4A) return PPURegisters.WindowY;
                    else if (index == 0xFF4B) return PPURegisters.WindowX;
                    throw new NotImplementedException($"Unsupported read of address ${index:X4}: Hardware I/O registers are not yet implemented.");
                }
                else if (index < 0xFFFF)
                {
                    //zero page (128 bytes)
                    return zram[index - 0xFF80];
                }
                else if (index == 0xFFFF)
                {
                    return interruptEnableFlag;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Address must be between $0000 and $FFFF");
                }
            }

            set
            {
                if (index >= 0x8000 && index < 0xA000)
                {
                    //VRAM (8K)
                    Vram[index - 0x8000] = value;
                }
                else if (index < 0xC000)
                {
                    //external RAM (8K)
                    throw new NotImplementedException($"Unsupported write to address ${index:X4}: Cartridge RAM is not yet implemented.");
                }
                else if (index < 0xE000)
                {
                    //internal work RAM (8K)
                    wram[index - 0xC000] = value;
                }
                else if (index < 0xFE00)
                {
                    //internal work RAM shadow
                    wram[index - 0xE000] = value;
                }
                else if (index < 0xFEA0)
                {
                    //sprite OAM (160 bytes)
                    throw new NotImplementedException($"Unsupported write to $${index:X4}: Sprite OAM is not yet implemented.");
                }
                else if (index < 0xFF80)
                {
                    //various hardware I/O registers (PPU, APU, joypad, etc)
                    if (index == 0xFF40) PPURegisters.LCDControl.Data = value;
                    else if (index == 0xFF41) PPURegisters.LCDStatus.Data = value;
                    else if (index == 0xFF42) PPURegisters.ScrollY = value;
                    else if (index == 0xFF43) PPURegisters.ScrollX = value;
                    else if (index == 0xFF44) PPURegisters.CurrentScanline = value;
                    else if (index == 0xFF45) PPURegisters.CompareScanline = value;
                    else if (index == 0xFF47) PPURegisters.BackgroundPalette.Data = value;
                    else if (index == 0xFF48) PPURegisters.SpritePalette0.Data = value;
                    else if (index == 0xFF49) PPURegisters.SpritePalette1.Data = value;
                    else if (index == 0xFF4A) PPURegisters.WindowY = value;
                    else if (index == 0xFF4B) PPURegisters.WindowX = value;
                    throw new NotImplementedException($"Unsupported write to address ${index:X4}: Hardware I/O registers are not yet implemented.");
                }
                else if (index < 0xFFFF)
                {
                    //zero page (128 bytes)
                    zram[index - 0xFF80] = value;
                }
                else if (index == 0xFFFF)
                {
                    interruptEnableFlag = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException($"Unexpected write to address ${index:X4}");
                }
            }
        }

        public IEnumerator<byte> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
