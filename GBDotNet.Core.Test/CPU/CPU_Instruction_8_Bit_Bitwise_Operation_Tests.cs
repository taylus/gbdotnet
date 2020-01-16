using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GBDotNet.Core.Test
{
    [TestClass]
    public class CPU_Instruction_8_Bit_Bitwise_Operation_Tests
    {
        [TestMethod]
        public void Instruction_0xA0_Should_Bitwise_And_B_With_A()
        {
            var memory = new Memory(0xA0);
            var cpu = new CPU(new Registers(), memory);

            TestAnding8BitRegisterWithAccumulator(cpu,
                a: 0xFF, registerValue: 0xF0,
                registerSetter: (value) => cpu.Registers.B = value,
                expectedZero: false);
        }

        [TestMethod]
        public void Instruction_0xA1_Should_Bitwise_And_C_With_A()
        {
            var memory = new Memory(0xA1);
            var cpu = new CPU(new Registers(), memory);

            TestAnding8BitRegisterWithAccumulator(cpu,
                a: 0xFF, registerValue: 0x00,
                registerSetter: (value) => cpu.Registers.C = value,
                expectedZero: true);
        }

        [TestMethod]
        public void Instruction_0xA2_Should_Bitwise_And_D_With_A()
        {
            var memory = new Memory(0xA2);
            var cpu = new CPU(new Registers(), memory);

            TestAnding8BitRegisterWithAccumulator(cpu,
                a: 0x0F, registerValue: 0x0F,
                registerSetter: (value) => cpu.Registers.D = value,
                expectedZero: false);
        }

        [TestMethod]
        public void Instruction_0xA3_Should_Bitwise_And_E_With_A()
        {
            var memory = new Memory(0xA3);
            var cpu = new CPU(new Registers(), memory);

            TestAnding8BitRegisterWithAccumulator(cpu,
                a: 0xAA, registerValue: 0xFF,
                registerSetter: (value) => cpu.Registers.E = value,
                expectedZero: false);
        }

        [TestMethod]
        public void Instruction_0xA4_Should_Bitwise_And_H_With_A()
        {
            var memory = new Memory(0xA4);
            var cpu = new CPU(new Registers(), memory);

            TestAnding8BitRegisterWithAccumulator(cpu,
                a: 0x12, registerValue: 0x34,
                registerSetter: (value) => cpu.Registers.H = value,
                expectedZero: false);
        }

        [TestMethod]
        public void Instruction_0xA5_Should_Bitwise_And_L_With_A()
        {
            var memory = new Memory(0xA5);
            var cpu = new CPU(new Registers(), memory);

            TestAnding8BitRegisterWithAccumulator(cpu,
                a: 0xF0, registerValue: 0x0F,
                registerSetter: (value) => cpu.Registers.L = value,
                expectedZero: true);
        }

        [TestMethod]
        public void Instruction_0xA6_Should_Bitwise_And_Address_Pointed_To_By_HL_With_A()
        {
            var memory = new Memory(0xA6);
            var cpu = new CPU(new Registers() { HL = 0x4000 }, memory);

            TestAnding8BitRegisterWithAccumulator(cpu,
                a: 0xF0, registerValue: 0x0F,
                registerSetter: (value) => memory[cpu.Registers.HL] = value,
                expectedZero: true, expectedCycles: 8);
        }

        [TestMethod]
        public void Instruction_0xA7_Should_Bitwise_And_A_With_A()
        {
            var memory = new Memory(0xA7);
            var cpu = new CPU(new Registers(), memory);

            TestAnding8BitRegisterWithAccumulator(cpu,
                a: 0x05, registerValue: 0x05,
                registerSetter: (value) => cpu.Registers.A = value,
                expectedZero: false);
        }

        [TestMethod]
        public void Instruction_0xA8_Should_Bitwise_Exclusive_Or_B_With_A()
        {
            var memory = new Memory(0xA8);
            var cpu = new CPU(new Registers(), memory);

            TestXoring8BitRegisterWithAccumulator(cpu,
                a: 0xFF, registerValue: 0xFF,
                registerSetter: (value) => cpu.Registers.B = value,
                expectedZero: true);
        }

        [TestMethod]
        public void Instruction_0xA9_Should_Bitwise_Exclusive_Or_C_With_A()
        {
            var memory = new Memory(0xA9);
            var cpu = new CPU(new Registers(), memory);

            TestXoring8BitRegisterWithAccumulator(cpu,
                a: 0x00, registerValue: 0x00,
                registerSetter: (value) => cpu.Registers.C = value,
                expectedZero: true);
        }

        [TestMethod]
        public void Instruction_0xAA_Should_Bitwise_Exclusive_Or_D_With_A()
        {
            var memory = new Memory(0xAA);
            var cpu = new CPU(new Registers(), memory);

            TestXoring8BitRegisterWithAccumulator(cpu,
                a: 0xAA, registerValue: 0x55,
                registerSetter: (value) => cpu.Registers.D = value,
                expectedZero: false);
        }

        [TestMethod]
        public void Instruction_0xAB_Should_Bitwise_Exclusive_Or_E_With_A()
        {
            var memory = new Memory(0xAB);
            var cpu = new CPU(new Registers(), memory);

            TestXoring8BitRegisterWithAccumulator(cpu,
                a: 0xAA, registerValue: 0xFF,
                registerSetter: (value) => cpu.Registers.E = value,
                expectedZero: false);
        }

        [TestMethod]
        public void Instruction_0xAC_Should_Bitwise_Exclusive_Or_H_With_A()
        {
            var memory = new Memory(0xAC);
            var cpu = new CPU(new Registers(), memory);

            TestXoring8BitRegisterWithAccumulator(cpu,
                a: 0x00, registerValue: 0xFF,
                registerSetter: (value) => cpu.Registers.H = value,
                expectedZero: false);
        }

        [TestMethod]
        public void Instruction_0xAD_Should_Bitwise_Exclusive_Or_L_With_A()
        {
            var memory = new Memory(0xAD);
            var cpu = new CPU(new Registers(), memory);

            TestXoring8BitRegisterWithAccumulator(cpu,
                a: 0x01, registerValue: 0x02,
                registerSetter: (value) => cpu.Registers.L = value,
                expectedZero: false);
        }

        [TestMethod]
        public void Instruction_0xAE_Should_Bitwise_Exclusive_Or_Address_Pointed_To_By_HL_With_A()
        {
            var memory = new Memory(0xAE);
            var cpu = new CPU(new Registers() { HL = 0x4000 }, memory);

            TestXoring8BitRegisterWithAccumulator(cpu,
                a: 0xAA, registerValue: 0xAA,
                registerSetter: (value) => memory[cpu.Registers.HL] = value,
                expectedZero: true, expectedCycles: 8);
        }

        [TestMethod]
        public void Instruction_0xAF_Should_Bitwise_Exclusive_Or_A_With_A()
        {
            var memory = new Memory(0xAF);
            var cpu = new CPU(new Registers(), memory);

            TestXoring8BitRegisterWithAccumulator(cpu,
                a: 0xAA, registerValue: 0xAA,
                registerSetter: (value) => cpu.Registers.A = value,
                expectedZero: true);
        }

        [TestMethod]
        public void Instruction_0xB0_Should_Bitwise_Or_B_With_A()
        {
            var memory = new Memory(0xB0);
            var cpu = new CPU(new Registers(), memory);

            TestOring8BitRegisterWithAccumulator(cpu,
                a: 0xFF, registerValue: 0x00,
                registerSetter: (value) => cpu.Registers.B = value,
                expectedZero: false);
        }

        [TestMethod]
        public void Instruction_0xB1_Should_Bitwise_Or_C_With_A()
        {
            var memory = new Memory(0xB1);
            var cpu = new CPU(new Registers(), memory);

            TestOring8BitRegisterWithAccumulator(cpu,
                a: 0xFF, registerValue: 0xA0,
                registerSetter: (value) => cpu.Registers.C = value,
                expectedZero: false);
        }

        [TestMethod]
        public void Instruction_0xB2_Should_Bitwise_Or_D_With_A()
        {
            var memory = new Memory(0xB2);
            var cpu = new CPU(new Registers(), memory);

            TestOring8BitRegisterWithAccumulator(cpu,
                a: 0x00, registerValue: 0x00,
                registerSetter: (value) => cpu.Registers.D = value,
                expectedZero: true);
        }

        [TestMethod]
        public void Instruction_0xB3_Should_Bitwise_Or_E_With_A()
        {
            var memory = new Memory(0xB3);
            var cpu = new CPU(new Registers(), memory);

            TestOring8BitRegisterWithAccumulator(cpu,
                a: 0x12, registerValue: 0x34,
                registerSetter: (value) => cpu.Registers.E = value,
                expectedZero: false);
        }

        [TestMethod]
        public void Instruction_0xB4_Should_Bitwise_Or_H_With_A()
        {
            var memory = new Memory(0xB4);
            var cpu = new CPU(new Registers(), memory);

            TestOring8BitRegisterWithAccumulator(cpu,
                a: 0xAB, registerValue: 0xCD,
                registerSetter: (value) => cpu.Registers.H = value,
                expectedZero: false);
        }

        [TestMethod]
        public void Instruction_0xB5_Should_Bitwise_Or_L_With_A()
        {
            var memory = new Memory(0xB5);
            var cpu = new CPU(new Registers(), memory);

            TestOring8BitRegisterWithAccumulator(cpu,
                a: 0xFF, registerValue: 0xFF,
                registerSetter: (value) => cpu.Registers.L = value,
                expectedZero: false);
        }

        [TestMethod]
        public void Instruction_0xB6_Should_Bitwise_Or_Address_Pointed_To_By_HL_With_A()
        {
            var memory = new Memory(0xB6);
            var cpu = new CPU(new Registers() { HL = 0x4000 }, memory);

            TestOring8BitRegisterWithAccumulator(cpu,
                a: 0xAB, registerValue: 0xCD,
                registerSetter: (value) => memory[cpu.Registers.HL] = value,
                expectedZero: false, expectedCycles: 8);
        }

        [TestMethod]
        public void Instruction_0xB7_Should_Bitwise_Or_A_With_A()
        {
            var memory = new Memory(0xB7);
            var cpu = new CPU(new Registers(), memory);

            TestOring8BitRegisterWithAccumulator(cpu,
                a: 0xAA, registerValue: 0xAA,
                registerSetter: (value) => cpu.Registers.A = value,
                expectedZero: false);
        }

        [TestMethod]
        public void Instruction_0xE6_Should_Bitwise_And_8_Bit_Immediate_With_A()
        {
            var memory = new Memory(0xE6, 0xAA);
            var cpu = new CPU(new Registers() { A = 0xBB }, memory);

            var expected = (byte)(cpu.Registers.A & memory[1]);
            cpu.Tick();

            Assert.AreEqual(expected, cpu.Registers.A);
            Assert.AreEqual(8, cpu.CyclesLastTick);
        }

        [TestMethod]
        public void Instruction_0xEE_Should_Bitwise_Exclusive_Or_8_Bit_Immediate_With_A()
        {
            var memory = new Memory(0xEE, 0xAA);
            var cpu = new CPU(new Registers() { A = 0xBB }, memory);

            var expected = (byte)(cpu.Registers.A ^ memory[1]);
            cpu.Tick();

            Assert.AreEqual(expected, cpu.Registers.A);
            Assert.AreEqual(8, cpu.CyclesLastTick);
        }

        [TestMethod]
        public void Instruction_0xF6_Should_Bitwise_Or_8_Bit_Immediate_With_A()
        {
            var memory = new Memory(0xF6, 0xAA);
            var cpu = new CPU(new Registers() { A = 0xBB }, memory);

            var expected = (byte)(cpu.Registers.A | memory[1]);
            cpu.Tick();

            Assert.AreEqual(expected, cpu.Registers.A);
        }


        /// <summary>
        /// Tests instructions like and a, b.
        /// </summary>
        private static void TestAnding8BitRegisterWithAccumulator(CPU cpu, byte a, byte registerValue, Action<byte> registerSetter, bool expectedZero, int expectedCycles = 4)
        {
            cpu.Registers.PC = 0;   //assume the and instruction is always at the beginning of memory
            cpu.Registers.SetFlag(Flags.AddSubtract);   //set the N flag (and instructions should always clear it)
            cpu.Registers.ClearFlag(Flags.HalfCarry);   //clear the H flag (and instructions should always set it)
            cpu.Registers.SetFlag(Flags.Carry);   //set the C flag (and instructions should always clear it)
            cpu.Registers.A = a;
            registerSetter(registerValue);

            cpu.Tick();

            Assert.AreEqual((byte)(a & registerValue), cpu.Registers.A);

            if (expectedZero)
                Assert.IsTrue(cpu.Registers.HasFlag(Flags.Zero), $"Zero flag should be set when ANDing {registerValue} with accumulator {a}.");
            else
                Assert.IsFalse(cpu.Registers.HasFlag(Flags.Zero), $"Zero flag should not be set when ANDing {registerValue} with accumulator {a}.");

            Assert.IsFalse(cpu.Registers.HasFlag(Flags.AddSubtract), "AND instructions should always clear the N flag.");
            Assert.IsTrue(cpu.Registers.HasFlag(Flags.HalfCarry), "AND instructions should always set the H flag.");
            Assert.IsFalse(cpu.Registers.HasFlag(Flags.Carry), "AND instructions should always clear the C flag.");
            Assert.AreEqual(expectedCycles, cpu.CyclesLastTick);
        }

        /// <summary>
        /// Tests instructions like xor a, b.
        /// </summary>
        private static void TestXoring8BitRegisterWithAccumulator(CPU cpu, byte a, byte registerValue, Action<byte> registerSetter, bool expectedZero, int expectedCycles = 4)
        {
            cpu.Registers.PC = 0;   //assume the and instruction is always at the beginning of memory
            cpu.Registers.SetFlag(Flags.AddSubtract | Flags.HalfCarry | Flags.Carry);   //set these flags (xor instructions always clear them)
            cpu.Registers.A = a;
            registerSetter(registerValue);

            cpu.Tick();

            Assert.AreEqual((byte)(a ^ registerValue), cpu.Registers.A);

            if (expectedZero)
                Assert.IsTrue(cpu.Registers.HasFlag(Flags.Zero), $"Zero flag should be set when XORing {registerValue} with accumulator {a}.");
            else
                Assert.IsFalse(cpu.Registers.HasFlag(Flags.Zero), $"Zero flag should not be set when XORing {registerValue} with accumulator {a}.");

            Assert.IsFalse(cpu.Registers.HasFlag(Flags.AddSubtract), "XOR instructions should always clear the N flag.");
            Assert.IsFalse(cpu.Registers.HasFlag(Flags.HalfCarry), "XOR instructions should always clear the H flag.");
            Assert.IsFalse(cpu.Registers.HasFlag(Flags.Carry), "XOR instructions should always clear the C flag.");
            Assert.AreEqual(expectedCycles, cpu.CyclesLastTick);
        }

        /// <summary>
        /// Tests instructions like or a, b.
        /// </summary>
        private static void TestOring8BitRegisterWithAccumulator(CPU cpu, byte a, byte registerValue, Action<byte> registerSetter, bool expectedZero, int expectedCycles = 4)
        {
            cpu.Registers.PC = 0;   //assume the and instruction is always at the beginning of memory
            cpu.Registers.SetFlag(Flags.AddSubtract | Flags.HalfCarry | Flags.Carry);   //set these flags (or instructions always clear them)
            cpu.Registers.A = a;
            registerSetter(registerValue);

            cpu.Tick();

            Assert.AreEqual((byte)(a | registerValue), cpu.Registers.A);

            if (expectedZero)
                Assert.IsTrue(cpu.Registers.HasFlag(Flags.Zero), $"Zero flag should be set when ORing {registerValue} with accumulator {a}.");
            else
                Assert.IsFalse(cpu.Registers.HasFlag(Flags.Zero), $"Zero flag should not be set when ORing {registerValue} with accumulator {a}.");

            Assert.IsFalse(cpu.Registers.HasFlag(Flags.AddSubtract), "OR instructions should always clear the N flag.");
            Assert.IsFalse(cpu.Registers.HasFlag(Flags.HalfCarry), "OR instructions should always clear the H flag.");
            Assert.IsFalse(cpu.Registers.HasFlag(Flags.Carry), "OR instructions should always clear the C flag.");
            Assert.AreEqual(expectedCycles, cpu.CyclesLastTick);
        }
    }
}
