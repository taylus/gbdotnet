using System;
using System.IO;
using System.Linq;

namespace GBDotNet.Core
{
    public class Cartridge : Memory, IMemory
    {
        public const int RamBankSize = 0x2000;
        public const int RomBankSize = 0x4000;
        public int NumberOfRomBanks => Math.Max(1, Data.Length / RomBankSize);
        public bool HasHeader { get; set; }

        /// <summary>
        /// Header section of the ROM containing metadata about the game.
        /// </summary>
        public ArraySegment<byte> Header => new ArraySegment<byte>(Data.ToArray(), offset: 0x104, count: 0x4C);

        public override byte this[int index]
        {
            get => Data[index];
            set { }
        }

        /// <summary>
        /// Returns true if the given address falls within the ROM header, false otherwise.
        /// </summary>
        public bool IsInHeader(int address) => (HasHeader && address >= Header.Offset && address < (Header.Offset + Header.Count));

        /// <summary>
        /// Loads a ROM from the given data.
        /// </summary>
        public Cartridge(byte[] data)
        {
            Data = data;
        }

        /// <summary>
        /// Loads a ROM file from the given path.
        /// </summary>
        public Cartridge(string path)
        {
            Data = File.ReadAllBytes(path);
        }

        /// <summary>
        /// Loads a ROM w/ the appropriate memory bank controller (MBC) from the given data.
        /// </summary>
        public static Cartridge LoadFrom(string path)
        {
            var data = File.ReadAllBytes(path);
            var cartridgeType = data[0x147];
            if (cartridgeType == 0x01) return new MBC1(data);   //MBC1
            if (cartridgeType == 0x02) return new MBC1(data);   //MBC1+RAM
            if (cartridgeType == 0x03) return new MBC1(data);   //MBC1+RAM+BATTERY
            return new Cartridge(data);                         //ROM only
        }

        public override string ToString() => "";
    }
}
