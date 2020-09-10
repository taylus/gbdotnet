using System;
using System.Linq;

namespace GBDotNet.Core
{
    public class MBC1 : Cartridge
    {
        private const int MaxRomBanks = 125;     //MBC1 can address 32 ROM banks (in "RAM mode") or 125 ROM banks (in "ROM mode")
        private const int MaxRamBanks = 4;       //MBC1 can have 2KB, 8KB, or 32KB of RAM (latter is 4 banks switched in "RAM mode")

        private byte selectedRomBankNumber = 1;
        private ArraySegment<byte> SelectedRomBank => GetRomBank(selectedRomBankNumber);

        private byte selectedRamBankNumber;
        private ArraySegment<byte> SelectedRamBank => GetRamBank(selectedRamBankNumber);
        private readonly byte[] ram = Enumerable.Repeat<byte>(0xFF, count: MaxRamBanks * RamBankSize).ToArray();
        private bool isExternalRamEnabled;

        private Mode currentMode;

        public MBC1(byte[] data) : base(data)
        {

        }

        public MBC1(string path) : base(path)
        {

        }

        public override byte this[int address]
        {
            get
            {
                if (address < RomBankSize) return Data[address]; //ROM0
                else if (address < RomBankSize * 2) return SelectedRomBank[address - RomBankSize]; //ROMX
                else if (address >= 0xA000 && address < 0xC000) return isExternalRamEnabled ? SelectedRamBank[address - 0xA000] : (byte)0xFF;
                throw new ArgumentOutOfRangeException("Cartridge address must be between $0000-$7FFF (ROM) or $A000 - $BFFF (RAM)");
            }
            set
            {
                //intercept ROM writes as MBC commands
                if (address < 0x2000)
                {
                    isExternalRamEnabled = (value & 0x0F) == 0x0A;
                }
                else if (address < 0x4000)
                {
                    //select ROM bank's lower 5 bits
                    value = (byte)(value & 0b0001_1111);
                    if (value == 0) value = 1;  //MBC1 quirk/bug: 0 is treated as 1, making banks $20, $40, and $60 unusable
                    selectedRomBankNumber = (byte)((selectedRomBankNumber & 0b0110_0000) + value);
                }
                else if (address < 0x6000)
                {
                    if (currentMode == Mode.Rom)
                    {
                        //select ROM bank's upper 2 bits if in ROM mode
                        selectedRomBankNumber = (byte)((selectedRomBankNumber & 0b0001_1111) + ((value & 0b0011) << 5));
                    }
                    else if (currentMode == Mode.Ram)
                    {
                        //select RAM bank number if in RAM mode
                        selectedRamBankNumber = (byte)(value & 0b0011);
                    }
                }
                else if (address < 0x8000)
                {
                    if (value == 0) currentMode = Mode.Rom;
                    else if (value == 1) currentMode = Mode.Ram;
                }
                else if (address >= 0xA000 && address < 0xC000)
                {
                    ram[(selectedRamBankNumber * RamBankSize) + (address - 0xA000)] = value;
                }
            }
        }

        /// <summary>
        /// Returns the ROM bank with the given number.
        /// Game Boy ROMs are split up into 16KB banks.
        /// The first bank is always addressable from $0000 - $3FFF.
        /// The other banks are switched into range $4000 - $7FFF using a memory bank controller (MBC).
        /// </summary>
        /// <see cref="http://gameboy.mongenel.com/dmg/asmmemmap.html"/>
        public ArraySegment<byte> GetRomBank(int bankNumber)
        {
            if (bankNumber < NumberOfRomBanks) return new ArraySegment<byte>(Data, offset: bankNumber * RomBankSize, count: RomBankSize);
            throw new ArgumentException($"Bank number {bankNumber} exceeds number of banks in ROM ({NumberOfRomBanks}).", nameof(bankNumber));
        }

        public ArraySegment<byte> GetRamBank(int bankNumber)
        {
            if (bankNumber < MaxRamBanks) return new ArraySegment<byte>(ram, offset: bankNumber * RamBankSize, count: RamBankSize);
            throw new ArgumentException($"Bank number {bankNumber} exceeds number of banks in RAM ({MaxRamBanks}).", nameof(bankNumber));
        }

        private enum Mode
        {
            Rom = 0,    //~2MB ROM (125x16KB banks)/8KB (1 bank) RAM
            Ram = 1     //512KB ROM (32x16KB banks)/32KB (4x8KB banks) RAM
        }

        public override string ToString() => $"ROM{selectedRomBankNumber:X} RAM{selectedRamBankNumber:X}";
    }
}
