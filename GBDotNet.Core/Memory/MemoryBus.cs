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

        //TODO: joypad: http://bgb.bircd.org/pandocs.htm#joypadinput
        private byte joypadPort;        //$FF00

        //TODO: game link cable: http://bgb.bircd.org/pandocs.htm#serialdatatransferlinkcable
        private byte serialData;        //$FF01
        private byte serialControl;     //$FF02

        //TODO: audio
        private readonly SoundRegisters soundRegisters = new SoundRegisters();

        //graphics
        public TileSet TileSet { get; set; }
        public IMemory VideoMemory { get; set; } = new Memory();
        public IMemory ObjectAttributeMemory { get; set; } = new Memory();
        public PPURegisters PPURegisters { get; set; }
        
        //interrupts
        public InterruptFlags InterruptFlags { get; } = new InterruptFlags();
        public InterruptEnable InterruptEnable { get; } = new InterruptEnable();

        public byte[] Data
        {
            get
            {
                var bytes = new byte[Memory.Size];
                for(int i = 0; i < Memory.Size; i++)
                {
                    try
                    {
                        bytes[i] = this[i];
                    }
                    catch
                    {
                        //ignore non-implemented memory regions
                        bytes[i] = 0xFF;
                    }
                }
                return bytes;
            }
        }

        private readonly Random random = new Random();

        public MemoryBus(PPURegisters ppuRegisters)
        {
            PPURegisters = ppuRegisters;
            //Randomize(wram);
            //Randomize(zram);
            //Randomize(VideoMemory);
            //Randomize(ObjectAttributeMemory);
            TileSet = new TileSet(VideoMemory);
        }

        private void Randomize(IMemory memory)
        {
            for(int i = 0; i < Memory.Size; i++)
            {
                memory[i] = (byte)random.Next();
            }
        }

        public void LoadRom(RomFile rom)
        {
            this.rom = rom;
        }

        public void LoadVideoMemory(Memory vram)
        {
            VideoMemory = vram;
        }

        public byte this[int address]
        {
            get
            {
                if (address < 0)
                {
                    throw new ArgumentOutOfRangeException("Address cannot be negative.");
                }
                else if (address < 0x4000)
                {
                    return rom[address];  //ROM bank 0 (16K)
                }
                else if (address < 0x8000)
                {
                    return rom[address];  //ROM bank 1 (16K) -- TODO: bank switching
                }
                else if (address < 0xA000)
                {
                    //VRAM (8K)
                    return VideoMemory[address - 0x8000];
                }
                else if (address < 0xC000)
                {
                    //external RAM (8K)
                    throw new NotImplementedException($"Unsupported read of address ${address:X4}: Cartridge RAM is not yet implemented.");
                }
                else if (address < 0xE000)
                {
                    //internal work RAM (8K)
                    return wram[address - 0xC000];
                }
                else if (address < 0xFE00)
                {
                    //internal work RAM shadow
                    return wram[address - 0xE000];
                }
                else if (address < 0xFEA0)
                {
                    //sprite OAM (160 bytes)
                    return ObjectAttributeMemory[address - 0xFE00];
                }
                else if (address < 0xFF00)
                {
                    //unusable memory range, should always read as zero
                    return 0;
                }
                else if (address < 0xFF80)
                {
                    //various hardware I/O registers (PPU, APU, joypad, etc)
                    if (address == 0xFF00) return joypadPort;
                    else if (address == 0xFF01) return serialData;
                    else if (address == 0xFF02) return serialControl;
                    else if (address == 0xFF0F) return InterruptFlags.Data;
                    else if (soundRegisters.MappedToAddress(address)) return soundRegisters[address];
                    else if (address == 0xFF40) return PPURegisters.LCDControl.Data;
                    else if (address == 0xFF41) return PPURegisters.LCDStatus.Data;
                    else if (address == 0xFF42) return PPURegisters.ScrollY;
                    else if (address == 0xFF43) return PPURegisters.ScrollX;
                    else if (address == 0xFF44) return PPURegisters.CurrentScanline;
                    else if (address == 0xFF45) return PPURegisters.CompareScanline;
                    else if (address == 0xFF47) return PPURegisters.BackgroundPalette.Data;
                    else if (address == 0xFF48) return PPURegisters.SpritePalette0.Data;
                    else if (address == 0xFF49) return PPURegisters.SpritePalette1.Data;
                    else if (address == 0xFF4A) return PPURegisters.WindowY;
                    else if (address == 0xFF4B) return PPURegisters.WindowX;
                    else throw new NotImplementedException($"Unsupported read of address ${address:X4} (not all hardware I/O registers are implemented yet).");
                }
                else if (address < 0xFFFF)
                {
                    //zero page (128 bytes)
                    return zram[address - 0xFF80];
                }
                else if (address == 0xFFFF)
                {
                    return InterruptEnable.Data;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Address must be between $0000 and $FFFF");
                }
            }

            set
            {
                if (address < 0)
                {
                    throw new ArgumentOutOfRangeException("Address cannot be negative.");
                }
                else if (address < 0x8000)
                {
                    //TODO: memory bank controllers
                    //why does Tetris write here when it doesn't use one?
                    //see: https://www.reddit.com/r/EmuDev/comments/5ht388/gb_why_does_tetris_write_to_the_rom/
                }
                else if (address >= 0x8000 && address < 0xA000)
                {
                    //VRAM (8K)
                    var vramAddress = address - 0x8000;
                    bool tilesetDirty = address < 0x9800 && VideoMemory[vramAddress] != value;
                    VideoMemory[vramAddress] = value;
                    if (tilesetDirty) TileSet.UpdateFromMemoryWrite(VideoMemory, vramAddress);
                }
                else if (address < 0xC000)
                {
                    //external RAM (8K)
                    throw new NotImplementedException($"Unsupported write to address ${address:X4}: Cartridge RAM is not yet implemented.");
                }
                else if (address < 0xE000)
                {
                    //internal work RAM (8K)
                    wram[address - 0xC000] = value;
                }
                else if (address < 0xFE00)
                {
                    //internal work RAM shadow
                    wram[address - 0xE000] = value;
                }
                else if (address < 0xFEA0)
                {
                    //sprite OAM (160 bytes)
                    ObjectAttributeMemory[address - 0xFE00] = value;
                }
                else if (address < 0xFF00)
                {
                    //unusable memory, but same games (like Tetris) write here (by mistake?)... so don't crash
                    //see: https://www.reddit.com/r/EmuDev/comments/5nixai/gb_tetris_writing_to_unused_memory/
                }
                else if (address < 0xFF80)
                {
                    //various hardware I/O registers (PPU, APU, joypad, etc)
                    if (address == 0xFF00) joypadPort = value;
                    else if (address == 0xFF01) serialData = value;
                    else if (address == 0xFF02) serialControl = value;
                    else if (address == 0xFF0F) InterruptFlags.Data = value;
                    else if (soundRegisters.MappedToAddress(address)) soundRegisters[address] = value;
                    else if (address == 0xFF40) PPURegisters.LCDControl.Data = value;
                    else if (address == 0xFF41) PPURegisters.LCDStatus.Data = value;
                    else if (address == 0xFF42) PPURegisters.ScrollY = value;
                    else if (address == 0xFF43) PPURegisters.ScrollX = value;
                    else if (address == 0xFF44) PPURegisters.CurrentScanline = value;
                    else if (address == 0xFF45) PPURegisters.CompareScanline = value;
                    else if (address == 0xFF47) PPURegisters.BackgroundPalette.Data = value;
                    else if (address == 0xFF48) PPURegisters.SpritePalette0.Data = value;
                    else if (address == 0xFF49) PPURegisters.SpritePalette1.Data = value;
                    else if (address == 0xFF4A) PPURegisters.WindowY = value;
                    else if (address == 0xFF4B) PPURegisters.WindowX = value;
                    //else throw new NotImplementedException($"Unsupported write to address ${address:X4} (not all hardware I/O registers are implemented yet).");
                }
                else if (address < 0xFFFF)
                {
                    //zero page (128 bytes)
                    zram[address - 0xFF80] = value;
                }
                else if (address == 0xFFFF)
                {
                    InterruptEnable.Data = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException($"Unexpected write to address ${address:X4}");
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
