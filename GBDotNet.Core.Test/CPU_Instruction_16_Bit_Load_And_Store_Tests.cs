using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GBDotNet.Core.Test
{
    [TestClass]
    public class CPU_Instruction_16_Bit_Load_And_Store_Tests
    {
        private const int initialStackPointer = 0x0003;

        [TestMethod]
        public void Instruction_0x01_Should_Load_BC_With_16_Bit_Immediate()
        {
            var memory = new Memory(0x01);
            var cpu = new CPU(new Registers(), memory);
            TestLoadRegisterWith16BitImmediate(cpu, () => cpu.Registers.BC);
        }

        [TestMethod]
        public void Instruction_0x08_Should_Load_Address_With_Stack_Pointer()
        {
            var memory = new Memory(0x08, 0x10, 0xFF);
            var cpu = new CPU(new Registers() { SP = 0xABCD }, memory);

            cpu.Tick();

            Assert.AreEqual(0xCD, memory[0xFF10], "Expected low byte of stack pointer to be stored at given address.");
            Assert.AreEqual(0xAB, memory[0xFF11], "Expected high byte of stack pointer to be stored at given address + 1.");
        }

        [TestMethod]
        public void Instruction_0x11_Should_Load_DE_With_16_Bit_Immediate()
        {
            var memory = new Memory(0x11);
            var cpu = new CPU(new Registers(), memory);
            TestLoadRegisterWith16BitImmediate(cpu, () => cpu.Registers.DE);
        }

        [TestMethod]
        public void Instruction_0x21_Should_Load_HL_With_16_Bit_Immediate()
        {
            var memory = new Memory(0x21);
            var cpu = new CPU(new Registers(), memory);
            TestLoadRegisterWith16BitImmediate(cpu, () => cpu.Registers.HL);
        }

        [TestMethod]
        public void Instruction_0x31_Should_Load_SP_With_16_Bit_Immediate()
        {
            var memory = new Memory(0x31);
            var cpu = new CPU(new Registers(), memory);
            TestLoadRegisterWith16BitImmediate(cpu, () => cpu.Registers.SP);
        }

        [TestMethod]
        public void Instruction_0xC1_Should_Pop_Stack_Into_BC()
        {
            var memory = new Memory(0xC1, 0x00, 0x00, 0xCD, 0xAB);
            var cpu = new CPU(new Registers() { SP = initialStackPointer }, memory);

            cpu.Tick();

            Assert.AreEqual(0xABCD, cpu.Registers.BC);
            Assert.AreEqual(initialStackPointer + 2, cpu.Registers.SP);
        }

        [TestMethod]
        public void Instruction_0xC5_Should_Push_BC_Onto_Stack()
        {
            var memory = new Memory(0xC5);
            var cpu = new CPU(new Registers() { BC = 0xBEEF, SP = initialStackPointer }, memory);

            cpu.Tick();

            CollectionAssert.AreEqual(new byte[] { 0xC5, 0xEF, 0xBE }, memory.Take(3).ToArray());
            Assert.AreEqual(initialStackPointer - 2, cpu.Registers.SP);
        }

        [TestMethod]
        public void Instruction_0xD1_Should_Pop_Stack_Into_DE()
        {
            var memory = new Memory(0xD1, 0x00, 0x34, 0x12);
            var cpu = new CPU(new Registers() { SP = 0x0002 }, memory);

            cpu.Tick();

            Assert.AreEqual(0x1234, cpu.Registers.DE);
            Assert.AreEqual(0x0004, cpu.Registers.SP);
        }

        [TestMethod]
        public void Instruction_0xD5_Should_Push_DE_Onto_Stack()
        {
            var memory = new Memory(0xD5);
            var cpu = new CPU(new Registers() { DE = 0x1234, SP = initialStackPointer }, memory);

            cpu.Tick();

            CollectionAssert.AreEqual(new byte[] { 0xD5, 0x34, 0x12 }, memory.Take(3).ToArray());
            Assert.AreEqual(initialStackPointer - 2, cpu.Registers.SP);
        }

        [TestMethod]
        public void Instruction_0xE1_Should_Pop_Stack_Into_HL()
        {
            var memory = new Memory(0xE1, 0xAD, 0xDE);
            var cpu = new CPU(new Registers() { SP = 0x0001 }, memory);

            cpu.Tick();

            Assert.AreEqual(0xDEAD, cpu.Registers.HL);
            Assert.AreEqual(0x0003, cpu.Registers.SP);
        }

        [TestMethod]
        public void Instruction_0xE5_Should_Push_HL_Onto_Stack()
        {
            var memory = new Memory(0xE5);
            var cpu = new CPU(new Registers(), memory);
            //TODO: finish arranging test

            cpu.Tick();

            //TODO: assertions
        }

        [TestMethod]
        public void Instruction_0xF1_Should_Pop_Stack_Into_AF()
        {
            var memory = new Memory(0xF1, 0xEF, 0xBE);
            var cpu = new CPU(new Registers() { SP = 0x0001 }, memory);

            cpu.Tick();

            Assert.AreEqual(0xBEEF, cpu.Registers.AF);
            Assert.AreEqual(0x0003, cpu.Registers.SP);
        }

        [TestMethod]
        public void Instruction_0xF5_Should_Push_AF_Onto_Stack()
        {
            var memory = new Memory(0xF5);
            var cpu = new CPU(new Registers(), memory);
            //TODO: finish arranging test

            cpu.Tick();

            //TODO: assertions
        }

        [TestMethod]
        public void Instruction_0xF8_Should_Add_8_Bit_Signed_Immediate_To_Stack_Pointer_And_Store_Result_In_HL()
        {
            var memory = new Memory(0xF8);
            var cpu = new CPU(new Registers(), memory);
            //TODO: finish arranging test

            cpu.Tick();

            //TODO: assertions
        }

        [TestMethod]
        public void Instruction_0xF9_Should_Load_Stack_Pointer_From_HL()
        {
            var memory = new Memory(0xF9);
            var cpu = new CPU(new Registers(), memory);
            //TODO: finish arranging test

            cpu.Tick();

            //TODO: assertions
        }

        /// <summary>
        /// Tests instructions like ld de, 9000.
        /// </summary>
        /// <see cref="https://rednex.github.io/rgbds/gbz80.7.html#LD_r16,n16"/>
        private static void TestLoadRegisterWith16BitImmediate(CPU cpu, Func<ushort> registerUnderTest)
        {
            for (int i = 0; i <= byte.MaxValue; i++)
            {
                cpu.Memory[1] = (byte)i;
                for (int j = 0; j <= byte.MaxValue; j++)
                {
                    cpu.Memory[2] = (byte)j;
                    cpu.Tick();
                    Assert.AreEqual((j << 8) | i, registerUnderTest());
                    cpu.Registers.PC -= 3;  //rewind to run again w/ new value
                }
            }
        }
    }
}
