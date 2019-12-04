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
