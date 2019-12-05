using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GBDotNet.Core.Test
{
    [TestClass]
    public class CPU_Instruction_8_Bit_Shift_And_Rotation_Tests
    {
        [TestMethod]
        public void Instruction_0x07_Should_Rotate_A_Left_Circular()
        {
            var memory = new Memory(0x07);
            var cpu = new CPU(new Registers() { A = 0b0100_0110 }, memory);
            cpu.Registers.SetFlag(Flags.Zero);
            cpu.Registers.SetFlag(Flags.AddSubtract);
            cpu.Registers.SetFlag(Flags.HalfCarry);

            cpu.Tick();
            Assert.AreEqual(0b1000_1100, cpu.Registers.A, "Accumulator has incorrect value after first rlca instruction.");
            Assert.IsFalse(cpu.Registers.HasFlag(Flags.Carry), "Carry flag should be zero after first rlca instruction.");
            AssertFlagsAreCleared(cpu, Flags.Zero | Flags.AddSubtract | Flags.HalfCarry);
            cpu.Registers.PC--;

            cpu.Tick();
            Assert.AreEqual(0b0001_1001, cpu.Registers.A, "Accumulator has incorrect value after second rlca instruction.");
            Assert.IsTrue(cpu.Registers.HasFlag(Flags.Carry), "Carry flag should be set after second rlca instruction.");
            AssertFlagsAreCleared(cpu, Flags.Zero | Flags.AddSubtract | Flags.HalfCarry);
        }

        [TestMethod]
        public void Instruction_0x0F_Should_Rotate_A_Right_Circular()
        {
            var memory = new Memory(0x0F);
            var cpu = new CPU(new Registers() { A = 0b0100_0110 }, memory);
            cpu.Registers.SetFlag(Flags.Zero);
            cpu.Registers.SetFlag(Flags.AddSubtract);
            cpu.Registers.SetFlag(Flags.HalfCarry);

            cpu.Tick();
            Assert.AreEqual(0b0010_0011, cpu.Registers.A, "Accumulator has incorrect value after first rrca instruction.");
            Assert.IsFalse(cpu.Registers.HasFlag(Flags.Carry), "Carry flag should be zero after first rrca instruction.");
            AssertFlagsAreCleared(cpu, Flags.Zero | Flags.AddSubtract | Flags.HalfCarry);
            cpu.Registers.PC--;

            cpu.Tick();
            Assert.AreEqual(0b1001_0001, cpu.Registers.A, "Accumulator has incorrect value after second rrca instruction.");
            Assert.IsTrue(cpu.Registers.HasFlag(Flags.Carry), "Carry flag should be set after second rrca instruction.");
            AssertFlagsAreCleared(cpu, Flags.Zero | Flags.AddSubtract | Flags.HalfCarry);
        }

        [TestMethod]
        public void Instruction_0x17_Should_Rotate_A_Left()
        {
            var memory = new Memory(0x17);
            var cpu = new CPU(new Registers(), memory);
            cpu.Registers.SetFlag(Flags.Carry);
            cpu.Registers.SetFlag(Flags.Zero);
            cpu.Registers.SetFlag(Flags.AddSubtract);
            cpu.Registers.SetFlag(Flags.HalfCarry);

            //test by shifting the carry bit all the way across the accumulator:
            //00000000 C:1, then 00000001 C:0, then 00000010 C:0, ..., all the way back to 00000000 C:1
            for (int i = 0; i < 8; i++)
            {
                cpu.Tick();
                Assert.AreEqual((byte)(1 << i), cpu.Registers.A, "Accumulator has incorrect value after rla instruction.");
                Assert.IsFalse(cpu.Registers.HasFlag(Flags.Carry), "Carry flag should be cleared after rla instruction.");
                AssertFlagsAreCleared(cpu, Flags.Zero | Flags.AddSubtract | Flags.HalfCarry);
                cpu.Registers.PC--;
            }

            cpu.Tick();
            Assert.AreEqual(0, cpu.Registers.A, "Accumulator should be zero after a full left rotation.");
            Assert.IsTrue(cpu.Registers.HasFlag(Flags.Carry), "Carry flag should be set again after a full left rotation.");
            AssertFlagsAreCleared(cpu, Flags.Zero | Flags.AddSubtract | Flags.HalfCarry);
        }

        [TestMethod]
        public void Instruction_0x1F_Should_Rotate_A_Right()
        {
            var memory = new Memory(0x1F);
            var cpu = new CPU(new Registers(), memory);
            //...

            cpu.Tick();

            //...
        }

        private static void AssertFlagsAreCleared(CPU cpu, Flags flags)
        {
            if (flags.HasFlag(Flags.Zero)) Assert.IsFalse(cpu.Registers.HasFlag(Flags.Zero), "Expected instruction to clear Z flag.");
            if (flags.HasFlag(Flags.AddSubtract))  Assert.IsFalse(cpu.Registers.HasFlag(Flags.AddSubtract), "Expected instruction to clear N flag.");
            if (flags.HasFlag(Flags.HalfCarry))  Assert.IsFalse(cpu.Registers.HasFlag(Flags.HalfCarry), "Expected instruction to clear H flag.");
            if (flags.HasFlag(Flags.Carry)) Assert.IsFalse(cpu.Registers.HasFlag(Flags.Carry), "Expected instruction to clear C flag.");
        }
    }
}
