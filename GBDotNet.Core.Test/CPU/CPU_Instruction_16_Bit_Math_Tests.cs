using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GBDotNet.Core.Test
{
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
            TestAdd16BitRegisterToHL(cpu,
                registerPairGetter: () => cpu.Registers.BC,
                registerPairSetter: (value) => cpu.Registers.BC = value);
        }

        [TestMethod]
        public void Instruction_0x19_Should_Add_DE_To_HL()
        {
            var memory = new Memory(0x19);
            var cpu = new CPU(new Registers(), memory);
            TestAdd16BitRegisterToHL(cpu,
                registerPairGetter: () => cpu.Registers.DE,
                registerPairSetter: (value) => cpu.Registers.DE = value);
        }

        [TestMethod]
        public void Instruction_0x29_Should_Add_HL_To_HL()
        {
            var memory = new Memory(0x29);
            var cpu = new CPU(new Registers(), memory);
            TestAdd16BitRegisterToHL(cpu,
                registerPairGetter: () => cpu.Registers.HL,
                registerPairSetter: (value) => cpu.Registers.HL = value);
        }

        [TestMethod]
        public void Instruction_0x39_Should_Add_SP_To_HL()
        {
            var memory = new Memory(0x39);
            var cpu = new CPU(new Registers(), memory);
            TestAdd16BitRegisterToHL(cpu,
                registerPairGetter: () => cpu.Registers.SP,
                registerPairSetter: (value) => cpu.Registers.SP = value);
        }

        [TestMethod]
        public void Instruction_0xE8_Should_Add_8_Bit_Signed_Immediate_To_Stack_Pointer()
        {
            var memory = new Memory(0xE8, 0xFF);
            var cpu = new CPU(new Registers() { SP = 0xFFFE }, memory);

            cpu.Tick();

            Assert.AreEqual(0xFFFD, cpu.Registers.SP);
            Assert.IsFalse(cpu.Registers.HasFlag(Flags.Zero | Flags.AddSubtract), "add sp, e8 instruction should always clear Z and N flags.");
            Assert.IsFalse(cpu.Registers.HasFlag(Flags.Carry | Flags.HalfCarry));
        }

        private static void TestIncrement16BitRegister(CPU cpu, Func<ushort> registerPairUnderTest)
        {
            for (int i = 0; i <= ushort.MaxValue; i++)
            {
                cpu.Tick();
                var expected = (ushort)(i + 1);
                Assert.AreEqual(expected, registerPairUnderTest());
                Assert.AreEqual(8, cpu.CyclesLastTick);
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

        private static void TestAdd16BitRegisterToHL(CPU cpu, Func<ushort> registerPairGetter, Action<ushort> registerPairSetter)
        {
            TestCaseWithCarries();
            TestCaseWithoutCarries();

            void TestCaseWithCarries()
            {
                const ushort testInput = 0b1000_1000_0000_0000; //use same value for add hl, hl case
                registerPairSetter(testInput);
                cpu.Registers.HL = testInput;
                ExecuteTestCase(expectedHalfCarry: true, expectedCarry: true);
            }

            void TestCaseWithoutCarries()
            {
                const ushort testInput = 0b0001_0000_0100_0011; ////use same value for add hl, hl case
                registerPairSetter(testInput);
                cpu.Registers.HL = testInput;
                ExecuteTestCase(expectedHalfCarry: false, expectedCarry: false);
            }

            void ExecuteTestCase(bool expectedHalfCarry, bool expectedCarry)
            {
                cpu.Registers.PC = 0;
                cpu.Registers.SetFlag(Flags.AddSubtract);

                ushort oldRegisterPairValue = registerPairGetter(); //capture for when this register pair is also hl (add hl, hl)
                ushort oldHL = cpu.Registers.HL;
                cpu.Tick();

                Assert.AreEqual((ushort)(oldRegisterPairValue + oldHL), cpu.Registers.HL);
                Assert.IsFalse(cpu.Registers.HasFlag(Flags.AddSubtract), "add hl, r16 instruction should clear N flag.");

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
    }
}
