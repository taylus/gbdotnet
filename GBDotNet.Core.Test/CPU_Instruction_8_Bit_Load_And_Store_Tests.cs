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
            var memory = new Memory(0x32);
            var cpu = new CPU(new Registers() { A = 0xAA, HL = 0x0001 }, memory);

            cpu.Tick();

            Assert.AreEqual(0xAA, memory[0x0001]);
            Assert.AreEqual(0x0000, cpu.Registers.HL);
        }

        [TestMethod]
        public void Instruction_0x36_Should_Load_Address_Pointed_To_By_HL_With_8_Bit_Immediate()
        {
            var memory = new Memory(0x36, 0xFF);
            var cpu = new CPU(new Registers() { HL = 0x1000 }, memory);

            cpu.Tick();

            Assert.AreEqual(0xFF, memory[0x1000]);
        }

        [TestMethod]
        public void Instruction_0x3A_Should_Load_A_With_Address_Pointed_To_By_HL_Then_Decrement_HL()
        {
            var memory = new Memory(0x3A, 0xAA);
            var cpu = new CPU(new Registers() { HL = 0x0001 }, memory);

            cpu.Tick();

            Assert.AreEqual(0xAA, cpu.Registers.A);
            Assert.AreEqual(0x0000, cpu.Registers.HL);
        }

        [TestMethod]
        public void Instruction_0x3E_Should_Load_A_With_8_Bit_Immediate()
        {
            var memory = new Memory(0x3E);
            var cpu = new CPU(new Registers(), memory);
            TestLoadRegisterWith8BitImmediate(cpu, () => cpu.Registers.A);
        }

        [TestMethod]
        public void Instruction_0x41_Should_Load_B_From_C()
        {
            var memory = new Memory(0x41);
            var cpu = new CPU(new Registers(), memory);
            TestLoadRegisterFromRegister(cpu, 
                destinationRegisterGetter: () => cpu.Registers.B,
                sourceRegisterSetter: (value) => cpu.Registers.C = value);
        }

        [TestMethod]
        public void Instruction_0x42_Should_Load_B_From_D()
        {
            var memory = new Memory(0x42);
            var cpu = new CPU(new Registers(), memory);
            TestLoadRegisterFromRegister(cpu,
                destinationRegisterGetter: () => cpu.Registers.B,
                sourceRegisterSetter: (value) => cpu.Registers.D = value);
        }

        [TestMethod]
        public void Instruction_0x43_Should_Load_B_From_E()
        {
            var memory = new Memory(0x43);
            var cpu = new CPU(new Registers(), memory);
            TestLoadRegisterFromRegister(cpu,
                destinationRegisterGetter: () => cpu.Registers.B,
                sourceRegisterSetter: (value) => cpu.Registers.E = value);
        }

        [TestMethod]
        public void Instruction_0x44_Should_Load_B_From_H()
        {
            var memory = new Memory(0x44);
            var cpu = new CPU(new Registers(), memory);
            TestLoadRegisterFromRegister(cpu,
                destinationRegisterGetter: () => cpu.Registers.B,
                sourceRegisterSetter: (value) => cpu.Registers.H = value);
        }

        [TestMethod]
        public void Instruction_0x45_Should_Load_B_From_L()
        {
            var memory = new Memory(0x45);
            var cpu = new CPU(new Registers(), memory);
            TestLoadRegisterFromRegister(cpu,
                destinationRegisterGetter: () => cpu.Registers.B,
                sourceRegisterSetter: (value) => cpu.Registers.L = value);
        }

        [TestMethod]
        public void Instruction_0x46_Should_Load_B_From_Address_Pointed_To_By_HL()
        {
            var memory = new Memory(0x46, 0xBB);
            var cpu = new CPU(new Registers() { HL = 0x0001 }, memory);

            cpu.Tick();

            Assert.AreEqual(0xBB, cpu.Registers.B);
        }

        [TestMethod]
        public void Instruction_0x47_Should_Load_B_From_A()
        {
            var memory = new Memory(0x47);
            var cpu = new CPU(new Registers(), memory);
            TestLoadRegisterFromRegister(cpu,
                destinationRegisterGetter: () => cpu.Registers.B,
                sourceRegisterSetter: (value) => cpu.Registers.A = value);
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

        [TestMethod]
        public void Instruction_0x50_Should_Load_D_From_B()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x51_Should_Load_D_From_C()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x52_Should_Load_D_From_D()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x53_Should_Load_D_From_E()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x54_Should_Load_D_From_H()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x55_Should_Load_D_From_L()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x56_Should_Load_D_From_Address_Pointed_To_By_HL()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x57_Should_Load_D_From_A()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x58_Should_Load_E_From_B()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x59_Should_Load_E_From_C()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x5A_Should_Load_E_From_D()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x5B_Should_Load_E_From_E()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x5C_Should_Load_E_From_H()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x5D_Should_Load_E_From_L()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x5E_Should_Load_E_From_Address_Pointed_To_By_HL()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x5F_Should_Load_E_From_A()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x60_Should_Load_H_From_B()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x61_Should_Load_H_From_C()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x62_Should_Load_H_From_D()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x63_Should_Load_H_From_E()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x64_Should_Load_H_From_H()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x65_Should_Load_H_From_L()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x66_Should_Load_H_From_Address_Pointed_To_By_HL()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x67_Should_Load_H_From_A()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x68_Should_Load_L_From_B()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x69_Should_Load_L_From_C()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x6A_Should_Load_L_From_D()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x6B_Should_Load_L_From_E()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x6C_Should_Load_L_From_H()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x6D_Should_Load_L_From_L()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x6E_Should_Load_L_From_Address_Pointed_To_By_HL()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x6F_Should_Load_L_From_A()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x70_Should_Load_Address_Pointed_To_By_HL_With_B()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x71_Should_Load_Address_Pointed_To_By_HL_With_C()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x72_Should_Load_Address_Pointed_To_By_HL_With_D()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x73_Should_Load_Address_Pointed_To_By_HL_With_E()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x74_Should_Load_Address_Pointed_To_By_HL_With_H()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x75_Should_Load_Address_Pointed_To_By_HL_With_L()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x76_Should_Halt_The_Processor()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x77_Should_Load_Address_Pointed_To_By_HL_With_A()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x78_Should_Load_A_From_B()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x79_Should_Load_A_From_C()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x7A_Should_Load_A_From_D()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x7B_Should_Load_A_From_E()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x7C_Should_Load_A_From_H()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x7D_Should_Load_A_From_L()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x7E_Should_Load_A_From_Address_Pointed_To_By_HL()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0x7F_Should_Load_A_From_A()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xE0_Should_Load_A_Into_High_Memory_Address_Offset_By_8_Bit_Immediate()
        {
            //notes for later: immediate is unsigned
            //see https://rednex.github.io/rgbds/gbz80.7.html#LD__$FF00+n8_,A
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xE2_Should_Load_A_Into_High_Memory_Address_Offset_By_C()
        {
            //see https://rednex.github.io/rgbds/gbz80.7.html#LD__$FF00+C_,A
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xEA_Should_Load_Immediate_Memory_Location_From_A()
        {
            //see https://rednex.github.io/rgbds/gbz80.7.html#LD__n16_,A
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xF0_Should_Load_A_From_High_Memory_Address_Offset_By_8_Bit_Immediate()
        {
            //see https://rednex.github.io/rgbds/gbz80.7.html#LD_A,_$FF00+n8_
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xF2_Should_Load_A_From_High_Memory_Address_Offset_By_C()
        {
            //see https://rednex.github.io/rgbds/gbz80.7.html#LD_A,_$FF00+C_
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xFA_Should_Load_A_From_Immediate_Memory_Location()
        {
            //see https://rednex.github.io/rgbds/gbz80.7.html#LD_A,_n16_
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

        /// <summary>
        /// Tests instructions like ld b, c
        /// </summary>
        /// <see cref="https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,r8"/>
        private static void TestLoadRegisterFromRegister(CPU cpu, Func<byte> destinationRegisterGetter, Action<byte> sourceRegisterSetter)
        {
            //test setting the register to all possible byte values
            //assumes a memory layout of address 0 = the instruction
            for (int i = 0; i <= byte.MaxValue; i++)
            {
                sourceRegisterSetter((byte)i);
                cpu.Tick();
                Assert.AreEqual(i, destinationRegisterGetter(), $"Expected destination register to be set to {i} after executing ld instruction from source register w/ value {i}.");
                cpu.Registers.PC--;  //rewind by the size of the instruction
            }
        }
    }
}
