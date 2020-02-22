using System;

namespace GBDotNet.Core
{
    public class MBC1 : RomFile
    {
        public const int NumRomBanks = 125;
        public const int MaxRamBanks = 4;   //MBC1 can have 1, 2, or 4 (switchable) RAM banks
        public const int TotalRomSize = BankSize * 125;

        public int SelectedRomBankNumber { get; private set; }
        public int SelectedRamBankNumber { get; private set; }

        public ArraySegment<byte> SelectedRomBank => GetBank(SelectedRomBankNumber);

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
                if (address < BankSize) return Data[address];
                else if (address < BankSize * 2) return SelectedRomBank[address - BankSize];
                throw new ArgumentOutOfRangeException("ROM address must be between $0000 and $7FFF");
            }
            set
            {
                //TODO: intercept writes as MBC controls
            }
        }
    }
}
