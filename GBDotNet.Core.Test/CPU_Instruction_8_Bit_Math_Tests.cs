﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GBDotNet.Core.Test
{
    /// <summary>
    /// Tests the CPU's 8-bit math instructions by executing them in controlled environments.
    /// </summary>
    [TestClass]
    public class CPU_Instruction_8_Bit_Math_Tests
    {
        [TestMethod]
        public void Instruction_0x04_Should_Increment_B()
        {
            var memory = new Memory(0x04);
            var cpu = new CPU(new Registers(), memory);
            TestIncrement8BitRegister(cpu, () => cpu.Registers.B);
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
            TestDecrement8BitRegister(cpu, () => cpu.Registers.B);
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
            TestIncrement8BitRegister(cpu, () => cpu.Registers.C);
        }

        [TestMethod]
        public void Instruction_0x0D_Should_Decrement_C()
        {
            var memory = new Memory(0x0D);
            var cpu = new CPU(new Registers(), memory);
            TestDecrement8BitRegister(cpu, () => cpu.Registers.C);
        }

        [TestMethod]
        public void Instruction_0x14_Should_Increment_D()
        {
            var memory = new Memory(0x14);
            var cpu = new CPU(new Registers(), memory);
            TestIncrement8BitRegister(cpu, () => cpu.Registers.D);
        }

        [TestMethod]
        public void Instruction_0x15_Should_Decrement_D()
        {
            var memory = new Memory(0x15);
            var cpu = new CPU(new Registers(), memory);
            TestDecrement8BitRegister(cpu, () => cpu.Registers.D);
        }

        [TestMethod]
        public void Instruction_0x1C_Should_Increment_E()
        {
            var memory = new Memory(0x1C);
            var cpu = new CPU(new Registers(), memory);
            TestIncrement8BitRegister(cpu, () => cpu.Registers.E);
        }

        [TestMethod]
        public void Instruction_0x1D_Should_Decrement_E()
        {
            var memory = new Memory(0x1D);
            var cpu = new CPU(new Registers(), memory);
            TestDecrement8BitRegister(cpu, () => cpu.Registers.E);
        }

        [TestMethod]
        public void Instruction_0x24_Should_Increment_H()
        {
            var memory = new Memory(0x24);
            var cpu = new CPU(new Registers(), memory);
            TestIncrement8BitRegister(cpu, () => cpu.Registers.H);
        }

        [TestMethod]
        public void Instruction_0x25_Should_Decrement_H()
        {
            var memory = new Memory(0x25);
            var cpu = new CPU(new Registers(), memory);
            TestDecrement8BitRegister(cpu, () => cpu.Registers.H);
        }

        [TestMethod]
        public void Instruction_0x2C_Should_Increment_L()
        {
            var memory = new Memory(0x2C);
            var cpu = new CPU(new Registers(), memory);
            TestIncrement8BitRegister(cpu, () => cpu.Registers.L);
        }

        [TestMethod]
        public void Instruction_0x2D_Should_Decrement_L()
        {
            var memory = new Memory(0x2D);
            var cpu = new CPU(new Registers(), memory);
            TestDecrement8BitRegister(cpu, () => cpu.Registers.L);
        }

        [TestMethod]
        public void Instruction_0x3C_Should_Increment_A()
        {
            var memory = new Memory(0x3C);
            var cpu = new CPU(new Registers(), memory);
            TestIncrement8BitRegister(cpu, () => cpu.Registers.A);
        }

        [TestMethod]
        public void Instruction_0x3D_Should_Decrement_A()
        {
            var memory = new Memory(0x3D);
            var cpu = new CPU(new Registers(), memory);
            TestDecrement8BitRegister(cpu, () => cpu.Registers.A);
        }

        /// <summary>
        /// Tests instructions like inc b.
        /// </summary>
        /// <see cref="https://rednex.github.io/rgbds/gbz80.7.html#INC_r8"/>
        private static void TestIncrement8BitRegister(CPU cpu, Func<byte> registerUnderTest)
        {
            //loop up since we're incrementing (making sure to cover wraparound)
            for (int i = 0; i <= byte.MaxValue; i++)
            {
                cpu.Tick();

                var expected = (byte)(i + 1);
                Assert.AreEqual(expected, registerUnderTest(), "Expected 8-bit register to increment after executing inc instruction.");

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
                    //FF -> 0, F -> 10, 1F -> 20, etc should all set the half carry flag
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

        /// <summary>
        /// Tests instructions like dec b.
        /// </summary>
        /// <see cref="https://rednex.github.io/rgbds/gbz80.7.html#DEC_r8"/>
        private static void TestDecrement8BitRegister(CPU cpu, Func<byte> registerUnderTest)
        {
            //loop down since we're decrementing (making sure to cover wraparound)
            for (int i = byte.MaxValue; i >= 0; i--)
            {
                cpu.Tick();

                Assert.AreEqual(i, registerUnderTest(), "Expected 8-bit register to decrement after executing dec instruction.");

                if (registerUnderTest() == 0)
                {
                    Assert.IsTrue(cpu.Registers.HasFlag(Flags.Zero), $"Expected zero flag to be set when 8-bit register is {registerUnderTest()}.");
                }
                else
                {
                    Assert.IsFalse(cpu.Registers.HasFlag(Flags.Zero), $"Expected zero flag to be cleared when 8-bit register is {registerUnderTest()}.");
                }

                if ((registerUnderTest() + 1) % 16 == 0)
                {
                    //0 -> FF, F0 -> EF, E0 -> DF, etc should all set the half carry flag
                    Assert.IsTrue(cpu.Registers.HasFlag(Flags.HalfCarry), $"Expected half carry flag to be set when 8-bit register is decremented to {registerUnderTest()}");
                }
                else
                {
                    Assert.IsFalse(cpu.Registers.HasFlag(Flags.HalfCarry), $"Expected half carry flag to be cleared when 8-bit register is decremented to {registerUnderTest()}");
                }

                Assert.IsTrue(cpu.Registers.HasFlag(Flags.AddSubtract), $"Expected add/subtract flag to be set whenever an 8-bit register is decremented.");

                cpu.Registers.PC--;
            }
        }
    }
}