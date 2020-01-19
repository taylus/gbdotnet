using System;

namespace GBDotNet.Core
{
    public class RomFile : Memory, IMemory
    {
        public const int BankSize = 0x4000;
        public int NumberOfBanks => Math.Max(1, data.Length / BankSize);
        public bool HasHeader { get; set; }

        /// <summary>
        /// Header section of the ROM containing metadata about the game.
        /// </summary>
        public ArraySegment<byte> Header => new ArraySegment<byte>(data, offset: 0x104, count: 0x4C);

        public override byte this[int index]
        {
            get => data[index];
            set => throw new NotImplementedException("Cannot write to read-only memory.");
        }

        /// <summary>
        /// Returns true if the given address falls within the ROM header, false otherwise.
        /// </summary>
        public bool IsInHeader(int address) => (HasHeader && address >= Header.Offset && address < (Header.Offset + Header.Count));

        /// <summary>
        /// Loads a ROM from the given data.
        /// </summary>
        public RomFile(byte[] data) : base(data)
        {

        }

        /// <summary>
        /// Loads a ROM file from the given path.
        /// </summary>
        public RomFile(string path) : base(path)
        {

        }

        /// <summary>
        /// Returns the ROM bank with the given number.
        /// Game Boy ROMs are split up into 16KB banks.
        /// The first bank is always addressable from $0000 - $3FFF.
        /// The other banks are switched into range $4000 - $7FFF using a memory management controller (MMC).
        /// </summary>
        /// <see cref="http://gameboy.mongenel.com/dmg/asmmemmap.html"/>
        public ArraySegment<byte> GetBank(int bankNumber)
        {
            if (bankNumber < NumberOfBanks) throw new ArgumentException("Bank number exceeds number of banks in ROM.", nameof(bankNumber));
            return new ArraySegment<byte>(data, offset: bankNumber * BankSize, count: BankSize);
        }
    }
}
