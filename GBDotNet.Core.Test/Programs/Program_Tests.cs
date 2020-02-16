using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GBDotNet.Core.Test
{
    /// <summary>
    /// Tests of series of instructions (programs) as opposed to individual instructions.
    /// </summary>
    [TestClass]
    public class Program_Tests : Program_Tests_Base
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

            while (!cpu.IsHalted && !ProbablyInInfiniteLoop(cpu))
            {
                Console.WriteLine($"CPU state before executing instruction at address {cpu.Registers.PC:x4}:");
                Console.WriteLine(cpu + Environment.NewLine);
                cpu.Tick();
            }

            Assert.AreEqual(program.Length, cpu.Registers.PC);
        }

        [TestMethod]
        public void Hello_World_Program_Should_Write_Hello_World_To_Memory()
        {
            var program = new List<byte>() { 0x21, 0x00, 0x40 };    //ld hl, $4000
            program.AddRange(BuildProgramToLoadStringIntoMemoryAddressPointedToByHL("Hello, world!"));
            program.AddRange(new byte[] { 0x76 });  //add halt instruction to end
            var memory = new Memory(program.ToArray());
            var cpu = new CPU(new Registers(), memory);

            while (!cpu.IsHalted && !ProbablyInInfiniteLoop(cpu))
            {
                Console.WriteLine($"CPU state before executing instruction at address {cpu.Registers.PC:x4}:");
                Console.WriteLine(cpu + Environment.NewLine);
                cpu.Tick();
            }

            CollectionAssert.AreEqual("Hello, world!".ToArray(), memory.Skip(0x4000).Take(13).Select(b => (char)b).ToArray());
        }

        private static IEnumerable<byte> BuildProgramToLoadStringIntoMemoryAddressPointedToByHL(string str)
        {
            //surround each character w/ instructions to store them in memory:
            //0x3E, nn  =>  ld a, nn
            //0x22      =>  ld [hl+], a 
            return str.SelectMany(c => new byte[] { 0x3E, (byte)c, 0x22 });
        }
    }
}
