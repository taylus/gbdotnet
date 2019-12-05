using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GBDotNet.Core.Test
{
    /// <summary>
    /// Tests the CPU's instructions by executing them in controlled environments.
    /// </summary>
    /// <remarks>
    /// TODO: Break up CPU and test classes by types of instructions:
    /// - Loads and stores
    /// - Arithmetic
    /// - Jumps
    /// - etc
    /// <see cref="http://www.devrs.com/gb/files/opcodes.html"/>
    /// </remarks>
    [TestClass]
    public class CPU_Instruction_Tests
    {
        [TestMethod]
        public void Instruction_0x01_Should_Load_BC_With_16_Bit_Immediate()
        {
            var memory = new Memory(0x01);
            var cpu = new CPU(new Registers(), memory);
            TestLoadRegisterWith16BitImmediate(cpu, () => cpu.Registers.BC);
        }

        [TestMethod]
        public void Instruction_0x02_Should_Load_Address_Pointed_To_By_BC_With_A()
        {
            var memory = new Memory(0x02);
            var cpu = new CPU(new Registers() { A = 123, BC = 4000 }, memory);

            cpu.Tick();

            Assert.AreEqual(cpu.Registers.A, memory[cpu.Registers.BC]);
        }

        [TestMethod]
        public void Instruction_0x06_Should_Load_B_With_8_Bit_Immediate()
        {
            var memory = new Memory(0x06);
            var cpu = new CPU(new Registers(), memory);
            TestLoadRegisterWith8BitImmediate(cpu, () => cpu.Registers.B);
        }

        [TestMethod]
        public void Instruction_0x07_Should_Rotate_A_Left_Circular()
        {
            var memory = new Memory(0x07);
            var cpu = new CPU(new Registers() { A = 0b0100_0110 }, memory);
            cpu.Registers.SetFlag(Flags.Zero);
            cpu.Registers.SetFlag(Flags.AddSubtract);
            cpu.Registers.SetFlag(Flags.HalfCarry);

            cpu.Tick();
            Assert.AreEqual(0b1000_1100, cpu.Registers.A, "Accumulator has incorrect value after first rlca instruction.");
            Assert.IsFalse(cpu.Registers.HasFlag(Flags.Carry), "Carry flag should be zero after first rlca instruction.");
            AssertOtherFlagsAreCleared();
            cpu.Registers.PC--;

            cpu.Tick();
            Assert.AreEqual(0b0001_1001, cpu.Registers.A, "Accumulator has incorrect value after second rlca instruction.");
            Assert.IsTrue(cpu.Registers.HasFlag(Flags.Carry), "Carry flag should be set after first rlca instruction.");
            AssertOtherFlagsAreCleared();

            void AssertOtherFlagsAreCleared()
            {
                Assert.IsFalse(cpu.Registers.HasFlag(Flags.Zero), "rlca instruction should clear Z flag.");
                Assert.IsFalse(cpu.Registers.HasFlag(Flags.AddSubtract), "rlca instruction should clear N flag.");
                Assert.IsFalse(cpu.Registers.HasFlag(Flags.HalfCarry), "rlca instruction should clear H flag.");
            }
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
        public void Instruction_0x0A_Should_Load_A_From_Address_Pointed_To_By_BC()
        {
            var memory = new Memory(0x0A, 0x01, 0x02);
            var cpu = new CPU(new Registers(), memory);

            cpu.Tick();
            Assert.AreEqual(0x0A, cpu.Registers.A, $"Accumulator should be set to value at memory address {cpu.Registers.BC}.");

            cpu.Registers.PC = 0;
            cpu.Registers.BC = 1;
            cpu.Tick();
            Assert.AreEqual(0x01, cpu.Registers.A, $"Accumulator should be set to value at memory address {cpu.Registers.BC}.");

            cpu.Registers.PC = 0;
            cpu.Registers.BC = 2;
            cpu.Tick();
            Assert.AreEqual(0x02, cpu.Registers.A, $"Accumulator should be set to value at memory address {cpu.Registers.BC}.");
        }

        [TestMethod]
        public void Instruction_0x0E_Should_Load_C_With_8_Bit_Immediate()
        {
            var memory = new Memory(0x0E);
            var cpu = new CPU(new Registers(), memory);
            TestLoadRegisterWith8BitImmediate(cpu, () => cpu.Registers.C);
        }

        [TestMethod]
        public void Instruction_0x11_Should_Load_DE_With_16_Bit_Immediate()
        {
            var memory = new Memory(0x11);
            var cpu = new CPU(new Registers(), memory);
            TestLoadRegisterWith16BitImmediate(cpu, () => cpu.Registers.DE);
        }

        [TestMethod]
        public void Instruction_0x12_Should_Load_Address_Pointed_To_By_DE_With_A()
        {
            var memory = new Memory(0x12);
            var cpu = new CPU(new Registers() { A = 123, DE = 5000 }, memory);

            cpu.Tick();

            Assert.AreEqual(cpu.Registers.A, memory[cpu.Registers.DE]);
        }

        [TestMethod]
        public void Instruction_0x16_Should_Load_D_With_8_Bit_Immediate()
        {
            var memory = new Memory(0x16);
            var cpu = new CPU(new Registers(), memory);
            TestLoadRegisterWith8BitImmediate(cpu, () => cpu.Registers.D);
        }

        [TestMethod]
        public void Instruction_0x17_Should_Rotate_A_Left()
        {
            var memory = new Memory(0x17);
            var cpu = new CPU(new Registers(), memory);
            cpu.Registers.SetFlag(Flags.Carry);
            cpu.Registers.SetFlag(Flags.Zero);
            cpu.Registers.SetFlag(Flags.AddSubtract);
            cpu.Registers.SetFlag(Flags.HalfCarry);

            //test by shifting the carry bit all the way across the accumulator:
            //00000000 C:1, then 00000001 C:0, then 00000010 C:0, ..., all the way back to 00000000 C:1
            for (int i = 0; i < 8; i++)
            {
                cpu.Tick();
                Assert.AreEqual((byte)(1 << i), cpu.Registers.A, "Accumulator has incorrect value after rla instruction.");
                Assert.IsFalse(cpu.Registers.HasFlag(Flags.Carry), "Carry flag should be cleared after rla instruction.");
                AssertOtherFlagsAreCleared();
                cpu.Registers.PC--;
            }

            cpu.Tick();
            Assert.AreEqual(0, cpu.Registers.A, "Accumulator should be zero after a full left rotation.");
            Assert.IsTrue(cpu.Registers.HasFlag(Flags.Carry), "Carry flag should be set again after a full left rotation.");
            AssertOtherFlagsAreCleared();

            void AssertOtherFlagsAreCleared()
            {
                Assert.IsFalse(cpu.Registers.HasFlag(Flags.Zero), "rlca instruction should clear Z flag.");
                Assert.IsFalse(cpu.Registers.HasFlag(Flags.AddSubtract), "rlca instruction should clear N flag.");
                Assert.IsFalse(cpu.Registers.HasFlag(Flags.HalfCarry), "rlca instruction should clear H flag.");
            }
        }

        [TestMethod]
        public void Instruction_0x1E_Should_Load_E_With_8_Bit_Immediate()
        {
            var memory = new Memory(0x1E);
            var cpu = new CPU(new Registers(), memory);
            TestLoadRegisterWith8BitImmediate(cpu, () => cpu.Registers.E);
        }

        [TestMethod]
        public void Instruction_0x26_Should_Load_H_With_8_Bit_Immediate()
        {
            var memory = new Memory(0x26);
            var cpu = new CPU(new Registers(), memory);
            TestLoadRegisterWith8BitImmediate(cpu, () => cpu.Registers.H);
        }

        [TestMethod]
        public void Instruction_0x2E_Should_Load_L_With_8_Bit_Immediate()
        {
            var memory = new Memory(0x2E);
            var cpu = new CPU(new Registers(), memory);
            TestLoadRegisterWith8BitImmediate(cpu, () => cpu.Registers.L);
        }

        /// <summary>
        /// Tests instructions like ld d, 5.
        /// </summary>
        /// <see cref="https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,n8"/>
        private static void TestLoadRegisterWith8BitImmediate(CPU cpu, Func<byte> registerUnderTest)
        {
            //test setting the register to all possible byte values
            //assumes a memory layout of address 0 = the instruction and address 1 = the immediate value
            for (int i = 0; i <= byte.MaxValue; i++)
            {
                cpu.Memory[1] = (byte)i;
                cpu.Tick();
                Assert.AreEqual(i, registerUnderTest(), $"Expected register to be set to {i} after executing ld instruction w/ 8-bit immediate {i}.");
                cpu.Registers.PC -= 2;  //rewind by the size of the instruction
            }
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
