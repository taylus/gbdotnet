using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GBDotNet.Core.Test
{
    /// <summary>
    /// Tests of series of instructions (programs) as opposed to individual instructions.
    /// </summary>
    [TestClass]
    public class Program_Tests
    {
        [TestMethod]
        public void Branching_Program_Should_Halt_At_Expected_Address()
        {
            var program = new byte[]
            {
                0xCA, 0x06, 0x00,   //0000: jp z,  $0006 (shouldn't jump since Z flag starts out not set)
                0xC2, 0x07, 0x00,   //0003: jp nz, $0007
                0x76,               //0006: halt
                0x76                //0007: halt
            };
            var memory = new Memory(program);
            var cpu = new CPU(new Registers(), memory);

            while (!cpu.IsHalted)
            {
                Console.WriteLine($"CPU state before executing instruction at address {cpu.Registers.PC:x4}:");
                Console.WriteLine(cpu + Environment.NewLine);
                cpu.Tick();
            }

            Assert.AreEqual(program.Length, cpu.Registers.PC);
        }
    }
}
