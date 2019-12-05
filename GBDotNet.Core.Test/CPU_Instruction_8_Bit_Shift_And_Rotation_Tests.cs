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
            AssertOtherFlagsAreCleared();
            cpu.Registers.PC--;

            cpu.Tick();
            Assert.AreEqual(0b0001_1001, cpu.Registers.A, "Accumulator has incorrect value after second rlca instruction.");
            Assert.IsTrue(cpu.Registers.HasFlag(Flags.Carry), "Carry flag should be set after first rlca instruction.");
            AssertOtherFlagsAreCleared();

            void AssertOtherFlagsAreCleared()
            {
                Assert.IsFalse(cpu.Registers.HasFlag(Flags.Zero), "rlca instruction should clear Z flag.");
                Assert.IsFalse(cpu.Registers.HasFlag(Flags.AddSubtract), "rlca instruction should clear N flag.");
                Assert.IsFalse(cpu.Registers.HasFlag(Flags.HalfCarry), "rlca instruction should clear H flag.");
            }
        }

        [TestMethod]
        public void Instruction_0x0F_Should_Rotate_A_Right_Circular()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x17_Should_Rotate_A_Left()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x1F_Should_Rotate_A_Right()
        {
            throw new NotImplementedException();
        }
    }
}
