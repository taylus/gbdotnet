using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GBDotNet.Core.Test
{
    /// <summary>
    /// These are adaptations of Blargg's test ROMs found to be failing during development.
    /// I turned them into unit tests to more easily troubleshoot and get them passing.
    /// </summary>
    /// <see cref="https://github.com/retrio/gb-test-roms"/>
    [TestClass]
    public class Blargg_Tests : Program_Tests_Base
    {
        [TestMethod]
        public void Pop_AF_Should_Clear_Lower_Bits_Of_Flags_Register()
        {
            //adapted from test #5 of Blargg's test ROM cpu_instrs\individual\01-special.gb
            var program = new byte[]
            {
                0x01, 0x00, 0x12,   //$0000: ld bc, $1200
                0xC5,               //$0003: push bc  <-------------------+
                0xF1,               //$0004: pop af                       |
                0xF5,               //$0005: push af                      |
                0xD1,               //$0006: pop de                       |
                0x79,               //$0007: ld a, c,                     |
                0xE6, 0xF0,         //$0008: and $f0                      |
                0xBB,               //$000A: cp e                         |
                0xC2, 0xFF, 0xFF,   //$000B: jr nz, test_failed ($FFFF) --|---+
                0x04,               //$000E: inc b                        |   |
                0x0C,               //$000F: inc c                        |   |
                0x20, 0xF1,         //$0010: jr nz, -15 ($0003) ----------+   |
                0x76                //$0012: halt                             |
            };                      //...                                     |
            var memory = new Memory(program); //                              |
            memory[0xFFFF] = 0x76;  //$FFFF: halt  <--------------------------+
            var cpu = new CPU(new Registers() { SP = 0xFFFE }, memory);

            while (!cpu.IsHalted && !ProbablyInInfiniteLoop(cpu))
            {
                Console.WriteLine($"CPU state before executing instruction at address {cpu.Registers.PC:x4}:");
                Console.WriteLine(cpu + Environment.NewLine);
                cpu.Tick();
            }

            Assert.AreNotEqual(0xFFFF, cpu.Registers.LastPC, "CPU halting at address $ffff indicates failure.");
        }
    }
}
