using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GBDotNet.Core.Test
{
    [TestClass]
    public class CPU_Instructions
    {
        [TestMethod]
        public void Instruction_0x01_Should_Load_BC_With_16_Bit_Immediate()
        {
            var memory = new Memory(0x01);
            var cpu = new CPU(new Registers(), memory);

            for (int i = 0; i <= byte.MaxValue; i++)
            {
                memory[1] = (byte)i;
                for (int j = 0; j <= byte.MaxValue; j++)
                {
                    memory[2] = (byte)j;
                    cpu.Tick();
                    Assert.AreEqual((j << 8) | i, cpu.Registers.BC);
                    cpu.Registers.PC -= 3;  //rewind to run again w/ new value
                }
            }
        }

        [TestMethod]
        public void Instruction_0x02_Should_Load_Address_Pointed_To_By_BC_With_A()
        {
            var memory = new Memory(0x02);
            var cpu = new CPU(new Registers(), memory);

            cpu.Registers.BC = 5000;
            cpu.Registers.A = 100;
            cpu.Tick();

            Assert.AreEqual(cpu.Registers.A, memory[cpu.Registers.BC]);
        }

        [TestMethod]
        public void Instruction_0x03_Should_Increment_BC()
        {
            var memory = new Memory(0x03);
            var cpu = new CPU(new Registers(), memory);

            for (int i = 0; i <= ushort.MaxValue; i++)
            {
                cpu.Tick();
                var expected = (i + 1) & ushort.MaxValue;
                Assert.AreEqual(expected, cpu.Registers.BC);
                cpu.Registers.PC--;
            }
        }

        [TestMethod]
        public void Instruction_0x04_Should_Increment_B()
        {
            var memory = new Memory(0x04);
            var cpu = new CPU(new Registers(), memory);
            TestIncrement8BitRegister(cpu, () => cpu.Registers.B);
        }

        [TestMethod]
        public void Instruction_0x05_Should_Decrement_B()
        {
            var memory = new Memory(0x05);
            var cpu = new CPU(new Registers() { B = 255 }, memory);

            //loop down since we're decrementing (making sure to cover wraparound)
            for (int i = byte.MaxValue; i >= 0; i--)
            {
                cpu.Tick();

                var expected = (i - 1) & byte.MaxValue;
                Assert.AreEqual(expected, cpu.Registers.B);

                if (cpu.Registers.B == 0)
                {
                    Assert.IsTrue(cpu.Registers.HasFlag(Flags.Zero), $"Expected zero flag to be set when B is {cpu.Registers.B}.");
                }
                else
                {
                    Assert.IsFalse(cpu.Registers.HasFlag(Flags.Zero), $"Expected zero flag to be cleared when B is {cpu.Registers.B}.");
                }

                if (cpu.Registers.B % 16 == 0)
                {
                    Assert.IsTrue(cpu.Registers.HasFlag(Flags.HalfCarry), $"Expected half carry flag to be set when B is decremented to {cpu.Registers.B}");
                }
                else
                {
                    Assert.IsFalse(cpu.Registers.HasFlag(Flags.HalfCarry), $"Expected half carry flag to be cleared when B is decremented to {cpu.Registers.B}");
                }

                Assert.IsTrue(cpu.Registers.HasFlag(Flags.AddSubtract), $"Expected add/subtract flag to be set whenever an 8-bit register is decremented.");

                cpu.Registers.PC--;
            }
        }

        [TestMethod]
        public void Instruction_0x06_Should_Load_B_With_8_Bit_Immediate()
        {
            var memory = new Memory(0x06);
            var cpu = new CPU(new Registers(), memory);

            //test setting the register to all possible byte values
            for (int i = 0; i <= byte.MaxValue; i++)
            {
                cpu.Memory[1] = (byte)i;
                cpu.Tick();
                Assert.AreEqual(i, cpu.Registers.B, $"Expected register B to be set to {i} after executing ld b, {i}.");
                cpu.Registers.PC -= 2;  //rewind by the size of this instruction
            }
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
        public void Instruction_0x09_Should_Add_BC_To_HL()
        {
            var memory = new Memory(0x09);
            var cpu = new CPU(new Registers(), memory);

            TestCaseWithCarries();
            TestCaseWithoutCarries();

            void TestCaseWithoutCarries()
            {
                TestCase(bc: 0b1010_0000_0101_0101, 
                         hl: 0b0001_0000_0100_0000, 
                         expectedHalfCarry: false,
                         expectedCarry: false);
            }
            
            void TestCaseWithCarries()
            {
                TestCase(bc: 0b1010_1100_0101_0101,
                         hl: 0b1001_0100_0100_0000,
                         expectedHalfCarry: true,
                         expectedCarry: true);
            }

            void TestCase(ushort bc, ushort hl, bool expectedHalfCarry, bool expectedCarry)
            {
                cpu.Registers.PC = 0;
                cpu.Registers.BC = bc;
                cpu.Registers.HL = hl;
                cpu.Registers.SetFlag(Flags.AddSubtract);

                cpu.Tick();

                Assert.AreEqual((ushort)(bc + hl), cpu.Registers.HL);
                Assert.IsFalse(cpu.Registers.HasFlag(Flags.AddSubtract), "add hl, bc instruction should clear N flag.");

                if (expectedHalfCarry)
                    Assert.IsTrue(cpu.Registers.HasFlag(Flags.HalfCarry), "Half carry flag should be set.");
                else
                    Assert.IsFalse(cpu.Registers.HasFlag(Flags.HalfCarry), "Half carry flag should not be set.");

                if (expectedCarry)
                    Assert.IsTrue(cpu.Registers.HasFlag(Flags.Carry), "Carry flag should be set.");
                else
                    Assert.IsFalse(cpu.Registers.HasFlag(Flags.Carry), "Carry flag should not be set.");
            }
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
        public void Instruction_0x0B_Should_Decrement_BC()
        {
            var memory = new Memory(0x0B);
            var cpu = new CPU(new Registers(), memory);

            for (int i = ushort.MaxValue; i >= 0; i--)
            {
                cpu.Tick();
                Assert.AreEqual(i, cpu.Registers.BC);
                cpu.Registers.PC--;
            }
        }

        [TestMethod]
        public void Instruction_0x0C_Should_Increment_C()
        {
            var memory = new Memory(0x0C);
            var cpu = new CPU(new Registers(), memory);
            TestIncrement8BitRegister(cpu, () => cpu.Registers.C);
        }

        [TestMethod]
        public void Instruction_0x14_Should_Increment_D()
        {
            var memory = new Memory(0x14);
            var cpu = new CPU(new Registers(), memory);
            TestIncrement8BitRegister(cpu, () => cpu.Registers.D);
        }

        [TestMethod]
        public void Instruction_0x1C_Should_Increment_E()
        {
            var memory = new Memory(0x1C);
            var cpu = new CPU(new Registers(), memory);
            TestIncrement8BitRegister(cpu, () => cpu.Registers.E);
        }

        [TestMethod]
        public void Instruction_0x24_Should_Increment_H()
        {
            var memory = new Memory(0x24);
            var cpu = new CPU(new Registers(), memory);
            TestIncrement8BitRegister(cpu, () => cpu.Registers.H);
        }

        [TestMethod]
        public void Instruction_0x2C_Should_Increment_L()
        {
            var memory = new Memory(0x2C);
            var cpu = new CPU(new Registers(), memory);
            TestIncrement8BitRegister(cpu, () => cpu.Registers.L);
        }

        [TestMethod]
        public void Instruction_0x3C_Should_Increment_H()
        {
            var memory = new Memory(0x3C);
            var cpu = new CPU(new Registers(), memory);
            TestIncrement8BitRegister(cpu, () => cpu.Registers.A);
        }

        private void TestIncrement8BitRegister(CPU cpu, Func<byte> registerUnderTest)
        {
            //loop up since we're incrementing (making sure to cover wraparound)
            for (int i = 0; i <= byte.MaxValue; i++)
            {
                cpu.Tick();

                var expected = (i + 1) & byte.MaxValue;
                Assert.AreEqual(expected, registerUnderTest(), "Expected 8-bit register to increment after executing INC instruction.");

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
    }
}
