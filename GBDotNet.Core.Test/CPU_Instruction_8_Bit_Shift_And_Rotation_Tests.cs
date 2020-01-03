﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GBDotNet.Core.Test
{
    [TestClass]
    public class CPU_Instruction_8_Bit_Shift_And_Rotation_Tests
    {
        [TestMethod]
        public void Instruction_0x07_Should_Rotate_A_Left_With_Carry()
        {
            var memory = new Memory(0x07);
            var cpu = new CPU(new Registers() { A = 0b0100_0110 }, memory);
            cpu.Registers.SetFlag(Flags.Zero | Flags.AddSubtract | Flags.HalfCarry);

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
            cpu.Registers.SetFlag(Flags.Zero | Flags.AddSubtract | Flags.HalfCarry);

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
            cpu.Registers.SetFlag(Flags.Zero | Flags.AddSubtract | Flags.HalfCarry | Flags.Carry);

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
            cpu.Registers.SetFlag(Flags.Zero | Flags.AddSubtract | Flags.HalfCarry | Flags.Carry);

            //test by shifting the carry bit all the way across the accumulator:
            //00000000 C:1, then 10000000 C:0, then 01000000 C:0, ..., all the way back to 00000000 C:1
            for (int i = 0; i < 8; i++)
            {
                cpu.Tick();
                Assert.AreEqual((byte)(1 << (7 - i)), cpu.Registers.A, "Accumulator has incorrect value after rra instruction.");
                Assert.IsFalse(cpu.Registers.HasFlag(Flags.Carry), "Carry flag should be cleared after rra instruction.");
                AssertFlagsAreCleared(cpu, Flags.Zero | Flags.AddSubtract | Flags.HalfCarry);
                cpu.Registers.PC--;
            }

            cpu.Tick();
            Assert.AreEqual(0, cpu.Registers.A, "Accumulator should be zero after a full right rotation.");
            Assert.IsTrue(cpu.Registers.HasFlag(Flags.Carry), "Carry flag should be set again after a full right rotation.");
            AssertFlagsAreCleared(cpu, Flags.Zero | Flags.AddSubtract | Flags.HalfCarry);
        }

        [TestMethod]
        public void Instruction_0xCB_0x00_Should_Rotate_B_Left_With_Carry()
        {
            var memory = new Memory(0xCB, 0x00);
            var cpu = new CPU(new Registers() { B = 0b_1010_1010 }, memory);
            cpu.Registers.SetFlag(Flags.Carry);

            cpu.Tick();

            Assert.AreEqual(0b_0101_0101, cpu.Registers.B);
            Assert.IsTrue(cpu.Registers.HasFlag(Flags.Carry));
            Assert.IsFalse(cpu.Registers.HasFlag(Flags.Zero));
            Assert.IsFalse(cpu.Registers.HasFlag(Flags.AddSubtract | Flags.HalfCarry), "rlc b instruction should always clear N and H flags.");
        }

        [TestMethod]
        public void Instruction_0xCB_0x01_Should_Rotate_C_Left_With_Carry()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#RLC_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x02_Should_Rotate_D_Left_With_Carry()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#RLC_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x03_Should_Rotate_E_Left_With_Carry()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#RLC_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x04_Should_Rotate_H_Left_With_Carry()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#RLC_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x05_Should_Rotate_L_Left_With_Carry()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#RLC_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x06_Should_Rotate_Address_Pointed_To_By_HL_Left_With_Carry()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#RLC__HL_
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x07_Should_Rotate_A_Left_With_Carry()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#RLC_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x08_Should_Rotate_B_Right_With_Carry()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#RRC_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x09_Should_Rotate_C_Right_With_Carry()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#RRC_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x0A_Should_Rotate_D_Right_With_Carry()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#RRC_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x0B_Should_Rotate_E_Right_With_Carry()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#RRC_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x0C_Should_Rotate_H_Right_With_Carry()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#RRC_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x0D_Should_Rotate_L_Right_With_Carry()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#RRC_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x0E_Should_Rotate_Address_Pointed_To_By_HL_Right_With_Carry()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#RRC__HL_
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x0F_Should_Rotate_A_Right_With_Carry()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#RRC_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x10_Should_Rotate_B_Left()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#RL_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x11_Should_Rotate_C_Left()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#RL_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x12_Should_Rotate_D_Left()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#RL_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x13_Should_Rotate_E_Left()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#RL_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x14_Should_Rotate_H_Left()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#RL_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x15_Should_Rotate_L_Left()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#RL_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x16_Should_Rotate_Address_Pointed_To_By_HL_Left()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#RL__HL_
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x17_Should_Rotate_A_Left()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#RL_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x18_Should_Rotate_B_Right()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#RR_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x19_Should_Rotate_C_Right()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#RR_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x1A_Should_Rotate_D_Right()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#RR_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x1B_Should_Rotate_E_Right()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#RR_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x1C_Should_Rotate_H_Right()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#RR_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x1D_Should_Rotate_L_Right()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#RR_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x1E_Should_Rotate_Address_Pointed_To_By_HL_Right()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#RR__HL_
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x1F_Should_Rotate_A_Right()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#RL_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x20_Should_Shift_B_Left()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SLA_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x21_Should_Shift_C_Left()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SLA_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x22_Should_Shift_D_Left()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SLA_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x23_Should_Shift_E_Left()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SLA_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x24_Should_Shift_H_Left()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SLA_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x25_Should_Shift_L_Left()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SLA_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x26_Should_Shift_Address_Pointed_To_By_HL_Left()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SLA__HL_
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x27_Should_Shift_A_Left()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SLA_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x28_Should_Arithmetic_Shift_B_Right()
        {
            //arithmetic shifts preserve the sign bit: https://stackoverflow.com/a/6269641
            //https://rednex.github.io/rgbds/gbz80.7.html#SRA_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x29_Should_Arithmetic_Shift_C_Right()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SRA_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x2A_Should_Arithmetic_Shift_D_Right()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SRA_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x2B_Should_Arithmetic_Shift_E_Right()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SRA_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x2C_Should_Arithmetic_Shift_H_Right()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SRA_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x2D_Should_Arithmetic_Shift_L_Right()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SRA_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x2E_Should_Arithmetic_Shift_Address_Pointed_To_By_HL_Right()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SRA__HL_
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x2F_Should_Arithmetic_Shift_A_Right()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SRA_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x30_Should_Swap_Nibbles_In_B()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SWAP_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x31_Should_Swap_Nibbles_In_C()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SWAP_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x32_Should_Swap_Nibbles_In_D()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SWAP_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x33_Should_Swap_Nibbles_In_E()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SWAP_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x34_Should_Swap_Nibbles_In_H()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SWAP_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x35_Should_Swap_Nibbles_In_L()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SWAP_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x36_Should_Swap_Nibbles_In_Address_Pointed_To_By_HL()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SWAP__HL_
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x37_Should_Swap_Nibbles_In_A()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SWAP_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x38_Should_Shift_B_Right()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SRL_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x39_Should_Shift_C_Right()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SRL_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x3A_Should_Shift_D_Right()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SRL_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x3B_Should_Shift_E_Right()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SRL_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x3C_Should_Shift_H_Right()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SRL_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x3D_Should_Shift_L_Right()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SRL_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x3E_Should_Shift_Address_Pointed_To_By_HL_Right()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SRL__HL_
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x3F_Should_Shift_A_Right()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SRL_r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0x40_Should_Test_Bit_0_Of_B_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x40));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.B = value, bitToTest: 0);
        }

        [TestMethod]
        public void Instruction_0xCB_0x41_Should_Test_Bit_0_Of_C_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x41));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.C = value, bitToTest: 0);
        }

        [TestMethod]
        public void Instruction_0xCB_0x42_Should_Test_Bit_0_Of_D_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x42));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.D = value, bitToTest: 0);
        }

        [TestMethod]
        public void Instruction_0xCB_0x43_Should_Test_Bit_0_Of_E_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x43));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.E = value, bitToTest: 0);
        }

        [TestMethod]
        public void Instruction_0xCB_0x44_Should_Test_Bit_0_Of_H_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x44));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.H = value, bitToTest: 0);
        }

        [TestMethod]
        public void Instruction_0xCB_0x45_Should_Test_Bit_0_Of_L_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x45));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.L = value, bitToTest: 0);
        }

        [TestMethod]
        public void Instruction_0xCB_0x46_Should_Test_Bit_0_Of_Address_Pointed_To_By_HL_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers() { HL = 0x4000 }, new Memory(0xCB, 0x46));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Memory[cpu.Registers.HL] = value, bitToTest: 0);
        }

        [TestMethod]
        public void Instruction_0xCB_0x47_Should_Test_Bit_0_Of_A_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x47));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.A = value, bitToTest: 0);
        }

        [TestMethod]
        public void Instruction_0xCB_0x48_Should_Test_Bit_1_Of_B_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x48));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.B = value, bitToTest: 1);
        }

        [TestMethod]
        public void Instruction_0xCB_0x49_Should_Test_Bit_1_Of_C_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x49));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.C = value, bitToTest: 1);
        }

        [TestMethod]
        public void Instruction_0xCB_0x4A_Should_Test_Bit_1_Of_D_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x4A));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.D = value, bitToTest: 1);
        }

        [TestMethod]
        public void Instruction_0xCB_0x4B_Should_Test_Bit_1_Of_E_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x4B));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.E = value, bitToTest: 1);
        }

        [TestMethod]
        public void Instruction_0xCB_0x4C_Should_Test_Bit_1_Of_H_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x4C));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.H = value, bitToTest: 1);
        }

        [TestMethod]
        public void Instruction_0xCB_0x4D_Should_Test_Bit_1_Of_L_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x4D));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.L = value, bitToTest: 1);
        }

        [TestMethod]
        public void Instruction_0xCB_0x4E_Should_Test_Bit_1_Of_Address_Pointed_To_By_HL_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers() { HL = 0x4000 }, new Memory(0xCB, 0x4E));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Memory[cpu.Registers.HL] = value, bitToTest: 1);
        }

        [TestMethod]
        public void Instruction_0xCB_0x4F_Should_Test_Bit_1_Of_A_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x4F));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.A = value, bitToTest: 1);
        }

        [TestMethod]
        public void Instruction_0xCB_0x50_Should_Test_Bit_2_Of_B_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x50));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.B = value, bitToTest: 2);
        }

        [TestMethod]
        public void Instruction_0xCB_0x51_Should_Test_Bit_2_Of_C_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x51));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.C = value, bitToTest: 2);
        }

        [TestMethod]
        public void Instruction_0xCB_0x52_Should_Test_Bit_2_Of_D_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x52));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.D = value, bitToTest: 2);
        }

        [TestMethod]
        public void Instruction_0xCB_0x53_Should_Test_Bit_2_Of_E_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x53));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.E = value, bitToTest: 2);
        }

        [TestMethod]
        public void Instruction_0xCB_0x54_Should_Test_Bit_2_Of_H_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x54));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.H = value, bitToTest: 2);
        }

        [TestMethod]
        public void Instruction_0xCB_0x55_Should_Test_Bit_2_Of_L_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x55));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.L = value, bitToTest: 2);
        }

        [TestMethod]
        public void Instruction_0xCB_0x56_Should_Test_Bit_2_Of_Address_Pointed_To_By_HL_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers() { HL = 0x4000 }, new Memory(0xCB, 0x56));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Memory[cpu.Registers.HL] = value, bitToTest: 2);
        }

        [TestMethod]
        public void Instruction_0xCB_0x57_Should_Test_Bit_2_Of_A_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x57));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.A = value, bitToTest: 2);
        }

        [TestMethod]
        public void Instruction_0xCB_0x58_Should_Test_Bit_3_Of_B_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x58));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.B = value, bitToTest: 3);
        }

        [TestMethod]
        public void Instruction_0xCB_0x59_Should_Test_Bit_3_Of_C_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x59));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.C = value, bitToTest: 3);
        }

        [TestMethod]
        public void Instruction_0xCB_0x5A_Should_Test_Bit_3_Of_D_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x5A));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.D = value, bitToTest: 3);
        }

        [TestMethod]
        public void Instruction_0xCB_0x5B_Should_Test_Bit_3_Of_E_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x5B));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.E = value, bitToTest: 3);
        }

        [TestMethod]
        public void Instruction_0xCB_0x5C_Should_Test_Bit_3_Of_H_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x5C));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.H = value, bitToTest: 3);
        }

        [TestMethod]
        public void Instruction_0xCB_0x5D_Should_Test_Bit_3_Of_L_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x5D));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.L = value, bitToTest: 3);
        }

        [TestMethod]
        public void Instruction_0xCB_0x5E_Should_Test_Bit_3_Of_Address_Pointed_To_By_HL_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers() { HL = 0x4000 }, new Memory(0xCB, 0x5E));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Memory[cpu.Registers.HL] = value, bitToTest: 3);
        }

        [TestMethod]
        public void Instruction_0xCB_0x5F_Should_Test_Bit_3_Of_A_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x5F));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.A = value, bitToTest: 3);
        }

        [TestMethod]
        public void Instruction_0xCB_0x60_Should_Test_Bit_4_Of_B_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x60));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.B = value, bitToTest: 4);
        }

        [TestMethod]
        public void Instruction_0xCB_0x61_Should_Test_Bit_4_Of_C_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x61));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.C = value, bitToTest: 4);
        }

        [TestMethod]
        public void Instruction_0xCB_0x62_Should_Test_Bit_4_Of_D_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x62));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.D = value, bitToTest: 4);
        }

        [TestMethod]
        public void Instruction_0xCB_0x63_Should_Test_Bit_4_Of_E_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x63));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.E = value, bitToTest: 4);
        }

        [TestMethod]
        public void Instruction_0xCB_0x64_Should_Test_Bit_4_Of_H_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x64));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.H = value, bitToTest: 4);
        }

        [TestMethod]
        public void Instruction_0xCB_0x65_Should_Test_Bit_4_Of_L_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x65));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.L = value, bitToTest: 4);
        }

        [TestMethod]
        public void Instruction_0xCB_0x66_Should_Test_Bit_4_Of_Address_Pointed_To_By_HL_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers() { HL = 0x4000 }, new Memory(0xCB, 0x66));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Memory[cpu.Registers.HL] = value, bitToTest: 4);
        }

        [TestMethod]
        public void Instruction_0xCB_0x67_Should_Test_Bit_4_Of_A_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x67));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.A = value, bitToTest: 4);
        }

        [TestMethod]
        public void Instruction_0xCB_0x68_Should_Test_Bit_5_Of_B_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x68));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.B = value, bitToTest: 5);
        }

        [TestMethod]
        public void Instruction_0xCB_0x69_Should_Test_Bit_5_Of_C_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x69));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.C = value, bitToTest: 5);
        }

        [TestMethod]
        public void Instruction_0xCB_0x6A_Should_Test_Bit_5_Of_D_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x6A));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.D = value, bitToTest: 5);
        }

        [TestMethod]
        public void Instruction_0xCB_0x6B_Should_Test_Bit_5_Of_E_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x6B));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.E = value, bitToTest: 5);
        }

        [TestMethod]
        public void Instruction_0xCB_0x6C_Should_Test_Bit_5_Of_H_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x6C));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.H = value, bitToTest: 5);
        }

        [TestMethod]
        public void Instruction_0xCB_0x6D_Should_Test_Bit_5_Of_L_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x6D));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.L = value, bitToTest: 5);
        }

        [TestMethod]
        public void Instruction_0xCB_0x6E_Should_Test_Bit_5_Of_Address_Pointed_To_By_HL_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers() { HL = 0x4000 }, new Memory(0xCB, 0x6E));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Memory[cpu.Registers.HL] = value, bitToTest: 5);
        }

        [TestMethod]
        public void Instruction_0xCB_0x6F_Should_Test_Bit_5_Of_A_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x6F));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.A = value, bitToTest: 5);
        }

        [TestMethod]
        public void Instruction_0xCB_0x70_Should_Test_Bit_6_Of_B_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x70));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.B = value, bitToTest: 6);
        }

        [TestMethod]
        public void Instruction_0xCB_0x71_Should_Test_Bit_6_Of_C_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x71));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.C = value, bitToTest: 6);
        }

        [TestMethod]
        public void Instruction_0xCB_0x72_Should_Test_Bit_6_Of_D_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x72));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.D = value, bitToTest: 6);
        }

        [TestMethod]
        public void Instruction_0xCB_0x73_Should_Test_Bit_6_Of_E_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x73));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.E = value, bitToTest: 6);
        }

        [TestMethod]
        public void Instruction_0xCB_0x74_Should_Test_Bit_6_Of_H_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x74));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.H = value, bitToTest: 6);
        }

        [TestMethod]
        public void Instruction_0xCB_0x75_Should_Test_Bit_6_Of_L_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x75));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.L = value, bitToTest: 6);
        }

        [TestMethod]
        public void Instruction_0xCB_0x76_Should_Test_Bit_6_Of_Address_Pointed_To_By_HL_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers() { HL = 0x4000 }, new Memory(0xCB, 0x76));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Memory[cpu.Registers.HL] = value, bitToTest: 6);
        }

        [TestMethod]
        public void Instruction_0xCB_0x77_Should_Test_Bit_6_Of_A_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x77));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.A = value, bitToTest: 6);
        }

        [TestMethod]
        public void Instruction_0xCB_0x78_Should_Test_Bit_7_Of_B_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x78));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.B = value, bitToTest: 7);
        }

        [TestMethod]
        public void Instruction_0xCB_0x79_Should_Test_Bit_7_Of_C_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x79));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.C = value, bitToTest: 7);
        }

        [TestMethod]
        public void Instruction_0xCB_0x7A_Should_Test_Bit_7_Of_D_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x7A));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.D = value, bitToTest: 7);
        }

        [TestMethod]
        public void Instruction_0xCB_0x7B_Should_Test_Bit_7_Of_E_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x7B));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.E = value, bitToTest: 7);
        }

        [TestMethod]
        public void Instruction_0xCB_0x7C_Should_Test_Bit_7_Of_H_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x7C));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.H = value, bitToTest: 7);
        }

        [TestMethod]
        public void Instruction_0xCB_0x7D_Should_Test_Bit_7_Of_L_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x7D));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.L = value, bitToTest: 7);
        }

        [TestMethod]
        public void Instruction_0xCB_0x7E_Should_Test_Bit_7_Of_Address_Pointed_To_By_HL_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers() { HL = 0x4000 }, new Memory(0xCB, 0x7E));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Memory[cpu.Registers.HL] = value, bitToTest: 7);
        }

        [TestMethod]
        public void Instruction_0xCB_0x7F_Should_Test_Bit_7_Of_A_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x7F));
            TestBitInstruction(cpu, setValueUnderTest: (value) => cpu.Registers.A = value, bitToTest: 7);
        }

        [TestMethod]
        public void Instruction_0xCB_0x80_Should_Reset_Bit_0_Of_B()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x80));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.B,
                setValueUnderTest: (value) => cpu.Registers.B = value, bitToReset: 0);
        }

        [TestMethod]
        public void Instruction_0xCB_0x81_Should_Reset_Bit_0_Of_C()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x81));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.C,
                setValueUnderTest: (value) => cpu.Registers.C = value, bitToReset: 0);
        }

        [TestMethod]
        public void Instruction_0xCB_0x82_Should_Reset_Bit_0_Of_D()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x82));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.D,
                setValueUnderTest: (value) => cpu.Registers.D = value, bitToReset: 0);
        }

        [TestMethod]
        public void Instruction_0xCB_0x83_Should_Reset_Bit_0_Of_E()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x83));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.E,
                setValueUnderTest: (value) => cpu.Registers.E = value, bitToReset: 0);
        }

        [TestMethod]
        public void Instruction_0xCB_0x84_Should_Reset_Bit_0_Of_H()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x84));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.H,
                setValueUnderTest: (value) => cpu.Registers.H = value, bitToReset: 0);
        }

        [TestMethod]
        public void Instruction_0xCB_0x85_Should_Reset_Bit_0_Of_L()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x85));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.L,
                setValueUnderTest: (value) => cpu.Registers.L = value, bitToReset: 0);
        }

        [TestMethod]
        public void Instruction_0xCB_0x86_Should_Reset_Bit_0_Of_Address_Pointed_To_By_HL()
        {
            var cpu = new CPU(new Registers() { HL = 0x4000 }, new Memory(0xCB, 0x86));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Memory[cpu.Registers.HL],
                setValueUnderTest: (value) => cpu.Memory[cpu.Registers.HL] = value, bitToReset: 0);
        }

        [TestMethod]
        public void Instruction_0xCB_0x87_Should_Reset_Bit_0_Of_A()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x87));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.A,
                setValueUnderTest: (value) => cpu.Registers.A = value, bitToReset: 0);
        }

        [TestMethod]
        public void Instruction_0xCB_0x88_Should_Reset_Bit_1_Of_B()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x88));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.B,
                setValueUnderTest: (value) => cpu.Registers.B = value, bitToReset: 1);
        }

        [TestMethod]
        public void Instruction_0xCB_0x89_Should_Reset_Bit_1_Of_C()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x89));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.C,
                setValueUnderTest: (value) => cpu.Registers.C = value, bitToReset: 1);
        }

        [TestMethod]
        public void Instruction_0xCB_0x8A_Should_Reset_Bit_1_Of_D()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x8A));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.D,
                setValueUnderTest: (value) => cpu.Registers.D = value, bitToReset: 1);
        }

        [TestMethod]
        public void Instruction_0xCB_0x8B_Should_Reset_Bit_1_Of_E()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x8B));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.E,
                setValueUnderTest: (value) => cpu.Registers.E = value, bitToReset: 1);
        }

        [TestMethod]
        public void Instruction_0xCB_0x8C_Should_Reset_Bit_1_Of_H()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x8C));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.H,
                setValueUnderTest: (value) => cpu.Registers.H = value, bitToReset: 1);
        }

        [TestMethod]
        public void Instruction_0xCB_0x8D_Should_Reset_Bit_1_Of_L()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x8D));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.L,
                setValueUnderTest: (value) => cpu.Registers.L = value, bitToReset: 1);
        }

        [TestMethod]
        public void Instruction_0xCB_0x8E_Should_Reset_Bit_1_Of_Address_Pointed_To_By_HL()
        {
            var cpu = new CPU(new Registers() { HL = 0x4000 }, new Memory(0xCB, 0x8E));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Memory[cpu.Registers.HL],
                setValueUnderTest: (value) => cpu.Memory[cpu.Registers.HL] = value, bitToReset: 1);
        }

        [TestMethod]
        public void Instruction_0xCB_0x8F_Should_Reset_Bit_1_Of_A()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x8F));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.A,
                setValueUnderTest: (value) => cpu.Registers.A = value, bitToReset: 1);
        }

        [TestMethod]
        public void Instruction_0xCB_0x90_Should_Reset_Bit_2_Of_B()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x90));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.B,
                setValueUnderTest: (value) => cpu.Registers.B = value, bitToReset: 2);
        }

        [TestMethod]
        public void Instruction_0xCB_0x91_Should_Reset_Bit_2_Of_C()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x91));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.C,
                setValueUnderTest: (value) => cpu.Registers.C = value, bitToReset: 2);
        }

        [TestMethod]
        public void Instruction_0xCB_0x92_Should_Reset_Bit_2_Of_D()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x92));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.D,
                setValueUnderTest: (value) => cpu.Registers.D = value, bitToReset: 2);
        }

        [TestMethod]
        public void Instruction_0xCB_0x93_Should_Reset_Bit_2_Of_E()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x93));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.E,
                setValueUnderTest: (value) => cpu.Registers.E = value, bitToReset: 2);
        }

        [TestMethod]
        public void Instruction_0xCB_0x94_Should_Reset_Bit_2_Of_H()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x94));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.H,
                setValueUnderTest: (value) => cpu.Registers.H = value, bitToReset: 2);
        }

        [TestMethod]
        public void Instruction_0xCB_0x95_Should_Reset_Bit_2_Of_L()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x95));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.L,
                setValueUnderTest: (value) => cpu.Registers.L = value, bitToReset: 2);
        }

        [TestMethod]
        public void Instruction_0xCB_0x96_Should_Reset_Bit_2_Of_Address_Pointed_To_By_HL()
        {
            var cpu = new CPU(new Registers() { HL = 0x4000 }, new Memory(0xCB, 0x96));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Memory[cpu.Registers.HL],
                setValueUnderTest: (value) => cpu.Memory[cpu.Registers.HL] = value, bitToReset: 2);
        }

        [TestMethod]
        public void Instruction_0xCB_0x97_Should_Reset_Bit_2_Of_A()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x97));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.A,
                setValueUnderTest: (value) => cpu.Registers.A = value, bitToReset: 2);
        }

        [TestMethod]
        public void Instruction_0xCB_0x98_Should_Reset_Bit_3_Of_B()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x98));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.B,
                setValueUnderTest: (value) => cpu.Registers.B = value, bitToReset: 3);
        }

        [TestMethod]
        public void Instruction_0xCB_0x99_Should_Reset_Bit_3_Of_C()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x99));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.C,
                setValueUnderTest: (value) => cpu.Registers.C = value, bitToReset: 3);
        }

        [TestMethod]
        public void Instruction_0xCB_0x9A_Should_Reset_Bit_3_Of_D()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x9A));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.D,
                setValueUnderTest: (value) => cpu.Registers.D = value, bitToReset: 3);
        }

        [TestMethod]
        public void Instruction_0xCB_0x9B_Should_Reset_Bit_3_Of_E()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x9B));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.E,
                setValueUnderTest: (value) => cpu.Registers.E = value, bitToReset: 3);
        }

        [TestMethod]
        public void Instruction_0xCB_0x9C_Should_Reset_Bit_3_Of_H()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x9C));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.H,
                setValueUnderTest: (value) => cpu.Registers.H = value, bitToReset: 3);
        }

        [TestMethod]
        public void Instruction_0xCB_0x9D_Should_Reset_Bit_3_Of_L()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x9D));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.L,
                setValueUnderTest: (value) => cpu.Registers.L = value, bitToReset: 3);
        }

        [TestMethod]
        public void Instruction_0xCB_0x9E_Should_Reset_Bit_3_Of_Address_Pointed_To_By_HL()
        {
            var cpu = new CPU(new Registers() { HL = 0x4000 }, new Memory(0xCB, 0x9E));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Memory[cpu.Registers.HL],
                setValueUnderTest: (value) => cpu.Memory[cpu.Registers.HL] = value, bitToReset: 3);
        }

        [TestMethod]
        public void Instruction_0xCB_0x9F_Should_Reset_Bit_3_Of_A()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0x9F));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.A,
                setValueUnderTest: (value) => cpu.Registers.A = value, bitToReset: 3);
        }

        [TestMethod]
        public void Instruction_0xCB_0xA0_Should_Reset_Bit_4_Of_B()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0xA0));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.B,
                setValueUnderTest: (value) => cpu.Registers.B = value, bitToReset: 4);
        }

        [TestMethod]
        public void Instruction_0xCB_0xA1_Should_Reset_Bit_4_Of_C()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0xA1));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.C,
                setValueUnderTest: (value) => cpu.Registers.C = value, bitToReset: 4);
        }

        [TestMethod]
        public void Instruction_0xCB_0xA2_Should_Reset_Bit_4_Of_D()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0xA2));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.D,
                setValueUnderTest: (value) => cpu.Registers.D = value, bitToReset: 4);
        }

        [TestMethod]
        public void Instruction_0xCB_0xA3_Should_Reset_Bit_4_Of_E()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0xA3));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.E,
                setValueUnderTest: (value) => cpu.Registers.E = value, bitToReset: 4);
        }

        [TestMethod]
        public void Instruction_0xCB_0xA4_Should_Reset_Bit_4_Of_H()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0xA4));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.H,
                setValueUnderTest: (value) => cpu.Registers.H = value, bitToReset: 4);
        }

        [TestMethod]
        public void Instruction_0xCB_0xA5_Should_Reset_Bit_4_Of_L()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0xA5));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.L,
                setValueUnderTest: (value) => cpu.Registers.L = value, bitToReset: 4);
        }

        [TestMethod]
        public void Instruction_0xCB_0xA6_Should_Reset_Bit_4_Of_Address_Pointed_To_By_HL()
        {
            var cpu = new CPU(new Registers() { HL = 0x4000 }, new Memory(0xCB, 0xA6));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Memory[cpu.Registers.HL],
                setValueUnderTest: (value) => cpu.Memory[cpu.Registers.HL] = value, bitToReset: 4);
        }

        [TestMethod]
        public void Instruction_0xCB_0xA7_Should_Reset_Bit_4_Of_A()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0xA7));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.A,
                setValueUnderTest: (value) => cpu.Registers.A = value, bitToReset: 4);
        }

        [TestMethod]
        public void Instruction_0xCB_0xA8_Should_Reset_Bit_5_Of_B()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0xA8));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.B,
                setValueUnderTest: (value) => cpu.Registers.B = value, bitToReset: 5);
        }

        [TestMethod]
        public void Instruction_0xCB_0xA9_Should_Reset_Bit_5_Of_C()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0xA9));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.C,
                setValueUnderTest: (value) => cpu.Registers.C = value, bitToReset: 5);
        }

        [TestMethod]
        public void Instruction_0xCB_0xAA_Should_Reset_Bit_5_Of_D()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0xAA));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.D,
                setValueUnderTest: (value) => cpu.Registers.D = value, bitToReset: 5);
        }

        [TestMethod]
        public void Instruction_0xCB_0xAB_Should_Reset_Bit_5_Of_E()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0xAB));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.E,
                setValueUnderTest: (value) => cpu.Registers.E = value, bitToReset: 5);
        }

        [TestMethod]
        public void Instruction_0xCB_0xAC_Should_Reset_Bit_5_Of_H()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0xAC));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.H,
                setValueUnderTest: (value) => cpu.Registers.H = value, bitToReset: 5);
        }

        [TestMethod]
        public void Instruction_0xCB_0xAD_Should_Reset_Bit_5_Of_L()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0xAD));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.L,
                setValueUnderTest: (value) => cpu.Registers.L = value, bitToReset: 5);
        }

        [TestMethod]
        public void Instruction_0xCB_0xAE_Should_Reset_Bit_5_Of_Address_Pointed_To_By_HL()
        {
            var cpu = new CPU(new Registers() { HL = 0x4000 }, new Memory(0xCB, 0xAE));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Memory[cpu.Registers.HL],
                setValueUnderTest: (value) => cpu.Memory[cpu.Registers.HL] = value, bitToReset: 5);
        }

        [TestMethod]
        public void Instruction_0xCB_0xAF_Should_Reset_Bit_5_Of_A()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0xAF));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.A,
                setValueUnderTest: (value) => cpu.Registers.A = value, bitToReset: 5);
        }

        [TestMethod]
        public void Instruction_0xCB_0xB0_Should_Reset_Bit_6_Of_B()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0xB0));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.B,
                setValueUnderTest: (value) => cpu.Registers.B = value, bitToReset: 6);
        }

        [TestMethod]
        public void Instruction_0xCB_0xB1_Should_Reset_Bit_6_Of_C()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0xB1));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.C,
                setValueUnderTest: (value) => cpu.Registers.C = value, bitToReset: 6);
        }

        [TestMethod]
        public void Instruction_0xCB_0xB2_Should_Reset_Bit_6_Of_D()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0xB2));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.D,
                setValueUnderTest: (value) => cpu.Registers.D = value, bitToReset: 6);
        }

        [TestMethod]
        public void Instruction_0xCB_0xB3_Should_Reset_Bit_6_Of_E()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0xB3));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.E,
                setValueUnderTest: (value) => cpu.Registers.E = value, bitToReset: 6);
        }

        [TestMethod]
        public void Instruction_0xCB_0xB4_Should_Reset_Bit_6_Of_H()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0xB4));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.H,
                setValueUnderTest: (value) => cpu.Registers.H = value, bitToReset: 6);
        }

        [TestMethod]
        public void Instruction_0xCB_0xB5_Should_Reset_Bit_6_Of_L()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0xB5));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.L,
                setValueUnderTest: (value) => cpu.Registers.L = value, bitToReset: 6);
        }

        [TestMethod]
        public void Instruction_0xCB_0xB6_Should_Reset_Bit_6_Of_Address_Pointed_To_By_HL()
        {
            var cpu = new CPU(new Registers() { HL = 0x4000 }, new Memory(0xCB, 0xB6));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Memory[cpu.Registers.HL],
                setValueUnderTest: (value) => cpu.Memory[cpu.Registers.HL] = value, bitToReset: 6);
        }

        [TestMethod]
        public void Instruction_0xCB_0xB7_Should_Reset_Bit_6_Of_A()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0xB7));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.A,
                setValueUnderTest: (value) => cpu.Registers.A = value, bitToReset: 6);
        }

        [TestMethod]
        public void Instruction_0xCB_0xB8_Should_Reset_Bit_7_Of_B()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0xB8));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.B,
                setValueUnderTest: (value) => cpu.Registers.B = value, bitToReset: 7);
        }

        [TestMethod]
        public void Instruction_0xCB_0xB9_Should_Reset_Bit_7_Of_C()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0xB9));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.C,
                setValueUnderTest: (value) => cpu.Registers.C = value, bitToReset: 7);
        }

        [TestMethod]
        public void Instruction_0xCB_0xBA_Should_Reset_Bit_7_Of_D()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0xBA));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.D,
                setValueUnderTest: (value) => cpu.Registers.D = value, bitToReset: 7);
        }

        [TestMethod]
        public void Instruction_0xCB_0xBB_Should_Reset_Bit_7_Of_E()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0xBB));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.E,
                setValueUnderTest: (value) => cpu.Registers.E = value, bitToReset: 7);
        }

        [TestMethod]
        public void Instruction_0xCB_0xBC_Should_Reset_Bit_7_Of_H()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0xBC));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.H,
                setValueUnderTest: (value) => cpu.Registers.H = value, bitToReset: 7);
        }

        [TestMethod]
        public void Instruction_0xCB_0xBD_Should_Reset_Bit_7_Of_L()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0xBD));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.L,
                setValueUnderTest: (value) => cpu.Registers.L = value, bitToReset: 7);
        }

        [TestMethod]
        public void Instruction_0xCB_0xBE_Should_Reset_Bit_7_Of_Address_Pointed_To_By_HL()
        {
            var cpu = new CPU(new Registers() { HL = 0x4000 }, new Memory(0xCB, 0xBE));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Memory[cpu.Registers.HL],
                setValueUnderTest: (value) => cpu.Memory[cpu.Registers.HL] = value, bitToReset: 7);
        }

        [TestMethod]
        public void Instruction_0xCB_0xBF_Should_Reset_Bit_7_Of_A()
        {
            var cpu = new CPU(new Registers(), new Memory(0xCB, 0xBF));
            TestResInstruction(cpu, getValueUnderTest: () => cpu.Registers.A,
                setValueUnderTest: (value) => cpu.Registers.A = value, bitToReset: 7);
        }

        [TestMethod]
        public void Instruction_0xCB_0xC0_Should_Set_Bit_0_Of_B()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xC1_Should_Set_Bit_0_Of_C()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xC2_Should_Set_Bit_0_Of_D()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xC3_Should_Set_Bit_0_Of_E()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xC4_Should_Set_Bit_0_Of_H()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xC5_Should_Set_Bit_0_Of_L()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xC6_Should_Set_Bit_0_Of_Address_Pointed_To_By_HL()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,_HL_
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xC7_Should_Set_Bit_0_Of_A()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xC8_Should_Set_Bit_1_Of_B()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xC9_Should_Set_Bit_1_Of_C()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xCA_Should_Set_Bit_1_Of_D()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xCB_Should_Set_Bit_1_Of_E()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xCC_Should_Set_Bit_1_Of_H()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xCD_Should_Set_Bit_1_Of_L()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xCE_Should_Set_Bit_1_Of_Address_Pointed_To_By_HL()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,_HL_
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xCF_Should_Set_Bit_1_Of_A()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xD0_Should_Set_Bit_2_Of_B()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xD1_Should_Set_Bit_2_Of_C()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xD2_Should_Set_Bit_2_Of_D()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xD3_Should_Set_Bit_2_Of_E()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xD4_Should_Set_Bit_2_Of_H()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xD5_Should_Set_Bit_2_Of_L()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xD6_Should_Set_Bit_2_Of_Address_Pointed_To_By_HL()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,_HL_
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xD7_Should_Set_Bit_2_Of_A()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xD8_Should_Set_Bit_3_Of_B()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xD9_Should_Set_Bit_3_Of_C()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xDA_Should_Set_Bit_3_Of_D()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xDB_Should_Set_Bit_3_Of_E()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xDC_Should_Set_Bit_3_Of_H()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xDD_Should_Set_Bit_3_Of_L()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xDE_Should_Set_Bit_3_Of_Address_Pointed_To_By_HL()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,_HL_
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xDF_Should_Set_Bit_3_Of_A()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xE0_Should_Set_Bit_4_Of_B()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xE1_Should_Set_Bit_4_Of_C()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xE2_Should_Set_Bit_4_Of_D()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xE3_Should_Set_Bit_4_Of_E()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xE4_Should_Set_Bit_4_Of_H()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xE5_Should_Set_Bit_4_Of_L()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xE6_Should_Set_Bit_4_Of_Address_Pointed_To_By_HL()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,_HL_
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xE7_Should_Set_Bit_4_Of_A()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xE8_Should_Set_Bit_5_Of_B()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xE9_Should_Set_Bit_5_Of_C()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xEA_Should_Set_Bit_5_Of_D()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xEB_Should_Set_Bit_5_Of_E()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xEC_Should_Set_Bit_5_Of_H()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xED_Should_Set_Bit_5_Of_L()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xEE_Should_Set_Bit_5_Of_Address_Pointed_To_By_HL()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,_HL_
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xEF_Should_Set_Bit_5_Of_A()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xF0_Should_Set_Bit_6_Of_B()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xF1_Should_Set_Bit_6_Of_C()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xF2_Should_Set_Bit_6_Of_D()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xF3_Should_Set_Bit_6_Of_E()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xF4_Should_Set_Bit_6_Of_H()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xF5_Should_Set_Bit_6_Of_L()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xF6_Should_Set_Bit_6_Of_Address_Pointed_To_By_HL()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,_HL_
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xF7_Should_Set_Bit_6_Of_A()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xF8_Should_Set_Bit_7_Of_B()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xF9_Should_Set_Bit_7_Of_C()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xFA_Should_Set_Bit_7_Of_D()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xFB_Should_Set_Bit_7_Of_E()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xFC_Should_Set_Bit_7_Of_H()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xFD_Should_Set_Bit_7_Of_L()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xFE_Should_Set_Bit_7_Of_Address_Pointed_To_By_HL()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,_HL_
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCB_0xFF_Should_Set_Bit_7_Of_A()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tests instructions like bit 0, b
        /// </summary>
        private static void TestBitInstruction(CPU cpu, Action<byte> setValueUnderTest, int bitToTest)
        {
            //start with bit under test set to zero => bit instruction should set zero flag
            setValueUnderTest(0);
            cpu.Tick();

            Assert.IsTrue(cpu.Registers.HasFlag(Flags.Zero), "Expected bit instruction to set zero flag when specified bit is 0.");
            Assert.IsFalse(cpu.Registers.HasFlag(Flags.AddSubtract), "Expected bit instruction to always clear N flag.");
            Assert.IsTrue(cpu.Registers.HasFlag(Flags.HalfCarry), "Expected bit instruction to always set H flag.");

            //set bit under test to 1 => bit instruction should clear zero flag
            setValueUnderTest((byte)(1 << bitToTest));
            cpu.Registers.PC = 0;

            cpu.Tick();

            Assert.IsFalse(cpu.Registers.HasFlag(Flags.Zero), "Expected bit instruction to clear zero flag when specified bit is 1.");
            Assert.IsFalse(cpu.Registers.HasFlag(Flags.AddSubtract), "Expected bit instruction to always clear N flag.");
            Assert.IsTrue(cpu.Registers.HasFlag(Flags.HalfCarry), "Expected bit instruction to always set H flag.");
        }

        /// <summary>
        /// Tests instructions like res 0, b
        /// </summary>
        private static void TestResInstruction(CPU cpu, Func<byte> getValueUnderTest, Action<byte> setValueUnderTest, int bitToReset)
        {
            setValueUnderTest(0xFF);
            cpu.Tick();
            Assert.IsFalse(getValueUnderTest().IsBitSet(bitToReset), "Expected res instruction to set specified bit to zero.");
        }

        private static void AssertFlagsAreCleared(CPU cpu, Flags flags)
        {
            if (flags.HasFlag(Flags.Zero)) Assert.IsFalse(cpu.Registers.HasFlag(Flags.Zero), "Expected instruction to clear Z flag.");
            if (flags.HasFlag(Flags.AddSubtract)) Assert.IsFalse(cpu.Registers.HasFlag(Flags.AddSubtract), "Expected instruction to clear N flag.");
            if (flags.HasFlag(Flags.HalfCarry)) Assert.IsFalse(cpu.Registers.HasFlag(Flags.HalfCarry), "Expected instruction to clear H flag.");
            if (flags.HasFlag(Flags.Carry)) Assert.IsFalse(cpu.Registers.HasFlag(Flags.Carry), "Expected instruction to clear C flag.");
        }
    }
}
