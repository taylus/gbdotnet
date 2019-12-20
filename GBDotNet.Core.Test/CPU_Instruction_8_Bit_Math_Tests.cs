using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GBDotNet.Core.Test
{
    [TestClass]
    public class CPU_Instruction_8_Bit_Math_Tests
    {
        [TestMethod]
        public void Instruction_0x04_Should_Increment_B()
        {
            var memory = new Memory(0x04);
            var cpu = new CPU(new Registers(), memory);
            TestIncrement8BitRegister(cpu, () => cpu.Registers.B);
        }

        [TestMethod]
        public void Instruction_0x04_Increment_B_Should_Set_Half_Carry_Flag_Correctly()
        {
            var memory = new Memory(0x04);
            var cpu = new CPU(new Registers() { B = 0xFF }, memory);

            cpu.Tick();
            Assert.IsTrue(cpu.Registers.HasFlag(Flags.HalfCarry), "Half carry flag should be set when incrementing from ff -> 00.");

            cpu.Registers.B = 0xDF;
            cpu.Registers.PC--;
            cpu.Tick();
            Assert.IsTrue(cpu.Registers.HasFlag(Flags.HalfCarry), "Half carry flag should be set when incrementing from df -> e0.");

            cpu.Registers.PC--;
            cpu.Tick();
            Assert.IsFalse(cpu.Registers.HasFlag(Flags.HalfCarry), "Half carry flag should be cleared when incrementing from e0 -> e1.");
        }

        [TestMethod]
        public void Instruction_0x05_Should_Decrement_B()
        {
            var memory = new Memory(0x05);
            var cpu = new CPU(new Registers(), memory);
            TestDecrement8BitRegister(cpu, () => cpu.Registers.B);
        }

        [TestMethod]
        public void Instruction_0x05_Decrement_B_Should_Set_Half_Carry_Flag_Correctly()
        {
            var memory = new Memory(0x05);
            var cpu = new CPU(new Registers(), memory);

            cpu.Tick();
            Assert.IsTrue(cpu.Registers.HasFlag(Flags.HalfCarry), "Half carry flag should be set when decrementing from 0 -> ff.");

            cpu.Registers.B = 0xE0;
            cpu.Registers.PC--;
            cpu.Tick();
            Assert.IsTrue(cpu.Registers.HasFlag(Flags.HalfCarry), "Half carry flag should be set when decrementing from e0 -> df.");

            cpu.Registers.PC--;
            cpu.Tick();
            Assert.IsFalse(cpu.Registers.HasFlag(Flags.HalfCarry), "Half carry flag should be cleared when decrementing from df -> de.");
        }

        [TestMethod]
        public void Instruction_0x0C_Should_Increment_C()
        {
            var memory = new Memory(0x0C);
            var cpu = new CPU(new Registers(), memory);
            TestIncrement8BitRegister(cpu, () => cpu.Registers.C);
        }

        [TestMethod]
        public void Instruction_0x0D_Should_Decrement_C()
        {
            var memory = new Memory(0x0D);
            var cpu = new CPU(new Registers(), memory);
            TestDecrement8BitRegister(cpu, () => cpu.Registers.C);
        }

        [TestMethod]
        public void Instruction_0x14_Should_Increment_D()
        {
            var memory = new Memory(0x14);
            var cpu = new CPU(new Registers(), memory);
            TestIncrement8BitRegister(cpu, () => cpu.Registers.D);
        }

        [TestMethod]
        public void Instruction_0x15_Should_Decrement_D()
        {
            var memory = new Memory(0x15);
            var cpu = new CPU(new Registers(), memory);
            TestDecrement8BitRegister(cpu, () => cpu.Registers.D);
        }

        [TestMethod]
        public void Instruction_0x1C_Should_Increment_E()
        {
            var memory = new Memory(0x1C);
            var cpu = new CPU(new Registers(), memory);
            TestIncrement8BitRegister(cpu, () => cpu.Registers.E);
        }

        [TestMethod]
        public void Instruction_0x1D_Should_Decrement_E()
        {
            var memory = new Memory(0x1D);
            var cpu = new CPU(new Registers(), memory);
            TestDecrement8BitRegister(cpu, () => cpu.Registers.E);
        }

        [TestMethod]
        public void Instruction_0x24_Should_Increment_H()
        {
            var memory = new Memory(0x24);
            var cpu = new CPU(new Registers(), memory);
            TestIncrement8BitRegister(cpu, () => cpu.Registers.H);
        }

        [TestMethod]
        public void Instruction_0x25_Should_Decrement_H()
        {
            var memory = new Memory(0x25);
            var cpu = new CPU(new Registers(), memory);
            TestDecrement8BitRegister(cpu, () => cpu.Registers.H);
        }

        [TestMethod]
        public void Instruction_0x2C_Should_Increment_L()
        {
            var memory = new Memory(0x2C);
            var cpu = new CPU(new Registers(), memory);
            TestIncrement8BitRegister(cpu, () => cpu.Registers.L);
        }

        [TestMethod]
        public void Instruction_0x2D_Should_Decrement_L()
        {
            var memory = new Memory(0x2D);
            var cpu = new CPU(new Registers(), memory);
            TestDecrement8BitRegister(cpu, () => cpu.Registers.L);
        }

        [TestMethod]
        public void Instruction_0x3C_Should_Increment_A()
        {
            var memory = new Memory(0x3C);
            var cpu = new CPU(new Registers(), memory);
            TestIncrement8BitRegister(cpu, () => cpu.Registers.A);
        }

        [TestMethod]
        public void Instruction_0x3D_Should_Decrement_A()
        {
            var memory = new Memory(0x3D);
            var cpu = new CPU(new Registers(), memory);
            TestDecrement8BitRegister(cpu, () => cpu.Registers.A);
        }

        [TestMethod]
        public void Instruction_0x80_Should_Add_B_To_A()
        {
            var memory = new Memory(0x80);
            var cpu = new CPU(new Registers(), memory);

            TestAdding8BitRegisterToAccumulator(cpu,
                a: 0x00, registerValue: 0x00,
                registerSetter: (value) => cpu.Registers.B = value,
                expectedZero: true,
                expectedCarry: false,
                expectedHalfCarry: false);
        }

        [TestMethod]
        public void Instruction_0x81_Should_Add_C_To_A()
        {
            var memory = new Memory(0x81);
            var cpu = new CPU(new Registers(), memory);

            TestAdding8BitRegisterToAccumulator(cpu,
                a: 0xFF, registerValue: 0x01,
                registerSetter: (value) => cpu.Registers.C = value,
                expectedZero: true,
                expectedCarry: true,
                expectedHalfCarry: true);
        }

        [TestMethod]
        public void Instruction_0x82_Should_Add_D_To_A()
        {
            var memory = new Memory(0x82);
            var cpu = new CPU(new Registers(), memory);

            TestAdding8BitRegisterToAccumulator(cpu,
                a: 0xA0, registerValue: 0x10,
                registerSetter: (value) => cpu.Registers.D = value,
                expectedZero: false,
                expectedCarry: false,
                expectedHalfCarry: false);
        }

        [TestMethod]
        public void Instruction_0x83_Should_Add_E_To_A()
        {
            var memory = new Memory(0x83);
            var cpu = new CPU(new Registers(), memory);

            TestAdding8BitRegisterToAccumulator(cpu,
                a: 0xA0, registerValue: 0x10,
                registerSetter: (value) => cpu.Registers.E = value,
                expectedZero: false,
                expectedCarry: false,
                expectedHalfCarry: false);
        }

        [TestMethod]
        public void Instruction_0x84_Should_Add_H_To_A()
        {
            var memory = new Memory(0x84);
            var cpu = new CPU(new Registers(), memory);

            TestAdding8BitRegisterToAccumulator(cpu,
                a: 0xA0, registerValue: 0x10,
                registerSetter: (value) => cpu.Registers.H = value,
                expectedZero: false,
                expectedCarry: false,
                expectedHalfCarry: false);
        }

        [TestMethod]
        public void Instruction_0x85_Should_Add_L_To_A()
        {
            var memory = new Memory(0x85);
            var cpu = new CPU(new Registers(), memory);

            TestAdding8BitRegisterToAccumulator(cpu,
                a: 0xA0, registerValue: 0x10,
                registerSetter: (value) => cpu.Registers.L = value,
                expectedZero: false,
                expectedCarry: false,
                expectedHalfCarry: false);
        }

        [TestMethod]
        public void Instruction_0x86_Should_Add_Address_Pointed_To_By_HL_To_A()
        {
            var memory = new Memory(0x86);
            var cpu = new CPU(new Registers() { HL = 0x1000 }, memory);

            TestAdding8BitRegisterToAccumulator(cpu,
                a: 0xA0, registerValue: 0x10,
                registerSetter: (value) => memory[cpu.Registers.HL] = value,
                expectedZero: false,
                expectedCarry: false,
                expectedHalfCarry: false);
        }

        [TestMethod]
        public void Instruction_0x87_Should_Add_A_To_A()
        {
            var memory = new Memory(0x87);
            var cpu = new CPU(new Registers(), memory);

            TestAdding8BitRegisterToAccumulator(cpu,
                a: 0x01, registerValue: 0x01,
                registerSetter: (value) => cpu.Registers.A = value,
                expectedZero: false,
                expectedCarry: false,
                expectedHalfCarry: false);
        }

        [TestMethod]
        public void Instruction_0x88_Should_Add_B_Plus_Carry_To_A()
        {
            var memory = new Memory(0x88);
            var cpu = new CPU(new Registers(), memory);

            TestAdding8BitRegisterToAccumulator(cpu,
                a: 0x01, registerValue: 0x01, carryBit: true,
                registerSetter: (value) => cpu.Registers.B = value,
                expectedZero: false,
                expectedCarry: false,
                expectedHalfCarry: false);
        }

        [TestMethod]
        public void Instruction_0x89_Should_Add_C_Plus_Carry_To_A()
        {
            var memory = new Memory(0x89);
            var cpu = new CPU(new Registers(), memory);

            TestAdding8BitRegisterToAccumulator(cpu,
                a: 0xFF, registerValue: 0x00, carryBit: true,
                registerSetter: (value) => cpu.Registers.C = value,
                expectedZero: true,
                expectedCarry: true,
                expectedHalfCarry: true);
        }

        [TestMethod]
        public void Instruction_0x8A_Should_Add_D_Plus_Carry_To_A()
        {
            var memory = new Memory(0x8A);
            var cpu = new CPU(new Registers(), memory);

            TestAdding8BitRegisterToAccumulator(cpu,
                a: 0xFF, registerValue: 0x01, carryBit: false,
                registerSetter: (value) => cpu.Registers.D = value,
                expectedZero: true,
                expectedCarry: true,
                expectedHalfCarry: true);
        }

        [TestMethod]
        public void Instruction_0x8B_Should_Add_E_Plus_Carry_To_A()
        {
            var memory = new Memory(0x8B);
            var cpu = new CPU(new Registers(), memory);

            TestAdding8BitRegisterToAccumulator(cpu,
                a: 0xA0, registerValue: 0x0E, carryBit: true,
                registerSetter: (value) => cpu.Registers.E = value,
                expectedZero: false,
                expectedCarry: false,
                expectedHalfCarry: false);
        }

        [TestMethod]
        public void Instruction_0x8C_Should_Add_H_Plus_Carry_To_A()
        {
            var memory = new Memory(0x8C);
            var cpu = new CPU(new Registers(), memory);

            TestAdding8BitRegisterToAccumulator(cpu,
                a: 0xA0, registerValue: 0x0E, carryBit: true,
                registerSetter: (value) => cpu.Registers.H = value,
                expectedZero: false,
                expectedCarry: false,
                expectedHalfCarry: false);
        }

        [TestMethod]
        public void Instruction_0x8D_Should_Add_L_Plus_Carry_To_A()
        {
            var memory = new Memory(0x8D);
            var cpu = new CPU(new Registers(), memory);

            TestAdding8BitRegisterToAccumulator(cpu,
                a: 0xA0, registerValue: 0x0E, carryBit: true,
                registerSetter: (value) => cpu.Registers.L = value,
                expectedZero: false,
                expectedCarry: false,
                expectedHalfCarry: false);
        }

        [TestMethod]
        public void Instruction_0x8E_Should_Add_Address_Pointed_To_By_HL_Plus_Carry_To_A()
        {
            var memory = new Memory(0x8E);
            var cpu = new CPU(new Registers() { HL = 0x4000 }, memory);

            TestAdding8BitRegisterToAccumulator(cpu,
                a: 0xA0, registerValue: 0x0E, carryBit: true,
                registerSetter: (value) => memory[cpu.Registers.HL] = value,
                expectedZero: false,
                expectedCarry: false,
                expectedHalfCarry: false);
        }

        [TestMethod]
        public void Instruction_0x8F_Should_Add_A_Plus_Carry_To_A()
        {
            var memory = new Memory(0x8F);
            var cpu = new CPU(new Registers(), memory);

            TestAdding8BitRegisterToAccumulator(cpu,
                a: 0x08, registerValue: 0x08, carryBit: true,
                registerSetter: (value) => cpu.Registers.A = value,
                expectedZero: false,
                expectedCarry: false,
                expectedHalfCarry: true);
        }

        [TestMethod]
        public void Instruction_0x90_Should_Subtract_B_From_A()
        {
            var memory = new Memory(0x90);
            var cpu = new CPU(new Registers(), memory);

            TestSubtracting8BitRegisterFromAccumulator(cpu,
                a: 0xAA, registerValue: 0xAA,
                registerSetter: (value) => cpu.Registers.B = value,
                expectedZero: true,
                expectedCarry: false,
                expectedHalfCarry: false);
        }

        [TestMethod]
        public void Instruction_0x91_Should_Subtract_C_From_A()
        {
            var memory = new Memory(0x91);
            var cpu = new CPU(new Registers(), memory);

            TestSubtracting8BitRegisterFromAccumulator(cpu,
                a: 0xAA, registerValue: 0x0A,
                registerSetter: (value) => cpu.Registers.C = value,
                expectedZero: false,
                expectedCarry: false,
                expectedHalfCarry: false);
        }

        [TestMethod]
        public void Instruction_0x92_Should_Subtract_D_From_A()
        {
            var memory = new Memory(0x92);
            var cpu = new CPU(new Registers(), memory);

            TestSubtracting8BitRegisterFromAccumulator(cpu,
                a: 0x55, registerValue: 0xFF,
                registerSetter: (value) => cpu.Registers.D = value,
                expectedZero: false,
                expectedCarry: true,
                expectedHalfCarry: true);
        }

        [TestMethod]
        public void Instruction_0x93_Should_Subtract_E_From_A()
        {
            var memory = new Memory(0x93);
            var cpu = new CPU(new Registers(), memory);

            TestSubtracting8BitRegisterFromAccumulator(cpu,
                a: 0x55, registerValue: 0xF0,
                registerSetter: (value) => cpu.Registers.E = value,
                expectedZero: false,
                expectedCarry: true,
                expectedHalfCarry: false);
        }

        [TestMethod]
        public void Instruction_0x94_Should_Subtract_H_From_A()
        {
            var memory = new Memory(0x94);
            var cpu = new CPU(new Registers(), memory);

            TestSubtracting8BitRegisterFromAccumulator(cpu,
                a: 0x00, registerValue: 0xFF,
                registerSetter: (value) => cpu.Registers.H = value,
                expectedZero: false,
                expectedCarry: true,
                expectedHalfCarry: true);
        }

        [TestMethod]
        public void Instruction_0x95_Should_Subtract_L_From_A()
        {
            var memory = new Memory(0x95);
            var cpu = new CPU(new Registers(), memory);

            TestSubtracting8BitRegisterFromAccumulator(cpu,
                a: 0x01, registerValue: 0x00,
                registerSetter: (value) => cpu.Registers.L = value,
                expectedZero: false,
                expectedCarry: false,
                expectedHalfCarry: false);
        }

        [TestMethod]
        public void Instruction_0x96_Should_Subtract_Address_Pointed_To_By_HL_From_A()
        {
            var memory = new Memory(0x96);
            var cpu = new CPU(new Registers() { HL = 0x4000 }, memory);

            TestSubtracting8BitRegisterFromAccumulator(cpu,
                a: 0xBE, registerValue: 0xAF,
                registerSetter: (value) => memory[cpu.Registers.HL] = value,
                expectedZero: false,
                expectedCarry: false,
                expectedHalfCarry: true);
        }

        [TestMethod]
        public void Instruction_0x97_Should_Subtract_A_From_A()
        {
            var memory = new Memory(0x97);
            var cpu = new CPU(new Registers(), memory);

            TestSubtracting8BitRegisterFromAccumulator(cpu,
                a: 0x05, registerValue: 0x05,
                registerSetter: (value) => cpu.Registers.A = value,
                expectedZero: true,
                expectedCarry: false,
                expectedHalfCarry: false);
        }

        [TestMethod]
        public void Instruction_0x98_Should_Subtract_B_Plus_Carry_From_A()
        {
            var memory = new Memory(0x98);
            var cpu = new CPU(new Registers(), memory);

            TestSubtracting8BitRegisterFromAccumulator(cpu,
                a: 0x05, registerValue: 0x05, carryBit: true,
                registerSetter: (value) => cpu.Registers.B = value,
                expectedZero: false,
                expectedCarry: true,
                expectedHalfCarry: true);
        }

        [TestMethod]
        public void Instruction_0x99_Should_Subtract_C_Plus_Carry_From_A()
        {
            var memory = new Memory(0x99);
            var cpu = new CPU(new Registers(), memory);

            TestSubtracting8BitRegisterFromAccumulator(cpu,
                a: 0x05, registerValue: 0x04, carryBit: true,
                registerSetter: (value) => cpu.Registers.C = value,
                expectedZero: true,
                expectedCarry: false,
                expectedHalfCarry: false);
        }

        [TestMethod]
        public void Instruction_0x9A_Should_Subtract_D_Plus_Carry_From_A()
        {
            var memory = new Memory(0x9A);
            var cpu = new CPU(new Registers(), memory);

            TestSubtracting8BitRegisterFromAccumulator(cpu,
                a: 0x04, registerValue: 0x05, carryBit: true,
                registerSetter: (value) => cpu.Registers.D = value,
                expectedZero: false,
                expectedCarry: true,
                expectedHalfCarry: true);
        }

        [TestMethod]
        public void Instruction_0x9B_Should_Subtract_E_Plus_Carry_From_A()
        {
            var memory = new Memory(0x9B);
            var cpu = new CPU(new Registers(), memory);

            TestSubtracting8BitRegisterFromAccumulator(cpu,
                a: 0x00, registerValue: 0x00, carryBit: true,
                registerSetter: (value) => cpu.Registers.E = value,
                expectedZero: false,
                expectedCarry: true,
                expectedHalfCarry: true);
        }

        [TestMethod]
        public void Instruction_0x9C_Should_Subtract_H_Plus_Carry_From_A()
        {
            var memory = new Memory(0x9C);
            var cpu = new CPU(new Registers(), memory);

            TestSubtracting8BitRegisterFromAccumulator(cpu,
                a: 0x01, registerValue: 0x00, carryBit: true,
                registerSetter: (value) => cpu.Registers.H = value,
                expectedZero: true,
                expectedCarry: false,
                expectedHalfCarry: false);
        }

        [TestMethod]
        public void Instruction_0x9D_Should_Subtract_L_Plus_Carry_From_A()
        {
            var memory = new Memory(0x9D);
            var cpu = new CPU(new Registers(), memory);

            TestSubtracting8BitRegisterFromAccumulator(cpu,
                a: 0xFF, registerValue: 0x99, carryBit: true,
                registerSetter: (value) => cpu.Registers.L = value,
                expectedZero: false,
                expectedCarry: false,
                expectedHalfCarry: false);
        }

        [TestMethod]
        public void Instruction_0x9E_Should_Subtract_Address_Pointed_To_By_HL_Plus_Carry_From_A()
        {
            var memory = new Memory(0x9E);
            var cpu = new CPU(new Registers() { HL = 0x4000 }, memory);

            TestSubtracting8BitRegisterFromAccumulator(cpu,
                a: 0xBE, registerValue: 0xAE, carryBit: true,
                registerSetter: (value) => memory[cpu.Registers.HL] = value,
                expectedZero: false,
                expectedCarry: false,
                expectedHalfCarry: true);
        }

        [TestMethod]
        public void Instruction_0x9F_Should_Subtract_A_Plus_Carry_From_A()
        {
            var memory = new Memory(0x9F);
            var cpu = new CPU(new Registers(), memory);

            TestSubtracting8BitRegisterFromAccumulator(cpu,
                a: 0x00, registerValue: 0x00, carryBit: false,
                registerSetter: (value) => cpu.Registers.E = value,
                expectedZero: true,
                expectedCarry: false,
                expectedHalfCarry: false);
        }

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
                expectedZero: true);
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
            //sets flags, see https://rednex.github.io/rgbds/gbz80.7.html#XOR_A,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xA9_Should_Bitwise_Exclusive_Or_C_With_A()
        {
            //sets flags, see https://rednex.github.io/rgbds/gbz80.7.html#XOR_A,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xAA_Should_Bitwise_Exclusive_Or_D_With_A()
        {
            //sets flags, see https://rednex.github.io/rgbds/gbz80.7.html#XOR_A,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xAB_Should_Bitwise_Exclusive_Or_E_With_A()
        {
            //sets flags, see https://rednex.github.io/rgbds/gbz80.7.html#XOR_A,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xAC_Should_Bitwise_Exclusive_Or_H_With_A()
        {
            //sets flags, see https://rednex.github.io/rgbds/gbz80.7.html#XOR_A,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xAD_Should_Bitwise_Exclusive_Or_L_With_A()
        {
            //sets flags, see https://rednex.github.io/rgbds/gbz80.7.html#XOR_A,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xAE_Should_Bitwise_Exclusive_Or_Address_Pointed_To_By_HL_With_A()
        {
            //sets flags, see https://rednex.github.io/rgbds/gbz80.7.html#XOR_A,_HL_
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xAF_Should_Bitwise_Exclusive_Or_A_With_A()
        {
            //conventionally used to zero out the accumulator (takes 1 less cycle than ld a, 0)
            //sets flags, see https://rednex.github.io/rgbds/gbz80.7.html#XOR_A,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xB0_Should_Bitwise_Or_B_With_A()
        {
            //sets flags, see https://rednex.github.io/rgbds/gbz80.7.html#OR_A,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xB1_Should_Bitwise_Or_C_With_A()
        {
            //sets flags, see https://rednex.github.io/rgbds/gbz80.7.html#OR_A,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xB2_Should_Bitwise_Or_D_With_A()
        {
            //sets flags, see https://rednex.github.io/rgbds/gbz80.7.html#OR_A,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xB3_Should_Bitwise_Or_E_With_A()
        {
            //sets flags, see https://rednex.github.io/rgbds/gbz80.7.html#OR_A,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xB4_Should_Bitwise_Or_H_With_A()
        {
            //sets flags, see https://rednex.github.io/rgbds/gbz80.7.html#OR_A,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xB5_Should_Bitwise_Or_L_With_A()
        {
            //sets flags, see https://rednex.github.io/rgbds/gbz80.7.html#OR_A,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xB6_Should_Bitwise_Or_Address_Pointed_To_By_HL_With_A()
        {
            //sets flags, see https://rednex.github.io/rgbds/gbz80.7.html#OR_A,_HL_
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xB7_Should_Bitwise_Or_A_With_A()
        {
            //sets flags, see https://rednex.github.io/rgbds/gbz80.7.html#OR_A,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xB8_Should_Compare_B_To_A_And_Set_Flags_As_If_It_Was_Subtracted_From_A()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#CP_A,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xB9_Should_Compare_C_To_A_And_Set_Flags_As_If_It_Was_Subtracted_From_A()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#CP_A,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xBA_Should_Compare_D_To_A_And_Set_Flags_As_If_It_Was_Subtracted_From_A()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#CP_A,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xBB_Should_Compare_E_To_A_And_Set_Flags_As_If_It_Was_Subtracted_From_A()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#CP_A,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xBC_Should_Compare_H_To_A_And_Set_Flags_As_If_It_Was_Subtracted_From_A()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#CP_A,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xBD_Should_Compare_L_To_A_And_Set_Flags_As_If_It_Was_Subtracted_From_A()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#CP_A,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xBE_Should_Compare_Address_Pointed_To_By_HL_To_A_And_Set_Flags_As_If_It_Was_Subtracted_From_A()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#CP_A,_HL_
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xBF_Should_Compare_A_To_A_And_Set_Flags_As_If_It_Was_Subtracted_From_A()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#CP_A,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xC6_Should_Add_8_Bit_Immediate_To_A()
        {
            //sets flags, see https://rednex.github.io/rgbds/gbz80.7.html#ADD_A,n8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCE_Should_Add_8_Bit_Immediate_Plus_Carry_To_A()
        {
            //sets flags, see https://rednex.github.io/rgbds/gbz80.7.html#ADC_A,n8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xD6_Should_Subtract_8_Bit_Immediate_From_A()
        {
            //sets flags, see https://rednex.github.io/rgbds/gbz80.7.html#SUB_A,n8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xDE_Should_Subtract_8_Bit_Immediate_Plus_Carry_From_A()
        {
            //sets flags, see https://rednex.github.io/rgbds/gbz80.7.html#SBC_A,n8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xE6_Should_Bitwise_And_8_Bit_Immediate_With_A()
        {
            //sets flags, see https://rednex.github.io/rgbds/gbz80.7.html#AND_A,n8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xEE_Should_Bitwise_Exclusive_Or_8_Bit_Immediate_With_A()
        {
            //sets flags, see https://rednex.github.io/rgbds/gbz80.7.html#XOR_A,n8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xF6_Should_Bitwise_Or_8_Bit_Immediate_With_A()
        {
            //sets flags, see https://rednex.github.io/rgbds/gbz80.7.html#OR_A,n8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xFE_Should_Compare_8_Bit_Immediate_With_A_And_Set_Flags_As_If_It_Was_Subtracted_From_A()
        {
            //sets flags, see https://rednex.github.io/rgbds/gbz80.7.html#CP_A,n8
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tests instructions like add a, b or adc a, b
        /// </summary>
        private static void TestAdding8BitRegisterToAccumulator(CPU cpu, byte a, byte registerValue, Action<byte> registerSetter, bool expectedZero, bool expectedCarry, bool expectedHalfCarry, bool? carryBit = null)
        {
            cpu.Registers.PC = 0;   //assume the add instruction is always at the beginning of memory
            cpu.Registers.SetFlag(Flags.AddSubtract);   //set the N flag (add instructions should always clear it)
            cpu.Registers.A = a;
            registerSetter(registerValue);
            if (carryBit.HasValue) cpu.Registers.SetFlagTo(Flags.Carry, carryBit.Value);

            cpu.Tick();

            var carry = carryBit.HasValue && carryBit.Value ? 1 : 0;
            Assert.AreEqual((byte)(a + registerValue + carry), cpu.Registers.A);
            Assert.IsFalse(cpu.Registers.HasFlag(Flags.AddSubtract), "Add instructions should always clear the N flag.");

            if (expectedZero)
                Assert.IsTrue(cpu.Registers.HasFlag(Flags.Zero), $"Zero flag should be set when adding {registerValue} to accumulator {a}.");
            else
                Assert.IsFalse(cpu.Registers.HasFlag(Flags.Zero), $"Zero flag should not be set when adding {registerValue} to accumulator {a}.");

            if (expectedCarry)
                Assert.IsTrue(cpu.Registers.HasFlag(Flags.Carry), $"Carry flag should be set when adding {registerValue} to accumulator {a}.");
            else
                Assert.IsFalse(cpu.Registers.HasFlag(Flags.Carry), $"Carry flag should not be set when adding {registerValue} to accumulator {a}.");

            if (expectedHalfCarry)
                Assert.IsTrue(cpu.Registers.HasFlag(Flags.HalfCarry), $"Half carry flag should be set when adding {registerValue} to accumulator {a}.");
            else
                Assert.IsFalse(cpu.Registers.HasFlag(Flags.HalfCarry), $"Half carry flag should not be set when adding {registerValue} to accumulator {a}.");
        }

        /// <summary>
        /// Tests instructions like sub a, b or sbc a, b.
        /// </summary>
        private static void TestSubtracting8BitRegisterFromAccumulator(CPU cpu, byte a, byte registerValue, Action<byte> registerSetter, bool expectedZero, bool expectedCarry, bool expectedHalfCarry, bool? carryBit = null)
        {
            cpu.Registers.PC = 0;   //assume the add instruction is always at the beginning of memory
            cpu.Registers.ClearFlag(Flags.AddSubtract);   //clear the N flag (subtract instructions should always set it)
            cpu.Registers.A = a;
            registerSetter(registerValue);
            if (carryBit.HasValue) cpu.Registers.SetFlagTo(Flags.Carry, carryBit.Value);

            cpu.Tick();

            var carry = carryBit.HasValue && carryBit.Value ? 1 : 0;
            Assert.AreEqual((byte)(a - (registerValue + carry)), cpu.Registers.A);
            Assert.IsTrue(cpu.Registers.HasFlag(Flags.AddSubtract), "Subtract instructions should always set the N flag.");

            if (expectedZero)
                Assert.IsTrue(cpu.Registers.HasFlag(Flags.Zero), $"Zero flag should be set when subtracting {registerValue} from accumulator {a}.");
            else
                Assert.IsFalse(cpu.Registers.HasFlag(Flags.Zero), $"Zero flag should not be set when subtracting {registerValue} from accumulator {a}.");

            if (expectedCarry)
                Assert.IsTrue(cpu.Registers.HasFlag(Flags.Carry), $"Carry flag should be set when subtracting {registerValue} from accumulator {a}.");
            else
                Assert.IsFalse(cpu.Registers.HasFlag(Flags.Carry), $"Carry flag should not be set when subtracting {registerValue} from accumulator {a}.");

            if (expectedHalfCarry)
                Assert.IsTrue(cpu.Registers.HasFlag(Flags.HalfCarry), $"Half carry flag should be set when subtracting {registerValue} from accumulator {a}.");
            else
                Assert.IsFalse(cpu.Registers.HasFlag(Flags.HalfCarry), $"Half carry flag should not be set when subtracting {registerValue} from accumulator {a}.");
        }

        /// <summary>
        /// Tests instructions like and a, b.
        /// </summary>
        private static void TestAnding8BitRegisterWithAccumulator(CPU cpu, byte a, byte registerValue, Action<byte> registerSetter, bool expectedZero)
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
        }

        /// <summary>
        /// Tests instructions like inc b.
        /// </summary>
        /// <see cref="https://rednex.github.io/rgbds/gbz80.7.html#INC_r8"/>
        private static void TestIncrement8BitRegister(CPU cpu, Func<byte> registerUnderTest)
        {
            //loop up since we're incrementing (making sure to cover wraparound)
            for (int i = 0; i <= byte.MaxValue; i++)
            {
                cpu.Tick();

                var expected = (byte)(i + 1);
                Assert.AreEqual(expected, registerUnderTest(), "Expected 8-bit register to increment after executing inc instruction.");

                if (registerUnderTest() == 0)
                {
                    Assert.IsTrue(cpu.Registers.HasFlag(Flags.Zero), $"Expected zero flag to be set when 8-bit register is {registerUnderTest()}.");
                }
                else
                {
                    Assert.IsFalse(cpu.Registers.HasFlag(Flags.Zero), $"Expected zero flag to be cleared when 8-bit register is {registerUnderTest()}.");
                }

                if (registerUnderTest() % 16 == 0)
                {
                    //FF -> 0, F -> 10, 1F -> 20, etc should all set the half carry flag
                    Assert.IsTrue(cpu.Registers.HasFlag(Flags.HalfCarry), $"Expected half carry flag to be set when 8-bit register is incremented to {registerUnderTest()}");
                }
                else
                {
                    Assert.IsFalse(cpu.Registers.HasFlag(Flags.HalfCarry), $"Expected half carry flag to be cleared when 8-bit register is incremented to {registerUnderTest()}");
                }

                Assert.IsFalse(cpu.Registers.HasFlag(Flags.AddSubtract), $"Expected add/subtract flag to be cleared whenever an 8-bit register is incremented.");

                cpu.Registers.PC--;
            }
        }

        /// <summary>
        /// Tests instructions like dec b.
        /// </summary>
        /// <see cref="https://rednex.github.io/rgbds/gbz80.7.html#DEC_r8"/>
        private static void TestDecrement8BitRegister(CPU cpu, Func<byte> registerUnderTest)
        {
            //loop down since we're decrementing (making sure to cover wraparound)
            for (int i = byte.MaxValue; i >= 0; i--)
            {
                cpu.Tick();

                Assert.AreEqual(i, registerUnderTest(), "Expected 8-bit register to decrement after executing dec instruction.");

                if (registerUnderTest() == 0)
                {
                    Assert.IsTrue(cpu.Registers.HasFlag(Flags.Zero), $"Expected zero flag to be set when 8-bit register is {registerUnderTest()}.");
                }
                else
                {
                    Assert.IsFalse(cpu.Registers.HasFlag(Flags.Zero), $"Expected zero flag to be cleared when 8-bit register is {registerUnderTest()}.");
                }

                if ((registerUnderTest() + 1) % 16 == 0)
                {
                    //0 -> FF, F0 -> EF, E0 -> DF, etc should all set the half carry flag
                    Assert.IsTrue(cpu.Registers.HasFlag(Flags.HalfCarry), $"Expected half carry flag to be set when 8-bit register is decremented to {registerUnderTest()}");
                }
                else
                {
                    Assert.IsFalse(cpu.Registers.HasFlag(Flags.HalfCarry), $"Expected half carry flag to be cleared when 8-bit register is decremented to {registerUnderTest()}");
                }

                Assert.IsTrue(cpu.Registers.HasFlag(Flags.AddSubtract), $"Expected add/subtract flag to be set whenever an 8-bit register is decremented.");

                cpu.Registers.PC--;
            }
        }
    }
}
