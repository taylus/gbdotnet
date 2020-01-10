using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GBDotNet.Core.Test
{
    [TestClass]
    public class CPU_Flags_Should
    {
        private CPU cpu;

        [TestInitialize]
        public void BeforeEveryTest()
        {
            cpu = new CPU(new Registers(), new Memory());
        }

        [TestMethod]
        public void Update_Zero_Bit_When_Setting_F()
        {
            cpu.Registers.F = 0b10000000;
            Assert.IsTrue(cpu.Registers.HasFlag(Flags.Zero));

            cpu.Registers.F = 0b0000000;
            Assert.IsFalse(cpu.Registers.HasFlag(Flags.Zero));
        }

        [TestMethod]
        public void Update_F_When_Setting_Zero_Bit()
        {
            cpu.Registers.SetFlag(Flags.Zero);
            Assert.IsTrue(cpu.Registers.F.AreBitsSet(Flags.Zero));

            cpu.Registers.ClearFlag(Flags.Zero);
            Assert.IsFalse(cpu.Registers.F.AreBitsSet(Flags.Zero));
        }

        [TestMethod]
        public void Update_AddSubtract_Bit_When_Setting_F()
        {
            cpu.Registers.F = 0b01000000;
            Assert.IsTrue(cpu.Registers.HasFlag(Flags.AddSubtract));

            cpu.Registers.F = 0b0000000;
            Assert.IsFalse(cpu.Registers.HasFlag(Flags.AddSubtract));
        }

        [TestMethod]
        public void Update_F_When_Setting_AddSubtract_Bit()
        {
            cpu.Registers.SetFlag(Flags.AddSubtract);
            Assert.IsTrue(cpu.Registers.F.AreBitsSet(Flags.AddSubtract));
        }

        [TestMethod]
        public void Update_Half_Carry_Bit_When_Setting_F()
        {
            cpu.Registers.F = 0b00100000;
            Assert.IsTrue(cpu.Registers.HasFlag(Flags.HalfCarry));

            cpu.Registers.F = 0b0000000;
            Assert.IsFalse(cpu.Registers.HasFlag(Flags.HalfCarry));
        }

        [TestMethod]
        public void Update_F_When_Setting_Half_Carry_Bit()
        {
            cpu.Registers.SetFlag(Flags.HalfCarry);
            Assert.IsTrue(cpu.Registers.F.AreBitsSet(Flags.HalfCarry));
        }

        [TestMethod]
        public void Update_Carry_Bit_When_Setting_F()
        {
            cpu.Registers.F = 0b00010000;
            Assert.IsTrue(cpu.Registers.HasFlag(Flags.Carry));

            cpu.Registers.F = 0b0000000;
            Assert.IsFalse(cpu.Registers.HasFlag(Flags.Carry));
        }

        [TestMethod]
        public void Update_F_When_Setting_Carry_Bit()
        {
            cpu.Registers.SetFlag(Flags.Carry);
            Assert.IsTrue(cpu.Registers.F.AreBitsSet(Flags.Carry));
        }

        [TestMethod]
        public void Update_Multiple_Flags_When_Setting_F()
        {
            cpu.Registers.F = 0b11111111;
            Assert.IsTrue(cpu.Registers.HasFlag(Flags.Zero));
            Assert.IsTrue(cpu.Registers.HasFlag(Flags.AddSubtract));
            Assert.IsTrue(cpu.Registers.HasFlag(Flags.HalfCarry));
            Assert.IsTrue(cpu.Registers.HasFlag(Flags.Carry));
        }

        [TestMethod]
        public void Update_F_When_Setting_Multiple_Flags()
        {
            var flags = Flags.Zero | Flags.AddSubtract | Flags.HalfCarry | Flags.Carry;
            cpu.Registers.SetFlag(flags);
            Assert.IsTrue(cpu.Registers.F.AreBitsSet(flags));
        }
    }
}
