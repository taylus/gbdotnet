using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GBDotNet.Core.Test
{
    [TestClass]
    public class CPU_Instructions_Should
    {
        [TestMethod]
        public void Instruction_0x01_Load_BC_With_16_Bit_Immediate()
        {
            var memory = new Memory(0x01);
            var cpu = new CPU(new Registers(), memory);

            for (int i = 0; i <= byte.MaxValue; i++)
            {
                memory[1] = (byte)i;
                for(int j = 0; j <= byte.MaxValue; j++)
                {
                    memory[2] = (byte)j;
                    cpu.Tick();
                    Assert.AreEqual((j << 8) | i, cpu.Registers.BC);
                    cpu.Registers.PC -= 3;  //rewind to run again w/ new value
                }
            }
        }

        [TestMethod]
        public void Instruction_0x02_Load_Address_Pointed_To_By_BC_With_A()
        {
            var memory = new Memory(0x02);
            var cpu = new CPU(new Registers(), memory);

            cpu.Registers.BC = 5000;
            cpu.Registers.A = 100;
            cpu.Tick();

            Assert.AreEqual(cpu.Registers.A, memory[cpu.Registers.BC]);
        }

        [TestMethod]
        public void Instruction_0x03_Increment_BC()
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
        public void Instruction_0x04_Increment_B()
        {
            var memory = new Memory(0x04);
            var cpu = new CPU(new Registers(), memory);

            //loop up since we're incrementing (making sure to cover wraparound)
            for (int i = 0; i <= byte.MaxValue; i++)
            {
                cpu.Tick();

                var expected = (i + 1) & byte.MaxValue;
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
                    Assert.IsTrue(cpu.Registers.HasFlag(Flags.HalfCarry), $"Expected half carry flag to be set when B is incremented to {cpu.Registers.B}");
                }
                else
                {
                    Assert.IsFalse(cpu.Registers.HasFlag(Flags.HalfCarry), $"Expected half carry flag to be cleared when B is incremented to {cpu.Registers.B}");
                }

                Assert.IsFalse(cpu.Registers.HasFlag(Flags.AddSubtract), $"Expected add/subtract flag to be cleared whenever an 8-bit register is incremented.");

                cpu.Registers.PC--;
            }
        }

        [TestMethod]
        public void Instruction_0x05_Decrement_B()
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
    }
}
