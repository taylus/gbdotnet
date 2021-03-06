﻿using System;
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
        private Cartridge cart;
        private readonly IMemory wram = new Memory();
        private readonly IMemory zram = new Memory();

        //input
        public JoypadRegister JoypadRegister { get; private set; } = new JoypadRegister();  //$FF00

        //TODO: game link cable: http://bgb.bircd.org/pandocs.htm#serialdatatransferlinkcable
        private byte serialData;        //$FF01
        private byte serialControl;     //$FF02
        private GameLinkConsole gameLinkConsole;

        //TODO: audio
        private readonly SoundRegisters soundRegisters = new SoundRegisters();

        //graphics
        public TileSet TileSet { get; set; }
        public SpriteOam SpriteOam { get; set; }
        public IMemory VideoMemory { get; set; } = new Memory();
        public IMemory ObjectAttributeMemory { get; set; } = new Memory();
        public PPURegisters PPURegisters { get; set; }

        //interrupts
        public InterruptFlags InterruptFlags { get; } = new InterruptFlags();
        public InterruptEnable InterruptEnable { get; } = new InterruptEnable();
        public Timer Timer { get; }

        public bool IsBootRomMapped { get; set; } = true;
        private static readonly byte[] bootRom = new byte[]
        {
            0x31, 0xFE, 0xFF, 0xAF, 0x21, 0xFF, 0x9F, 0x32, 0xCB, 0x7C, 0x20, 0xFB, 0x21, 0x26, 0xFF, 0x0E,
            0x11, 0x3E, 0x80, 0x32, 0xE2, 0x0C, 0x3E, 0xF3, 0xE2, 0x32, 0x3E, 0x77, 0x77, 0x3E, 0xFC, 0xE0,
            0x47, 0x11, 0x04, 0x01, 0x21, 0x10, 0x80, 0x1A, 0xCD, 0x95, 0x00, 0xCD, 0x96, 0x00, 0x13, 0x7B,
            0xFE, 0x34, 0x20, 0xF3, 0x11, 0xD8, 0x00, 0x06, 0x08, 0x1A, 0x13, 0x22, 0x23, 0x05, 0x20, 0xF9,
            0x3E, 0x19, 0xEA, 0x10, 0x99, 0x21, 0x2F, 0x99, 0x0E, 0x0C, 0x3D, 0x28, 0x08, 0x32, 0x0D, 0x20,
            0xF9, 0x2E, 0x0F, 0x18, 0xF3, 0x67, 0x3E, 0x64, 0x57, 0xE0, 0x42, 0x3E, 0x91, 0xE0, 0x40, 0x04,
            0x1E, 0x02, 0x0E, 0x0C, 0xF0, 0x44, 0xFE, 0x90, 0x20, 0xFA, 0x0D, 0x20, 0xF7, 0x1D, 0x20, 0xF2,
            0x0E, 0x13, 0x24, 0x7C, 0x1E, 0x83, 0xFE, 0x62, 0x28, 0x06, 0x1E, 0xC1, 0xFE, 0x64, 0x20, 0x06,
            0x7B, 0xE2, 0x0C, 0x3E, 0x87, 0xE2, 0xF0, 0x42, 0x90, 0xE0, 0x42, 0x15, 0x20, 0xD2, 0x05, 0x20,
            0x4F, 0x16, 0x20, 0x18, 0xCB, 0x4F, 0x06, 0x04, 0xC5, 0xCB, 0x11, 0x17, 0xC1, 0xCB, 0x11, 0x17,
            0x05, 0x20, 0xF5, 0x22, 0x23, 0x22, 0x23, 0xC9, 0xCE, 0xED, 0x66, 0x66, 0xCC, 0x0D, 0x00, 0x0B,
            0x03, 0x73, 0x00, 0x83, 0x00, 0x0C, 0x00, 0x0D, 0x00, 0x08, 0x11, 0x1F, 0x88, 0x89, 0x00, 0x0E,
            0xDC, 0xCC, 0x6E, 0xE6, 0xDD, 0xDD, 0xD9, 0x99, 0xBB, 0xBB, 0x67, 0x63, 0x6E, 0x0E, 0xEC, 0xCC,
            0xDD, 0xDC, 0x99, 0x9F, 0xBB, 0xB9, 0x33, 0x3E, 0x3C, 0x42, 0xB9, 0xA5, 0xB9, 0xA5, 0x42, 0x3C,
            0x21, 0x04, 0x01, 0x11, 0xA8, 0x00, 0x1A, 0x13, 0xBE, 0x20, 0xFE, 0x23, 0x7D, 0xFE, 0x34, 0x20,
            0xF5, 0x06, 0x19, 0x78, 0x86, 0x23, 0x05, 0x20, 0xFB, 0x86, 0x20, 0xFE, 0x3E, 0x01, 0xE0, 0x50
        };

        public byte[] Data
        {
            get
            {
                var bytes = new byte[Memory.Size];
                for (int i = 0; i < Memory.Size; i++)
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
            SpriteOam = new SpriteOam(ppuRegisters, ObjectAttributeMemory, TileSet);
            Timer = new Timer(this);
        }

        public void Reset()
        {
            IsBootRomMapped = true;
        }

        public void Tick(int elapsedCycles)
        {
            Timer.Tick(elapsedCycles);
        }

        private void Randomize(IMemory memory)
        {
            for (int i = 0; i < Memory.Size; i++)
            {
                memory[i] = (byte)random.Next();
            }
        }

        public void Load(Cartridge cart)
        {
            this.cart = cart;
        }

        public void LoadVideoMemory(Memory vram)
        {
            VideoMemory = vram;
        }

        public void Attach(GameLinkConsole gameLinkConsole)
        {
            this.gameLinkConsole = gameLinkConsole;
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
                    if (address < 0x100 && IsBootRomMapped) return bootRom[address];
                    return cart[address];  //ROM bank 0 (16K)
                }
                else if (address < 0x8000)
                {
                    return cart[address];  //ROM bank 1-N (16K, switchable if cart has an MBC)
                }
                else if (address < 0xA000)
                {
                    //VRAM (8K)
                    return VideoMemory[address - 0x8000];
                }
                else if (address < 0xC000)
                {
                    //external RAM (8K)
                    return cart[address - 0xA000];
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
                    if (address == 0xFF00) return JoypadRegister.Read();
                    else if (address == 0xFF01) return serialData;
                    else if (address == 0xFF02) return serialControl;
                    else if (Timer.MappedToAddress(address)) return Timer[address];
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
                    else return 0xFF;
                    //else throw new NotImplementedException($"Unsupported read of address ${address:X4} (not all hardware I/O registers are implemented yet).");
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
                    //memory bank controllers intercept ROM writes and use them for bank switching and etc
                    cart[address] = value;

                    //why does Tetris write here when it doesn't use one?
                    //see: https://www.reddit.com/r/EmuDev/comments/5ht388/gb_why_does_tetris_write_to_the_rom/
                }
                else if (address >= 0x8000 && address < 0xA000)
                {
                    //VRAM (8K)
                    var vramAddress = address - 0x8000;
                    bool isTilesetDirty = address < 0x9800 && VideoMemory[vramAddress] != value;
                    VideoMemory[vramAddress] = value;
                    if (isTilesetDirty) TileSet.UpdateFromMemoryWrite(VideoMemory, vramAddress);
                }
                else if (address < 0xC000)
                {
                    //external RAM (8K)
                    cart[address - 0xA000] = value;
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
                    var oamAddress = address - 0xFE00;
                    bool isOamDirty = ObjectAttributeMemory[oamAddress] != value;
                    ObjectAttributeMemory[address - 0xFE00] = value;
                    if (isOamDirty) SpriteOam.UpdateFromMemoryWrite(ObjectAttributeMemory, oamAddress);
                }
                else if (address < 0xFF00)
                {
                    //unusable memory, but same games (like Tetris) write here (by mistake?)... so don't crash
                    //see: https://www.reddit.com/r/EmuDev/comments/5nixai/gb_tetris_writing_to_unused_memory/
                }
                else if (address < 0xFF80)
                {
                    //various hardware I/O registers (PPU, APU, joypad, etc)
                    if (address == 0xFF00) JoypadRegister.Write(value);
                    else if (address == 0xFF01)
                    {
                        serialData = value;
                        gameLinkConsole?.Print(value);
                    }
                    else if (address == 0xFF02) serialControl = value;
                    else if (Timer.MappedToAddress(address)) Timer[address] = value;
                    else if (address == 0xFF0F) InterruptFlags.Data = (byte)(value | 0b1110_0000);  //upper 3 bits are unused and always read as 1
                    else if (soundRegisters.MappedToAddress(address)) soundRegisters[address] = value;
                    else if (address == 0xFF40) PPURegisters.LCDControl.Data = value;
                    else if (address == 0xFF41) PPURegisters.LCDStatus.Data = value;
                    else if (address == 0xFF42) PPURegisters.ScrollY = value;
                    else if (address == 0xFF43) PPURegisters.ScrollX = value;
                    else if (address == 0xFF44) PPURegisters.CurrentScanline = value;
                    else if (address == 0xFF45) PPURegisters.CompareScanline = value;
                    else if (address == 0xFF46) OamDmaTransfer(value);
                    else if (address == 0xFF47) PPURegisters.BackgroundPalette.Data = value;
                    else if (address == 0xFF48) PPURegisters.SpritePalette0.Data = value;
                    else if (address == 0xFF49) PPURegisters.SpritePalette1.Data = value;
                    else if (address == 0xFF50) IsBootRomMapped = false;
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

        /// <summary>
        /// Performs a DMA (direct memory access) transfer of the 160 bytes in
        /// memory at the written value (times $100) into sprite OAM at $FE00.
        /// </summary>
        /// <param name="value">The upper byte of the source address.</param>
        /// <remarks>
        /// Sources say this takes 4 setup cycles + 160 x 4 cycles (for each byte copied).
        /// Does accurate timing/writing 1 byte every 4 cycles matter? Look into this later:
        /// https://www.reddit.com/r/EmuDev/comments/8b5wvr/gb_what_game_boy_games_rely_on_correct_dma_timing/
        /// </remarks>
        /// <see cref="https://github.com/AntonioND/giibiiadvance/blob/master/docs/TCAGBD.pdf"/>
        private void OamDmaTransfer(byte value)
        {
            var sourceAddress = value << 8;
            for (int i = 0; i < SpriteOam.SizeInBytes; i++)
            {
                this[0xFE00 + i] = this[sourceAddress + i];
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
