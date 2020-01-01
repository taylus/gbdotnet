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
            //carry flag set => should jump
            var memory = new Memory(0x38, 0x01);
            var cpu = new CPU(new Registers(), memory);
            cpu.Registers.SetFlag(Flags.Carry);
            var addressOfJump = cpu.Registers.PC;

            cpu.Tick();

            Assert.AreEqual(addressOfJump + 3, cpu.Registers.PC, "Expected jr c instruction to jump when carry flag is set.");

            //clear carry flag and replay => should not jump
            cpu.Registers.PC = 0;
            cpu.Registers.ClearFlag(Flags.Carry);

            cpu.Tick();

            Assert.AreEqual(addressOfJump + 1, cpu.Registers.PC, "Expected jr c instruction to *not* jump when carry flag is not set.");
        }

        [TestMethod]
        public void Instruction_0xC0_Should_Return_From_Subroutine_If_Zero_Flag_Not_Set()
        {
            //zero flag set => should not return
            var memory = new Memory(0xC0, 0x00, 0x40);
            var cpu = new CPU(new Registers() { SP = 0x0001 }, memory);
            cpu.Registers.SetFlag(Flags.Zero);

            cpu.Tick();

            Assert.AreEqual(0x0001, cpu.Registers.PC);

            //clear zero flag and replay => should jump to pushed return address
            cpu.Registers.PC = 0;
            cpu.Registers.ClearFlag(Flags.Zero);

            cpu.Tick();

            Assert.AreEqual(0x4000, cpu.Registers.PC);
        }

        [TestMethod]
        public void Instruction_0xC2_Should_Jump_To_Immediate_16_Bit_Address_If_Zero_Flag_Not_Set()
        {
            //zero flag not set => should jump
            var memory = new Memory(0xC2, 0x00, 0x40);
            var cpu = new CPU(new Registers(), memory);

            cpu.Tick();

            Assert.AreEqual(0x4000, cpu.Registers.PC, "Expected jp nz instruction to jump when zero flag is not set.");

            //set zero flag and replay => should not jump
            cpu.Registers.PC = 0;
            cpu.Registers.SetFlag(Flags.Zero);

            cpu.Tick();

            Assert.AreEqual(0x0003, cpu.Registers.PC, "Expected jp nz instruction to *not* jump when zero flag is set.");
        }

        [TestMethod]
        public void Instruction_0xC3_Should_Jump_To_Immediate_16_Bit_Address()
        {
            var memory = new Memory(0xC3, 0x00, 0x40);
            var cpu = new CPU(new Registers(), memory);

            cpu.Tick();

            Assert.AreEqual(0x4000, cpu.Registers.PC);
        }

        [TestMethod]
        public void Instruction_0xC4_Should_Call_Subroutine_At_Immediate_16_Bit_Address_If_Zero_Flag_Not_Set()
        {
            //zero flag not set => should call subroutine
            var memory = new Memory(0xC4, 0x00, 0x40);
            var cpu = new CPU(new Registers() { SP = 0xFFFE }, memory);
            var initialProgramCounter = cpu.Registers.PC;

            cpu.Tick();

            Assert.AreEqual(0x4000, cpu.Registers.PC);

            var expectedReturnAddress = initialProgramCounter + 3;  //call instructions are 3 bytes long
            var pushedReturnAddress = Common.FromLittleEndian(memory[cpu.Registers.SP], memory[cpu.Registers.SP + 1]);
            Assert.AreEqual(expectedReturnAddress, pushedReturnAddress, "Expected call nz instruction to push correct return address onto stack.");

            //set zero flag and replay => should not call subroutine
            cpu.Registers.PC = 0;
            cpu.Registers.SetFlag(Flags.Zero);

            cpu.Tick();

            Assert.AreEqual(expectedReturnAddress, cpu.Registers.PC, "Expected call nz instruction to *not* call subroutine when zero flag is set.");
        }

        [TestMethod]
        public void Instruction_0xC8_Should_Return_From_Subroutine_If_Zero_Flag_Set()
        {
            //zero flag set => should return from subroutine (jump to return address on stack)
            var memory = new Memory(0xC8);
            var cpu = new CPU(new Registers() { SP = 0xFFFE }, memory);
            cpu.PushOntoStack(0x4000);  //manually push a return address onto the stack
            cpu.Registers.SetFlag(Flags.Zero);

            cpu.Tick();

            Assert.AreEqual(0x4000, cpu.Registers.PC, "Expected ret z instruction to return from subroutine when zero flag is set.");

            //clear zero flag and replay => should not return from subroutine
            cpu.Registers.PC = 0;
            cpu.Registers.ClearFlag(Flags.Zero);

            cpu.Tick();

            Assert.AreEqual(0x0001, cpu.Registers.PC, "Expected ret z instruction to *not* return from subroutine when zero flag is clear.");
        }

        [TestMethod]
        public void Instruction_0xC9_Should_Return_From_Subroutine()
        {
            var memory = new Memory(0xC9);
            var cpu = new CPU(new Registers() { SP = 0xFFFE }, memory);
            cpu.PushOntoStack(0x4000);  //manually push a return address onto the stack

            cpu.Tick();

            Assert.AreEqual(0x4000, cpu.Registers.PC);
        }

        [TestMethod]
        public void Instruction_0xCA_Should_Jump_To_Immediate_16_Bit_Address_If_Zero_Flag_Set()
        {
            //zero flag set => should jump
            var memory = new Memory(0xCA, 0x00, 0x40);
            var cpu = new CPU(new Registers(), memory);
            cpu.Registers.SetFlag(Flags.Zero);

            cpu.Tick();

            Assert.AreEqual(0x4000, cpu.Registers.PC, "Expected jp z instruction to jump to immediate address when zero flag is set.");

            //clear zero flag and replay => should not return from subroutine
            cpu.Registers.PC = 0;
            cpu.Registers.ClearFlag(Flags.Zero);

            cpu.Tick();

            Assert.AreEqual(0x0003, cpu.Registers.PC, "Expected jp z instruction to *not* jump when zero flag is clear.");
        }

        [TestMethod]
        public void Instruction_0xCC_Should_Call_Subroutine_At_Immediate_16_Bit_Address_If_Zero_Flag_Set()
        {
            //zero flag set => should call subroutine
            var memory = new Memory(0xCC, 0x00, 0x40);
            var cpu = new CPU(new Registers() { SP = 0xFFFE }, memory);
            cpu.Registers.SetFlag(Flags.Zero);
            var initialProgramCounter = cpu.Registers.PC;

            cpu.Tick();

            Assert.AreEqual(0x4000, cpu.Registers.PC, "Expected call z instruction to call subroutine at immediate address when zero flag is set.");

            var expectedReturnAddress = initialProgramCounter + 3;  //call instructions are 3 bytes long
            var pushedReturnAddress = Common.FromLittleEndian(memory[cpu.Registers.SP], memory[cpu.Registers.SP + 1]);
            Assert.AreEqual(expectedReturnAddress, pushedReturnAddress, "Expected call z instruction to push correct return address onto stack.");

            //clear zero flag and replay => should not call subroutine
            cpu.Registers.PC = 0;
            cpu.Registers.ClearFlag(Flags.Zero);

            cpu.Tick();

            Assert.AreEqual(expectedReturnAddress, cpu.Registers.PC, "Expected call z instruction to *not* call subroutine when zero flag is clear.");
        }

        [TestMethod]
        public void Instruction_0xCD_Should_Call_Subroutine_At_Immediate_16_Bit_Address()
        {
            var memory = new Memory(0xCD, 0x00, 0x40);
            var cpu = new CPU(new Registers() { SP = 0xFFFE }, memory);
            var initialProgramCounter = cpu.Registers.PC;

            cpu.Tick();

            Assert.AreEqual(0x4000, cpu.Registers.PC);

            var expectedReturnAddress = initialProgramCounter + 3;  //call instructions are 3 bytes long
            var pushedReturnAddress = Common.FromLittleEndian(memory[cpu.Registers.SP], memory[cpu.Registers.SP + 1]);
            Assert.AreEqual(expectedReturnAddress, pushedReturnAddress, "Expected call instruction to push correct return address onto stack.");
        }

        [TestMethod]
        public void Instruction_0xD0_Should_Return_From_Subroutine_If_Carry_Flag_Not_Set()
        {
            //carry flag not set => should return from subroutine
            var memory = new Memory(0xD0);
            var cpu = new CPU(new Registers() { SP = 0xFFFE }, memory);
            cpu.PushOntoStack(0x4000);  //manually push a return address onto the stack

            cpu.Tick();

            Assert.AreEqual(0x4000, cpu.Registers.PC, "Expected ret nc instruction to return from subroutine when carry flag is not set.");

            //set carry flag and replay => should not return from subroutine
            cpu.Registers.PC = 0;
            cpu.Registers.SetFlag(Flags.Carry);

            cpu.Tick();

            Assert.AreEqual(0x0001, cpu.Registers.PC, "Expected ret nc instruction to *not* return from subroutine when carry flag is set.");
        }

        [TestMethod]
        public void Instruction_0xD2_Should_Jump_To_Immediate_16_Bit_Address_If_Carry_Flag_Not_Set()
        {
            //carry flag not set => should jump to address
            var memory = new Memory(0xD2, 0x00, 0x40);
            var cpu = new CPU(new Registers(), memory);

            cpu.Tick();

            Assert.AreEqual(0x4000, cpu.Registers.PC, "Expected jp nc instruction to jump to address when carry flag is not set.");

            //set carry flag and replay => should not jump to address
            cpu.Registers.PC = 0;
            cpu.Registers.SetFlag(Flags.Carry);

            cpu.Tick();

            Assert.AreEqual(0x0003, cpu.Registers.PC, "Expected jp nc instruction to *not* jump to address when carry flag is set.");
        }

        [TestMethod]
        public void Instruction_0xD4_Should_Call_Subroutine_At_Immediate_16_Bit_Address_If_Carry_Flag_Not_Set()
        {
            //carry flag not set => should call subroutine
            var memory = new Memory(0xD4, 0x00, 0x40);
            var cpu = new CPU(new Registers() { SP = 0xFFFE }, memory);
            var initialProgramCounter = cpu.Registers.PC;

            cpu.Tick();

            Assert.AreEqual(0x4000, cpu.Registers.PC);

            var expectedReturnAddress = initialProgramCounter + 3;  //call instructions are 3 bytes long
            var pushedReturnAddress = Common.FromLittleEndian(memory[cpu.Registers.SP], memory[cpu.Registers.SP + 1]);
            Assert.AreEqual(expectedReturnAddress, pushedReturnAddress, "Expected call nc instruction to push correct return address onto stack.");

            //set carry flag and replay => should not call subroutine
            cpu.Registers.PC = 0;
            cpu.Registers.SetFlag(Flags.Carry);

            cpu.Tick();

            Assert.AreEqual(expectedReturnAddress, cpu.Registers.PC, "Expected call nc instruction to *not* call subroutine when carry flag is set.");
        }

        [TestMethod]
        public void Instruction_0xD8_Should_Return_From_Subroutine_If_Carry_Flag_Set()
        {
            //carry flag set => should return from subroutine (jump to return address on stack)
            var memory = new Memory(0xD8);
            var cpu = new CPU(new Registers() { SP = 0xFFFE }, memory);
            cpu.PushOntoStack(0x4000);  //manually push a return address onto the stack
            cpu.Registers.SetFlag(Flags.Carry);

            cpu.Tick();

            Assert.AreEqual(0x4000, cpu.Registers.PC, "Expected ret c instruction to return from subroutine when carry flag is set.");

            //clear carry flag and replay => should not return from subroutine
            cpu.Registers.PC = 0;
            cpu.Registers.ClearFlag(Flags.Carry);

            cpu.Tick();

            Assert.AreEqual(0x0001, cpu.Registers.PC, "Expected ret c instruction to *not* return from subroutine when carry flag is clear.");
        }

        [TestMethod]
        public void Instruction_0xDA_Should_Jump_To_Immediate_16_Bit_Address_If_Carry_Flag_Set()
        {
            //carry flag set => should jump
            var memory = new Memory(0xDA, 0x00, 0x40);
            var cpu = new CPU(new Registers() { SP = 0xFFFE }, memory);
            cpu.Registers.SetFlag(Flags.Carry);

            cpu.Tick();

            Assert.AreEqual(0x4000, cpu.Registers.PC, "Expected jp c instruction to jump to immediate address when zero flag is set.");

            //clear carry flag and replay => should not return from subroutine
            cpu.Registers.PC = 0;
            cpu.Registers.ClearFlag(Flags.Carry);

            cpu.Tick();

            Assert.AreEqual(0x0003, cpu.Registers.PC, "Expected jp c instruction to *not* jump when zero flag is clear.");
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
            var memory = new Memory(0xE9);
            var cpu = new CPU(new Registers() { HL = 0x4000 }, memory);

            cpu.Tick();

            Assert.AreEqual(0x4000, cpu.Registers.PC);
        }
    }
}
