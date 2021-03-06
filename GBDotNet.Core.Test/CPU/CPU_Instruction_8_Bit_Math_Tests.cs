﻿using System;
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
            TestIncrement8BitValue(cpu, () => cpu.Registers.B);
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
            TestDecrement8BitValue(cpu, () => cpu.Registers.B);
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
            TestIncrement8BitValue(cpu, () => cpu.Registers.C);
        }

        [TestMethod]
        public void Instruction_0x0D_Should_Decrement_C()
        {
            var memory = new Memory(0x0D);
            var cpu = new CPU(new Registers(), memory);
            TestDecrement8BitValue(cpu, () => cpu.Registers.C);
        }

        [TestMethod]
        public void Instruction_0x14_Should_Increment_D()
        {
            var memory = new Memory(0x14);
            var cpu = new CPU(new Registers(), memory);
            TestIncrement8BitValue(cpu, () => cpu.Registers.D);
        }

        [TestMethod]
        public void Instruction_0x15_Should_Decrement_D()
        {
            var memory = new Memory(0x15);
            var cpu = new CPU(new Registers(), memory);
            TestDecrement8BitValue(cpu, () => cpu.Registers.D);
        }

        [TestMethod]
        public void Instruction_0x1C_Should_Increment_E()
        {
            var memory = new Memory(0x1C);
            var cpu = new CPU(new Registers(), memory);
            TestIncrement8BitValue(cpu, () => cpu.Registers.E);
        }

        [TestMethod]
        public void Instruction_0x1D_Should_Decrement_E()
        {
            var memory = new Memory(0x1D);
            var cpu = new CPU(new Registers(), memory);
            TestDecrement8BitValue(cpu, () => cpu.Registers.E);
        }

        [TestMethod]
        public void Instruction_0x24_Should_Increment_H()
        {
            var memory = new Memory(0x24);
            var cpu = new CPU(new Registers(), memory);
            TestIncrement8BitValue(cpu, () => cpu.Registers.H);
        }

        [TestMethod]
        public void Instruction_0x25_Should_Decrement_H()
        {
            var memory = new Memory(0x25);
            var cpu = new CPU(new Registers(), memory);
            TestDecrement8BitValue(cpu, () => cpu.Registers.H);
        }

        [TestMethod]
        public void Instruction_0x27_Should_Decimal_Adjust_A_For_Binary_Coded_Decimal_Addition()
        {
            var memory = new Memory(0x27);
            var cpu = new CPU(new Registers(), memory);
            cpu.Registers.ClearFlag(Flags.AddSubtract); //N flag clear => addition was last performed
            cpu.Registers.A = 0x60;
            cpu.Registers.A += 0x55;
            const byte expectedSum = 0x15; //60 + 55 = 115 => 15 w/ carry (since we only have room for 2 BCD digits)
            //sum is normally B5 in hex, so the DAA instruction should turn B5 into 15 w/ carry

            cpu.Tick();

            Assert.AreEqual(expectedSum, cpu.Registers.A, "Expected daa instruction to adjust accumulator for binary coded decimal addition.");
            Assert.IsTrue(cpu.Registers.HasFlag(Flags.Carry), "Expected carry flag to be set when BCD adjustment is > 99.");
            Assert.IsFalse(cpu.Registers.HasFlag(Flags.HalfCarry), "Expected daa instruction to always clear half carry flag.");
            Assert.IsFalse(cpu.Registers.HasFlag(Flags.Zero), "Expected daa instruction to set zero flag only when adjusted accumulator is zero.");
            Assert.AreEqual(4, cpu.CyclesLastTick);
        }

        [TestMethod]
        public void Instruction_0x27_Should_Decimal_Adjust_A_For_Binary_Coded_Decimal_Subtraction()
        {
            var memory = new Memory(0x27);
            var cpu = new CPU(new Registers(), memory);
            cpu.Registers.SetFlag(Flags.AddSubtract); //N flag set => subtraction was last performed
            cpu.Registers.A = 0x99;
            cpu.Registers.A -= 0x55;
            const byte expectedDifference = 0x44;

            cpu.Tick();

            Assert.AreEqual(expectedDifference, cpu.Registers.A, "Expected daa instruction to adjust accumulator for binary coded decimal subtraction.");
            Assert.IsFalse(cpu.Registers.HasFlag(Flags.HalfCarry), "Expected daa instruction to always clear half carry flag.");
            Assert.IsFalse(cpu.Registers.HasFlag(Flags.Zero), "Expected daa instruction to set zero flag only when adjusted accumulator is zero.");
            Assert.AreEqual(4, cpu.CyclesLastTick);
        }

        [TestMethod]
        public void Instruction_0x2C_Should_Increment_L()
        {
            var memory = new Memory(0x2C);
            var cpu = new CPU(new Registers(), memory);
            TestIncrement8BitValue(cpu, () => cpu.Registers.L);
        }

        [TestMethod]
        public void Instruction_0x2D_Should_Decrement_L()
        {
            var memory = new Memory(0x2D);
            var cpu = new CPU(new Registers(), memory);
            TestDecrement8BitValue(cpu, () => cpu.Registers.L);
        }

        [TestMethod]
        public void Instruction_0x2F_Should_Bitwise_Complement_A()
        {
            var memory = new Memory(0x2F);
            var cpu = new CPU(new Registers(), memory);

            for (int i = 0; i <= byte.MaxValue; i++)
            {
                cpu.Registers.A = (byte)i;
                cpu.Registers.PC = 0;
                cpu.Tick();

                Assert.AreEqual((byte)~i, cpu.Registers.A);
                Assert.IsTrue(cpu.Registers.HasFlag(Flags.AddSubtract | Flags.HalfCarry), "cpl instruction should always set N and H flags.");
                Assert.AreEqual(4, cpu.CyclesLastTick);
            }
        }

        [TestMethod]
        public void Instruction_0x34_Should_Increment_Value_Pointed_To_By_HL()
        {
            var memory = new Memory(0x34);
            var cpu = new CPU(new Registers() { HL = 0x4000 }, memory);
            TestIncrement8BitValue(cpu, () => memory[cpu.Registers.HL], expectedTicks: 12);
        }

        [TestMethod]
        public void Instruction_0x35_Should_Decrement_Value_Pointed_To_By_HL()
        {
            var memory = new Memory(0x35);
            var cpu = new CPU(new Registers() { HL = 0x4000 }, memory);
            TestDecrement8BitValue(cpu, () => memory[cpu.Registers.HL], expectedTicks: 12);
        }

        [TestMethod]
        public void Instruction_0x37_Should_Set_Carry_Flag()
        {
            var memory = new Memory(0x37);
            var cpu = new CPU(new Registers(), memory);
            cpu.Registers.ClearFlag(Flags.Carry);

            cpu.Tick();

            Assert.IsTrue(cpu.Registers.HasFlag(Flags.Carry), "Expected scf instruction to set carry flag.");
            Assert.IsFalse(cpu.Registers.HasFlag(Flags.AddSubtract | Flags.HalfCarry), "Expected scf instruction to clear N and H flags.");
            Assert.AreEqual(4, cpu.CyclesLastTick);
        }

        [TestMethod]
        public void Instruction_0x3C_Should_Increment_A()
        {
            var memory = new Memory(0x3C);
            var cpu = new CPU(new Registers(), memory);
            TestIncrement8BitValue(cpu, () => cpu.Registers.A);
        }

        [TestMethod]
        public void Instruction_0x3D_Should_Decrement_A()
        {
            var memory = new Memory(0x3D);
            var cpu = new CPU(new Registers(), memory);
            TestDecrement8BitValue(cpu, () => cpu.Registers.A);
        }

        [TestMethod]
        public void Instruction_0x3F_Should_Complement_Carry_Flag()
        {
            var memory = new Memory(0x3F);
            var cpu = new CPU(new Registers(), memory);
            cpu.Registers.ClearFlag(Flags.Carry);

            cpu.Tick();

            Assert.IsTrue(cpu.Registers.HasFlag(Flags.Carry), "Expected ccf instruction to toggle carry flag.");
            Assert.IsFalse(cpu.Registers.HasFlag(Flags.AddSubtract | Flags.HalfCarry), "Expected ccf instruction to clear N and H flags.");
            Assert.AreEqual(4, cpu.CyclesLastTick);

            cpu.Registers.PC--;
            cpu.Tick();

            Assert.IsFalse(cpu.Registers.HasFlag(Flags.Carry), "Expected ccf instruction to toggle carry flag.");
            Assert.IsFalse(cpu.Registers.HasFlag(Flags.AddSubtract | Flags.HalfCarry), "Expected ccf instruction to clear N and H flags.");
            Assert.AreEqual(4, cpu.CyclesLastTick);
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
                expectedHalfCarry: false,
                expectedCycles: 8);
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
                expectedHalfCarry: false,
                expectedCycles: 8);
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
                expectedHalfCarry: true,
                expectedCycles: 8);
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
                expectedHalfCarry: true,
                expectedCycles: 8);
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
        public void Instruction_0xB8_Should_Compare_B_To_A_And_Set_Flags_As_If_It_Was_Subtracted_From_A()
        {
            var memory = new Memory(0xB8);
            var cpu = new CPU(new Registers(), memory);

            TestComparing8BitRegisterToAccumulator(cpu,
                a: 0xBE, registerValue: 0xAF,
                registerSetter: (value) => cpu.Registers.B = value,
                expectedZero: false,
                expectedCarry: false,
                expectedHalfCarry: true);
        }

        [TestMethod]
        public void Instruction_0xB9_Should_Compare_C_To_A_And_Set_Flags_As_If_It_Was_Subtracted_From_A()
        {
            var memory = new Memory(0xB9);
            var cpu = new CPU(new Registers(), memory);

            TestComparing8BitRegisterToAccumulator(cpu,
                a: 0x00, registerValue: 0x00,
                registerSetter: (value) => cpu.Registers.C = value,
                expectedZero: true,
                expectedCarry: false,
                expectedHalfCarry: false);
        }

        [TestMethod]
        public void Instruction_0xBA_Should_Compare_D_To_A_And_Set_Flags_As_If_It_Was_Subtracted_From_A()
        {
            var memory = new Memory(0xBA);
            var cpu = new CPU(new Registers(), memory);

            TestComparing8BitRegisterToAccumulator(cpu,
                a: 0x00, registerValue: 0x01,
                registerSetter: (value) => cpu.Registers.D = value,
                expectedZero: false,
                expectedCarry: true,
                expectedHalfCarry: true);
        }

        [TestMethod]
        public void Instruction_0xBB_Should_Compare_E_To_A_And_Set_Flags_As_If_It_Was_Subtracted_From_A()
        {
            var memory = new Memory(0xBB);
            var cpu = new CPU(new Registers(), memory);

            TestComparing8BitRegisterToAccumulator(cpu,
                a: 0x01, registerValue: 0x01,
                registerSetter: (value) => cpu.Registers.E = value,
                expectedZero: true,
                expectedCarry: false,
                expectedHalfCarry: false);
        }

        [TestMethod]
        public void Instruction_0xBC_Should_Compare_H_To_A_And_Set_Flags_As_If_It_Was_Subtracted_From_A()
        {
            var memory = new Memory(0xBC);
            var cpu = new CPU(new Registers(), memory);

            TestComparing8BitRegisterToAccumulator(cpu,
                a: 0x01, registerValue: 0x00,
                registerSetter: (value) => cpu.Registers.H = value,
                expectedZero: false,
                expectedCarry: false,
                expectedHalfCarry: false);
        }

        [TestMethod]
        public void Instruction_0xBD_Should_Compare_L_To_A_And_Set_Flags_As_If_It_Was_Subtracted_From_A()
        {
            var memory = new Memory(0xBD);
            var cpu = new CPU(new Registers(), memory);

            TestComparing8BitRegisterToAccumulator(cpu,
                a: 0x01, registerValue: 0xFF,
                registerSetter: (value) => cpu.Registers.L = value,
                expectedZero: false,
                expectedCarry: true,
                expectedHalfCarry: true);
        }

        [TestMethod]
        public void Instruction_0xBE_Should_Compare_Address_Pointed_To_By_HL_To_A_And_Set_Flags_As_If_It_Was_Subtracted_From_A()
        {
            var memory = new Memory(0xBE);
            var cpu = new CPU(new Registers() { HL = 0x4000 }, memory);

            TestComparing8BitRegisterToAccumulator(cpu,
                a: 0xAA, registerValue: 0xB1,
                registerSetter: (value) => memory[cpu.Registers.HL] = value,
                expectedZero: false,
                expectedCarry: true,
                expectedHalfCarry: false,
                expectedCycles: 8);
        }

        [TestMethod]
        public void Instruction_0xBF_Should_Compare_A_To_A_And_Set_Flags_As_If_It_Was_Subtracted_From_A()
        {
            var memory = new Memory(0xBF);
            var cpu = new CPU(new Registers(), memory);

            TestComparing8BitRegisterToAccumulator(cpu,
                a: 0xAA, registerValue: 0xAA,
                registerSetter: (value) => cpu.Registers.A = value,
                expectedZero: true,
                expectedCarry: false,
                expectedHalfCarry: false);
        }

        [TestMethod]
        public void Instruction_0xC6_Should_Add_8_Bit_Immediate_To_A()
        {
            var memory = new Memory(0xC6, 0xAA);
            var cpu = new CPU(new Registers() { A = 1 }, memory);

            var expected = (byte)(cpu.Registers.A + memory[1]);
            cpu.Tick();

            Assert.AreEqual(expected, cpu.Registers.A);
            Assert.AreEqual(8, cpu.CyclesLastTick);
        }

        [TestMethod]
        public void Instruction_0xCE_Should_Add_8_Bit_Immediate_Plus_Carry_To_A()
        {
            var memory = new Memory(0xCE, 0xAA);
            var cpu = new CPU(new Registers() { A = 1 }, memory);
            cpu.Registers.SetFlag(Flags.Carry);

            var expected = (byte)(cpu.Registers.A + memory[1] + 1);
            cpu.Tick();

            Assert.AreEqual(expected, cpu.Registers.A);
            Assert.AreEqual(8, cpu.CyclesLastTick);
        }

        [TestMethod]
        public void Instruction_0xD6_Should_Subtract_8_Bit_Immediate_From_A()
        {
            var memory = new Memory(0xD6, 0xAA);
            var cpu = new CPU(new Registers() { A = 0xBB }, memory);

            var expected = (byte)(cpu.Registers.A - memory[1]);
            cpu.Tick();

            Assert.AreEqual(expected, cpu.Registers.A);
            Assert.AreEqual(8, cpu.CyclesLastTick);
        }

        [TestMethod]
        public void Instruction_0xDE_Should_Subtract_8_Bit_Immediate_Plus_Carry_From_A()
        {
            var memory = new Memory(0xDE, 0xAA);
            var cpu = new CPU(new Registers() { A = 0xBB }, memory);
            cpu.Registers.SetFlag(Flags.Carry);

            var expected = (byte)(cpu.Registers.A - memory[1] - 1);
            cpu.Tick();

            Assert.AreEqual(expected, cpu.Registers.A);
            Assert.AreEqual(8, cpu.CyclesLastTick);
        }

        [TestMethod]
        public void Instruction_0xFE_Should_Compare_8_Bit_Immediate_With_A_And_Set_Flags_As_If_It_Was_Subtracted_From_A()
        {
            var memory = new Memory(0xFE, 0xBB);
            var cpu = new CPU(new Registers() { A = 0xAA, F = 0x00 }, memory);

            var expected = cpu.Registers.A;
            cpu.Tick();

            Assert.AreEqual(expected, cpu.Registers.A, "Compare instruction should not alter register A");
            Assert.IsTrue(cpu.Registers.HasFlag(Flags.AddSubtract));
            Assert.IsTrue(cpu.Registers.HasFlag(Flags.Carry));
            Assert.IsTrue(cpu.Registers.HasFlag(Flags.HalfCarry));
            Assert.IsFalse(cpu.Registers.HasFlag(Flags.Zero));
            Assert.AreEqual(8, cpu.CyclesLastTick);
        }

        /// <summary>
        /// Tests instructions like add a, b or adc a, b
        /// </summary>
        private static void TestAdding8BitRegisterToAccumulator(CPU cpu, byte a, byte registerValue, Action<byte> registerSetter, bool expectedZero, bool expectedCarry, bool expectedHalfCarry, bool? carryBit = null, int expectedCycles = 4)
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

            Assert.AreEqual(expectedCycles, cpu.CyclesLastTick);
        }

        /// <summary>
        /// Tests instructions like sub a, b or sbc a, b.
        /// If <paramref name="performSubtraction"/> is false, tests instructions like cp a, b.
        /// </summary>
        private static void TestSubtracting8BitRegisterFromAccumulator(CPU cpu, byte a, byte registerValue, Action<byte> registerSetter,
            bool expectedZero, bool expectedCarry, bool expectedHalfCarry, bool? carryBit = null, bool performSubtraction = true, int expectedCycles = 4)
        {
            cpu.Registers.PC = 0;   //assume the add instruction is always at the beginning of memory
            cpu.Registers.ClearFlag(Flags.AddSubtract);   //clear the N flag (subtract instructions should always set it)
            cpu.Registers.A = a;
            registerSetter(registerValue);
            if (carryBit.HasValue) cpu.Registers.SetFlagTo(Flags.Carry, carryBit.Value);

            cpu.Tick();

            var carry = carryBit.HasValue && carryBit.Value ? 1 : 0;
            var expected = performSubtraction ? (byte)(a - (registerValue + carry)) : a;    //compare instructions shouldn't store the result
            Assert.AreEqual(expected, cpu.Registers.A);
            Assert.IsTrue(cpu.Registers.HasFlag(Flags.AddSubtract), "Subtract/compare instructions should always set the N flag.");

            if (expectedZero)
                Assert.IsTrue(cpu.Registers.HasFlag(Flags.Zero), $"Zero flag should be set when subtracting from or comparing {registerValue} w/ accumulator {a}.");
            else
                Assert.IsFalse(cpu.Registers.HasFlag(Flags.Zero), $"Zero flag should not be set when subtracting from or comparing {registerValue} w/ accumulator {a}.");

            if (expectedCarry)
                Assert.IsTrue(cpu.Registers.HasFlag(Flags.Carry), $"Carry flag should be set when subtracting from or comparing {registerValue} w/ accumulator {a}.");
            else
                Assert.IsFalse(cpu.Registers.HasFlag(Flags.Carry), $"Carry flag should not be set when subtracting from or comparing {registerValue} w/ accumulator {a}.");

            if (expectedHalfCarry)
                Assert.IsTrue(cpu.Registers.HasFlag(Flags.HalfCarry), $"Half carry flag should be set when subtracting from or comparing {registerValue} w/ accumulator {a}.");
            else
                Assert.IsFalse(cpu.Registers.HasFlag(Flags.HalfCarry), $"Half carry flag should not be set when subtracting from or comparing {registerValue} w/ accumulator {a}.");

            Assert.AreEqual(expectedCycles, cpu.CyclesLastTick);
        }

        /// <summary>
        /// Tests instructions like cp a, b. These instructions behave just like subtraction (set the same flags) but don't store the result in the accumulator.
        /// </summary>
        private static void TestComparing8BitRegisterToAccumulator(CPU cpu, byte a, byte registerValue, Action<byte> registerSetter, bool expectedZero, bool expectedCarry, bool expectedHalfCarry, int expectedCycles = 4)
        {
            TestSubtracting8BitRegisterFromAccumulator(cpu, a, registerValue, registerSetter, expectedZero, expectedCarry, expectedHalfCarry, null, performSubtraction: false, expectedCycles: expectedCycles);
        }

        /// <summary>
        /// Tests instructions like inc b.
        /// </summary>
        /// <see cref="https://rednex.github.io/rgbds/gbz80.7.html#INC_r8"/>
        private static void TestIncrement8BitValue(CPU cpu, Func<byte> getterForValueBeingIncremented, int expectedTicks = 4)
        {
            //loop up since we're incrementing (making sure to cover wraparound)
            for (int i = 0; i <= byte.MaxValue; i++)
            {
                cpu.Tick();

                var expected = (byte)(i + 1);
                Assert.AreEqual(expected, getterForValueBeingIncremented(), "Expected 8-bit value to increment after executing inc instruction.");

                if (getterForValueBeingIncremented() == 0)
                {
                    Assert.IsTrue(cpu.Registers.HasFlag(Flags.Zero), $"Expected zero flag to be set when 8-bit value is incremented to {getterForValueBeingIncremented()}.");
                }
                else
                {
                    Assert.IsFalse(cpu.Registers.HasFlag(Flags.Zero), $"Expected zero flag to be cleared when 8-bit register is incremented {getterForValueBeingIncremented()}.");
                }

                if (getterForValueBeingIncremented() % 16 == 0)
                {
                    //FF -> 0, F -> 10, 1F -> 20, etc should all set the half carry flag
                    Assert.IsTrue(cpu.Registers.HasFlag(Flags.HalfCarry), $"Expected half carry flag to be set when 8-bit value is incremented to {getterForValueBeingIncremented()}");
                }
                else
                {
                    Assert.IsFalse(cpu.Registers.HasFlag(Flags.HalfCarry), $"Expected half carry flag to be cleared when 8-bit value is incremented to {getterForValueBeingIncremented()}");
                }

                Assert.IsFalse(cpu.Registers.HasFlag(Flags.AddSubtract), $"Expected add/subtract flag to be cleared whenever an 8-bit value is incremented.");

                Assert.AreEqual(expectedTicks, cpu.CyclesLastTick);

                cpu.Registers.PC--;
            }
        }

        /// <summary>
        /// Tests instructions like dec b.
        /// </summary>
        /// <see cref="https://rednex.github.io/rgbds/gbz80.7.html#DEC_r8"/>
        private static void TestDecrement8BitValue(CPU cpu, Func<byte> getterForValueBeingDecremented, int expectedTicks = 4)
        {
            //loop down since we're decrementing (making sure to cover wraparound)
            for (int i = byte.MaxValue; i >= 0; i--)
            {
                cpu.Tick();

                Assert.AreEqual(i, getterForValueBeingDecremented(), "Expected 8-bit value to decrement after executing dec instruction.");

                if (getterForValueBeingDecremented() == 0)
                {
                    Assert.IsTrue(cpu.Registers.HasFlag(Flags.Zero), $"Expected zero flag to be set when 8-bit value is decremented to {getterForValueBeingDecremented()}.");
                }
                else
                {
                    Assert.IsFalse(cpu.Registers.HasFlag(Flags.Zero), $"Expected zero flag to be cleared when 8-bit value is decremented to {getterForValueBeingDecremented()}.");
                }

                if ((getterForValueBeingDecremented() + 1) % 16 == 0)
                {
                    //0 -> FF, F0 -> EF, E0 -> DF, etc should all set the half carry flag
                    Assert.IsTrue(cpu.Registers.HasFlag(Flags.HalfCarry), $"Expected half carry flag to be set when 8-bit value is decremented to {getterForValueBeingDecremented()}");
                }
                else
                {
                    Assert.IsFalse(cpu.Registers.HasFlag(Flags.HalfCarry), $"Expected half carry flag to be cleared when 8-bit value is decremented to {getterForValueBeingDecremented()}");
                }

                Assert.IsTrue(cpu.Registers.HasFlag(Flags.AddSubtract), $"Expected add/subtract flag to be set whenever an 8-bit value is decremented.");

                Assert.AreEqual(expectedTicks, cpu.CyclesLastTick);

                cpu.Registers.PC--;
            }
        }
    }
}
