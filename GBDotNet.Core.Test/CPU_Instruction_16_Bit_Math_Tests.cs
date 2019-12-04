using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GBDotNet.Core.Test
{
    /// <summary>
    /// Tests the CPU's 16-bit math instructions by executing them in controlled environments.
    /// </summary>
    [TestClass]
    public class CPU_Instruction_16_Bit_Math_Tests
    {
        [TestMethod]
        public void Instruction_0x03_Should_Increment_BC()
        {
            var memory = new Memory(0x03);
            var cpu = new CPU(new Registers(), memory);
            TestIncrement16BitRegister(cpu, () => cpu.Registers.BC);
        }

        [TestMethod]
        public void Instruction_0x0B_Should_Decrement_BC()
        {
            var memory = new Memory(0x0B);
            var cpu = new CPU(new Registers(), memory);
            TestDecrement16BitRegister(cpu, () => cpu.Registers.BC);
        }

        [TestMethod]
        public void Instruction_0x13_Should_Increment_DE()
        {
            var memory = new Memory(0x13);
            var cpu = new CPU(new Registers(), memory);
            TestIncrement16BitRegister(cpu, () => cpu.Registers.DE);
        }

        [TestMethod]
        public void Instruction_0x1B_Should_Decrement_DE()
        {
            var memory = new Memory(0x1B);
            var cpu = new CPU(new Registers(), memory);
            TestDecrement16BitRegister(cpu, () => cpu.Registers.DE);
        }

        [TestMethod]
        public void Instruction_0x23_Should_Increment_HL()
        {
            var memory = new Memory(0x23);
            var cpu = new CPU(new Registers(), memory);
            TestIncrement16BitRegister(cpu, () => cpu.Registers.HL);
        }

        [TestMethod]
        public void Instruction_0x2B_Should_Decrement_HL()
        {
            var memory = new Memory(0x2B);
            var cpu = new CPU(new Registers(), memory);
            TestDecrement16BitRegister(cpu, () => cpu.Registers.HL);
        }

        [TestMethod]
        public void Instruction_0x33_Should_Increment_SP()
        {
            var memory = new Memory(0x33);
            var cpu = new CPU(new Registers(), memory);
            TestIncrement16BitRegister(cpu, () => cpu.Registers.SP);
        }

        [TestMethod]
        public void Instruction_0x3B_Should_Decrement_SP()
        {
            var memory = new Memory(0x3B);
            var cpu = new CPU(new Registers(), memory);
            TestDecrement16BitRegister(cpu, () => cpu.Registers.SP);
        }

        [TestMethod]
        public void Instruction_0x09_Should_Add_BC_To_HL()
        {
            var memory = new Memory(0x09);
            var cpu = new CPU(new Registers(), memory);

            TestCaseWithCarries();
            TestCaseWithoutCarries();

            void TestCaseWithoutCarries()
            {
                TestCase(bc: 0b1010_0000_0101_0101,
                         hl: 0b0001_0000_0100_0000,
                         expectedHalfCarry: false,
                         expectedCarry: false);
            }

            void TestCaseWithCarries()
            {
                TestCase(bc: 0b1010_1100_0101_0101,
                         hl: 0b1001_0100_0100_0000,
                         expectedHalfCarry: true,
                         expectedCarry: true);
            }

            void TestCase(ushort bc, ushort hl, bool expectedHalfCarry, bool expectedCarry)
            {
                cpu.Registers.PC = 0;
                cpu.Registers.BC = bc;
                cpu.Registers.HL = hl;
                cpu.Registers.SetFlag(Flags.AddSubtract);

                cpu.Tick();

                Assert.AreEqual((ushort)(bc + hl), cpu.Registers.HL);
                Assert.IsFalse(cpu.Registers.HasFlag(Flags.AddSubtract), "add hl, bc instruction should clear N flag.");

                if (expectedHalfCarry)
                    Assert.IsTrue(cpu.Registers.HasFlag(Flags.HalfCarry), "Half carry flag should be set.");
                else
                    Assert.IsFalse(cpu.Registers.HasFlag(Flags.HalfCarry), "Half carry flag should not be set.");

                if (expectedCarry)
                    Assert.IsTrue(cpu.Registers.HasFlag(Flags.Carry), "Carry flag should be set.");
                else
                    Assert.IsFalse(cpu.Registers.HasFlag(Flags.Carry), "Carry flag should not be set.");
            }
        }

        [TestMethod]
        public void Instruction_0x19_Should_Add_DE_To_HL()
        {

        }

        [TestMethod]
        public void Instruction_0x29_Should_Add_HL_To_HL()
        {

        }

        [TestMethod]
        public void Instruction_0x39_Should_Add_SP_To_HL()
        {

        }

        private static void TestIncrement16BitRegister(CPU cpu, Func<ushort> registerPairUnderTest)
        {
            for (int i = 0; i <= ushort.MaxValue; i++)
            {
                cpu.Tick();
                var expected = (ushort)(i + 1);
                Assert.AreEqual(expected, registerPairUnderTest());
                cpu.Registers.PC--;
            }
        }

        private static void TestDecrement16BitRegister(CPU cpu, Func<ushort> registerPairUnderTest)
        {
            for (int i = ushort.MaxValue; i >= 0; i--)
            {
                cpu.Tick();
                Assert.AreEqual(i, registerPairUnderTest());
                cpu.Registers.PC--;
            }
        }
    }
}
