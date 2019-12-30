using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GBDotNet.Core.Test
{
    [TestClass]
    public class CPU_Instruction_Control_Flow_Tests
    {
        [TestMethod]
        public void Instruction_0x18_Should_Relative_Jump_By_Signed_Immediate_Negative_Offset()
        {
            var memory = new Memory(0x18, 0xFE);
            var cpu = new CPU(new Registers(), memory);
            var addressOfJump = cpu.Registers.PC;

            cpu.Tick();

            Assert.AreEqual(addressOfJump, cpu.Registers.PC);
        }

        [TestMethod]
        public void Instruction_0x18_Should_Relative_Jump_By_Signed_Immediate_Positive_Offset()
        {
            var memory = new Memory(0x18, 0x01);
            var cpu = new CPU(new Registers(), memory);
            var addressOfJump = cpu.Registers.PC;

            cpu.Tick();

            Assert.AreEqual(addressOfJump + 3, cpu.Registers.PC);
        }

        [TestMethod]
        public void Instruction_0x20_Should_Relative_Jump_By_Signed_Immediate_If_Zero_Flag_Not_Set()
        {
            //zero flag not set => should jump
            var memory = new Memory(0x20, 0x01);
            var cpu = new CPU(new Registers(), memory);
            var addressOfJump = cpu.Registers.PC;

            cpu.Tick();

            Assert.AreEqual(addressOfJump + 3, cpu.Registers.PC, "Expected jr nz instruction to jump when zero flag is not set.");

            //set zero flag and replay => should not jump
            cpu.Registers.PC = 0;
            cpu.Registers.SetFlag(Flags.Zero);

            cpu.Tick();

            Assert.AreEqual(addressOfJump + 1, cpu.Registers.PC, "Expected jr nz instruction to *not* jump when zero flag is set.");
        }

        [TestMethod]
        public void Instruction_0x28_Should_Relative_Jump_By_Signed_Immediate_If_Zero_Flag_Set()
        {
            //zero flag set => should jump
            var memory = new Memory(0x28, 0x01);
            var cpu = new CPU(new Registers(), memory);
            cpu.Registers.SetFlag(Flags.Zero);
            var addressOfJump = cpu.Registers.PC;

            cpu.Tick();

            Assert.AreEqual(addressOfJump + 3, cpu.Registers.PC, "Expected jr z instruction to jump when zero flag is set.");

            //clear zero flag and replay => should not jump
            cpu.Registers.PC = 0;
            cpu.Registers.ClearFlag(Flags.Zero);

            cpu.Tick();

            Assert.AreEqual(addressOfJump + 1, cpu.Registers.PC, "Expected jr z instruction to *not* jump when zero flag is not set.");
        }

        [TestMethod]
        public void Instruction_0x30_Should_Relative_Jump_By_Signed_Immediate_If_Carry_Flag_Not_Set()
        {
            //carry flag not set => should jump
            var memory = new Memory(0x30, 0x01);
            var cpu = new CPU(new Registers(), memory);
            var addressOfJump = cpu.Registers.PC;

            cpu.Tick();

            Assert.AreEqual(addressOfJump + 3, cpu.Registers.PC, "Expected jr nc instruction to jump when carry flag is not set.");

            //set carry flag and replay => should not jump
            cpu.Registers.PC = 0;
            cpu.Registers.SetFlag(Flags.Carry);

            cpu.Tick();

            Assert.AreEqual(addressOfJump + 1, cpu.Registers.PC, "Expected jr nc instruction to *not* jump when carry flag is set.");
        }

        [TestMethod]
        public void Instruction_0x38_Should_Relative_Jump_By_Signed_Immediate_If_Carry_Flag_Set()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#JR_cc,e8
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xC0_Should_Return_From_Subroutine_If_Zero_Flag_Not_Set()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#RET_cc
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xC2_Should_Jump_To_Immediate_16_Bit_Address_If_Zero_Flag_Not_Set()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#JP_cc,n16
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xC3_Should_Jump_To_Immediate_16_Bit_Address()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#JP_n16
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xC4_Should_Call_Subroutine_At_Immediate_16_Bit_Address_If_Zero_Flag_Not_Set()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#CALL_cc,n16
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xC8_Should_Return_From_Subroutine_If_Zero_Flag_Set()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#RET_cc
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xC9_Should_Return_From_Subroutine()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#RET
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCA_Should_Jump_To_Immediate_16_Bit_Address_If_Zero_Flag_Set()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#JP_cc,n16
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCC_Should_Call_Subroutine_At_Immediate_16_Bit_Address_If_Zero_Flag_Set()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#CALL_cc,n16
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xCD_Should_Call_Subroutine_At_Immediate_16_Bit_Address()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#CALL_n16
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xD0_Should_Return_From_Subroutine_If_Carry_Flag_Not_Set()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#RET_cc
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xD2_Should_Jump_To_Immediate_16_Bit_Address_If_Carry_Flag_Not_Set()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#JP_cc,n16
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xD4_Should_Call_Subroutine_At_Immediate_16_Bit_Address_If_Carry_Flag_Not_Set()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#CALL_cc,n16
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xD8_Should_Return_From_Subroutine_If_Carry_Flag_Set()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#RET_cc
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xDA_Should_Jump_To_Immediate_16_Bit_Address_If_Carry_Flag_Set()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#JP_cc,n16
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xDC_Should_Call_Subroutine_At_Immediate_16_Bit_Address_If_Carry_Flag_Set()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#CALL_cc,n16
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xE9_Should_Jump_To_Address_Pointed_To_By_HL()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#JP_HL
            throw new NotImplementedException();
        }
    }
}
