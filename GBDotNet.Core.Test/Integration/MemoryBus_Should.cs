using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GBDotNet.Core.Test.Integration
{
    [TestClass]
    public class MemoryBus_Should
    {
        private const byte marker = 0xFF;

        [TestMethod]
        public void Expose_Rom_Bank_Zero_From_0x0000_To_0x3FFF()
        {
            var romData = Enumerable.Repeat(marker, Cartridge.RomBankSize);
            var rom = new Cartridge(romData.ToArray());
            var memoryBus = new MemoryBus(ppuRegisters: null) { IsBootRomMapped = false };

            memoryBus.Load(rom);

            for(int i = 0; i < Cartridge.RomBankSize; i++)
            {
                Assert.AreEqual(marker, memoryBus[i]);
            }
        }

        [TestMethod]
        public void Expose_Rom_Bank_One_From_0x4000_To_0x7FFF()
        {
            var romData = Enumerable.Repeat(marker, Cartridge.RomBankSize * 2);
            var rom = new Cartridge(romData.ToArray());
            var memoryBus = new MemoryBus(ppuRegisters: null);

            memoryBus.Load(rom);

            for (int i = Cartridge.RomBankSize; i < Cartridge.RomBankSize * 2; i++)
            {
                Assert.AreEqual(marker, memoryBus[i]);
            }
        }

        [TestMethod]
        public void Expose_Read_Writable_Work_Ram_From_0xC000_To_0xDFFF()
        {
            var memoryBus = new MemoryBus(ppuRegisters: null);

            for (int i = 0xC000; i <= 0xDFFF; i++)
            {
                memoryBus[i] = marker;
            }

            for (int i = 0xC000; i <= 0xDFFF; i++)
            {
                Assert.AreEqual(marker, memoryBus[i]);
            }
        }

        [TestMethod]
        public void Expose_Echo_Ram_From_0xE000_To_0xFDFF()
        {
            var memoryBus = new MemoryBus(ppuRegisters: null);

            //values written to work RAM at $C000 - $DFFF are repeated in the "echo RAM" range
            for (int i = 0xC000; i <= 0xDFFF; i++)
            {
                memoryBus[i] = marker;
            }

            for (int i = 0xE000; i <= 0xFDFF; i++)
            {
                Assert.AreEqual(marker, memoryBus[i]);
            }
        }

        [TestMethod]
        public void Expose_Read_Writable_Zero_Page_From_0xFF80_To_0xFFFE()
        {
            var memoryBus = new MemoryBus(ppuRegisters: null);

            for (int i = 0xFF80; i < 0xFFFF; i++)
            {
                memoryBus[i] = marker;
            }

            for (int i = 0xFF80; i < 0xFFFF; i++)
            {
                Assert.AreEqual(marker, memoryBus[i]);
            }
        }
    }
}
