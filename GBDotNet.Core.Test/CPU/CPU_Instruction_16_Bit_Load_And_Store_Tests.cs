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
            Assert.AreEqual(20, cpu.CyclesLastTick);
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
            Assert.AreEqual(12, cpu.CyclesLastTick);
        }

        [TestMethod]
        public void Instruction_0xC5_Should_Push_BC_Onto_Stack()
        {
            var memory = new Memory(0xC5);
            var cpu = new CPU(new Registers() { BC = 0xBEEF, SP = initialStackPointer }, memory);

            cpu.Tick();

            CollectionAssert.AreEqual(new byte[] { 0xC5, 0xEF, 0xBE }, memory.Take(3).ToArray());
            Assert.AreEqual(initialStackPointer - 2, cpu.Registers.SP);
            Assert.AreEqual(16, cpu.CyclesLastTick);
        }

        [TestMethod]
        public void Instruction_0xD1_Should_Pop_Stack_Into_DE()
        {
            var memory = new Memory(0xD1, 0x00, 0x34, 0x12);
            var cpu = new CPU(new Registers() { SP = 0x0002 }, memory);

            cpu.Tick();

            Assert.AreEqual(0x1234, cpu.Registers.DE);
            Assert.AreEqual(0x0004, cpu.Registers.SP);
            Assert.AreEqual(12, cpu.CyclesLastTick);
        }

        [TestMethod]
        public void Instruction_0xD5_Should_Push_DE_Onto_Stack()
        {
            var memory = new Memory(0xD5);
            var cpu = new CPU(new Registers() { DE = 0x1234, SP = initialStackPointer }, memory);

            cpu.Tick();

            CollectionAssert.AreEqual(new byte[] { 0xD5, 0x34, 0x12 }, memory.Take(3).ToArray());
            Assert.AreEqual(initialStackPointer - 2, cpu.Registers.SP);
            Assert.AreEqual(16, cpu.CyclesLastTick);
        }

        [TestMethod]
        public void Instruction_0xE1_Should_Pop_Stack_Into_HL()
        {
            var memory = new Memory(0xE1, 0xAD, 0xDE);
            var cpu = new CPU(new Registers() { SP = 0x0001 }, memory);

            cpu.Tick();

            Assert.AreEqual(0xDEAD, cpu.Registers.HL);
            Assert.AreEqual(0x0003, cpu.Registers.SP);
            Assert.AreEqual(12, cpu.CyclesLastTick);
        }

        [TestMethod]
        public void Instruction_0xE5_Should_Push_HL_Onto_Stack()
        {
            var memory = new Memory(0xE5);
            var cpu = new CPU(new Registers() { HL = 0x4000, SP = initialStackPointer }, memory);

            cpu.Tick();

            CollectionAssert.AreEqual(new byte[] { 0xE5, 0x00, 0x40 }, memory.Take(3).ToArray());
            Assert.AreEqual(initialStackPointer - 2, cpu.Registers.SP);
            Assert.AreEqual(16, cpu.CyclesLastTick);
        }

        [TestMethod]
        public void Instruction_0xF1_Should_Pop_Stack_Into_AF()
        {
            var memory = new Memory(0xF1, 0xEF, 0xBE);
            var cpu = new CPU(new Registers() { SP = 0x0001 }, memory);

            cpu.Tick();

            Assert.AreEqual(0xBEE0, cpu.Registers.AF);  //accuracy: the unused, lower bits of F should *not* be preserved
            Assert.AreEqual(0x0003, cpu.Registers.SP);  //per Blargg_Tests.CPU_Instructions_01_Special_Test_Number_5()
            Assert.AreEqual(12, cpu.CyclesLastTick);
        }

        [TestMethod]
        public void Instruction_0xF5_Should_Push_AF_Onto_Stack()
        {
            var memory = new Memory(0xF5);
            var cpu = new CPU(new Registers() { AF = 0xFFF0, SP = initialStackPointer }, memory);

            cpu.Tick();

            CollectionAssert.AreEqual(new byte[] { 0xF5, 0xF0, 0xFF }, memory.Take(3).ToArray());
            Assert.AreEqual(initialStackPointer - 2, cpu.Registers.SP);
            Assert.AreEqual(16, cpu.CyclesLastTick);
        }

        [TestMethod]
        public void Instruction_0xF8_Should_Add_8_Bit_Signed_Immediate_To_Stack_Pointer_And_Store_Result_In_HL()
        {
            var memory = new Memory(0xF8, 0x00);
            var cpu = new CPU(new Registers() { SP = initialStackPointer }, memory);
            cpu.Registers.SetFlag(Flags.AddSubtract | Flags.Zero);

            //test adding all possible offsets in range [-128, 127] to SP
            for(int i = sbyte.MinValue; i <= sbyte.MaxValue; i++)
            {
                cpu.Memory[1] = (byte)i;
                cpu.Tick();
                var expected = (ushort)(cpu.Registers.SP + i);
                Assert.AreEqual(expected, cpu.Registers.HL);
                Assert.IsFalse(cpu.Registers.HasFlag(Flags.AddSubtract), "ld hl, sp + e8 instruction should clear N flag.");
                Assert.IsFalse(cpu.Registers.HasFlag(Flags.Zero), "ld hl, sp + e8 instruction should clear Z flag.");
                if ((cpu.Registers.SP & 0xF) + (i & 0xF) > 0xF) // this matches the CPU code, but I couldn't think of a better way to express the half carry logic...
                {
                    Assert.IsTrue(cpu.Registers.HasFlag(Flags.HalfCarry), $"For SP = {cpu.Registers.SP}, e8 = {i}, expected half carry flag to be set when SP + e8 overflows from bit 3.");
                }
                else
                {
                    Assert.IsFalse(cpu.Registers.HasFlag(Flags.HalfCarry), $"For SP = {cpu.Registers.SP}, e8 = {i}, expected half carry flag to be cleared when SP + e8 does not overflow from bit 3.");
                }
                if ((cpu.Registers.SP & 0xFF) + i > 0xFF)   // this matches in the CPU code, but I couldn't think of a better way to express the carry logic...
                {
                    Assert.IsTrue(cpu.Registers.HasFlag(Flags.Carry), $"For SP = {cpu.Registers.SP}, e8 = {i}, expected carry flag to be set when SP + e8 overflows from bit 7.");
                }
                else
                {
                    Assert.IsFalse(cpu.Registers.HasFlag(Flags.Carry), $"For SP = {cpu.Registers.SP}, e8 = {i}, expected carry flag to be cleared when SP + e8 does not overflow from bit 7.");
                }

                Assert.AreEqual(12, cpu.CyclesLastTick);

                cpu.Registers.PC -= 2;
            }
        }

        [TestMethod]
        public void Instruction_0xF9_Should_Load_Stack_Pointer_From_HL()
        {
            var memory = new Memory(0xF9);
            var cpu = new CPU(new Registers() { HL = 0xBABE }, memory);

            cpu.Tick();

            Assert.AreEqual(0xBABE, cpu.Registers.SP);
            Assert.AreEqual(8, cpu.CyclesLastTick);
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
                    Assert.AreEqual(12, cpu.CyclesLastTick);
                    cpu.Registers.PC -= 3;  //rewind to run again w/ new value
                }
            }
        }
    }
}
