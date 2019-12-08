using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GBDotNet.Core.Test
{
    [TestClass]
    public class CPU_Instruction_8_Bit_Load_And_Store_Tests
    {
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
        public void Instruction_0x1A_Should_Load_A_From_Address_Pointed_To_By_DE()
        {
            var memory = new Memory(0x1A, 0x01, 0x02);
            var cpu = new CPU(new Registers(), memory);

            cpu.Tick();
            Assert.AreEqual(0x1A, cpu.Registers.A, $"Accumulator should be set to value at memory address {cpu.Registers.DE}.");

            cpu.Registers.PC = 0;
            cpu.Registers.DE = 1;
            cpu.Tick();
            Assert.AreEqual(0x01, cpu.Registers.A, $"Accumulator should be set to value at memory address {cpu.Registers.DE}.");

            cpu.Registers.PC = 0;
            cpu.Registers.DE = 2;
            cpu.Tick();
            Assert.AreEqual(0x02, cpu.Registers.A, $"Accumulator should be set to value at memory address {cpu.Registers.DE}.");
        }

        [TestMethod]
        public void Instruction_0x1E_Should_Load_E_With_8_Bit_Immediate()
        {
            var memory = new Memory(0x1E);
            var cpu = new CPU(new Registers(), memory);
            TestLoadRegisterWith8BitImmediate(cpu, () => cpu.Registers.E);
        }

        [TestMethod]
        public void Instruction_0x22_Should_Load_Address_Pointed_To_By_HL_With_A_Then_Increment_HL()
        {
            var memory = new Memory(0x22);
            var cpu = new CPU(new Registers() { A = 0xFF, HL = 0x4000 }, memory);

            cpu.Tick();

            Assert.AreEqual(0xFF, memory[0x4000]);
            Assert.AreEqual(0x4001, cpu.Registers.HL);
        }

        [TestMethod]
        public void Instruction_0x26_Should_Load_H_With_8_Bit_Immediate()
        {
            var memory = new Memory(0x26);
            var cpu = new CPU(new Registers(), memory);
            TestLoadRegisterWith8BitImmediate(cpu, () => cpu.Registers.H);
        }

        [TestMethod]
        public void Instruction_0x2A_Should_Load_A_With_Address_Pointed_To_By_HL_Then_Increment_HL()
        {
            var memory = new Memory(0x2A, 0xFF);
            var cpu = new CPU(new Registers() { HL = 0x0001 }, memory);

            cpu.Tick();

            Assert.AreEqual(0xFF, cpu.Registers.A);
            Assert.AreEqual(0x0002, cpu.Registers.HL);
        }

        [TestMethod]
        public void Instruction_0x2E_Should_Load_L_With_8_Bit_Immediate()
        {
            var memory = new Memory(0x2E);
            var cpu = new CPU(new Registers(), memory);
            TestLoadRegisterWith8BitImmediate(cpu, () => cpu.Registers.L);
        }

        [TestMethod]
        public void Instruction_0x32_Should_Load_Address_Pointed_To_By_HL_With_A_Then_Decrement_HL()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x36_Should_Load_Address_Pointed_To_By_HL_With_8_Bit_Immediate()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x3A_Should_Load_A_With_Address_Pointed_To_By_HL_Then_Decrement_HL()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x3E_Should_Load_A_With_8_Bit_Immediate()
        {
            var memory = new Memory(0x3E);
            var cpu = new CPU(new Registers(), memory);
            TestLoadRegisterWith8BitImmediate(cpu, () => cpu.Registers.A);
        }

        [TestMethod]
        public void Instruction_0x40_Should_Load_B_From_B()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x41_Should_Load_B_From_C()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x42_Should_Load_B_From_D()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x43_Should_Load_B_From_E()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x44_Should_Load_B_From_H()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x45_Should_Load_B_From_L()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x46_Should_Load_B_From_Address_Pointed_To_By_HL()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x47_Should_Load_B_From_A()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x48_Should_Load_C_From_B()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x49_Should_Load_C_From_C()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x4A_Should_Load_C_From_D()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x4B_Should_Load_C_From_E()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x4C_Should_Load_C_From_H()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x4D_Should_Load_C_From_L()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x4E_Should_Load_C_From_Address_Pointed_To_By_HL()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x4F_Should_Load_C_From_A()
        {
            throw new NotImplementedException();
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
    }
}
