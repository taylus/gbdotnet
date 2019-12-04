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
        public void Instruction_0x13_Should_Increment_DE()
        {
            var memory = new Memory(0x13);
            var cpu = new CPU(new Registers(), memory);
            TestIncrement16BitRegister(cpu, () => cpu.Registers.DE);
        }

        [TestMethod]
        public void Instruction_0x23_Should_Increment_HL()
        {
            var memory = new Memory(0x23);
            var cpu = new CPU(new Registers(), memory);
            TestIncrement16BitRegister(cpu, () => cpu.Registers.HL);
        }

        [TestMethod]
        public void Instruction_0x33_Should_Increment_SP()
        {
            var memory = new Memory(0x33);
            var cpu = new CPU(new Registers(), memory);
            TestIncrement16BitRegister(cpu, () => cpu.Registers.SP);
        }

        /// <summary>
        /// Tests instructions like inc bc.
        /// </summary>
        /// <remarks>Simpler to test since no pesky flags are set.</remarks>
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
    }
}
