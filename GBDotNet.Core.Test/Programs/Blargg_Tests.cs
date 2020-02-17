using System;
using System.Linq;
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
                0xC2, 0x88, 0x88,   //$000B: jp nz, test_failed ($8888) --|---+
                0x04,               //$000E: inc b                        |   |
                0x0C,               //$000F: inc c                        |   |
                0x20, 0xF1,         //$0010: jr nz, -15 ($0003) ----------+   |
                0x76                //$0012: success: halt                    |
            };                      //...                                     |
            var memory = new Memory(program); //                              |
            memory[0x8888] = 0x76;  //$8888: test_failed: halt  <-------------+
            var cpu = new CPU(new Registers() { SP = 0xFFFE }, memory);

            while (!cpu.IsHalted && !ProbablyInInfiniteLoop(cpu))
            {
                Console.WriteLine($"CPU state before executing instruction at address {cpu.Registers.PC:x4}:");
                Console.WriteLine(cpu + Environment.NewLine);
                cpu.Tick();
            }

            Assert.AreNotEqual(0x8888, cpu.Registers.LastPC, "CPU halting at address $8888 indicates failure.");
        }

        [TestMethod]
        public void Timer_Interrupt_Should_Fire_When_Enabled_And_Requested()
        {
            //adapted from test #4 of Blargg's test ROM cpu_instrs\individual\02-interrupts.gb
            var padding = Enumerable.Repeat<byte>(0xFF, 0x2000);
            var program = new byte[]
            {
                0x3E, 0x04,         //$2000: ld a, $04
                0xE0, 0xFF,         //$2002: ldh [$ffff], a     ;enable timer interrupt
                0xFB,               //$2004: ei
                0x01, 0x00, 0x00,   //$2005: ld bc, $0000
                0xC5,               //$2008: push bc
                0xC1,               //$2009: pop bc
                0x04,               //$200A: inc b
                0x3E, 0x04,         //$200B: ld a, $04
                0xE0, 0x0F,         //$200D: ldh [$ff0f], a     ;request timer interrupt
                0x05,               //$200F: interrupt_addr: dec b
                0xC2, 0x88, 0x88,   //$2010: jp nz, test_failed --------------------+
                0xF8, 0xFE,         //$2013: ld hl, sp-2                            |
                0x2A,               //$2015: ldi a, [hl]                            |
                0xFE, 0x0F,         //$2016: cp a, <interrupt_addr (lower byte)     |
                0xC2, 0x88, 0x88,   //$2018: jp nz, test_failed --------------------+
                0x7E,               //$201B: ld a, [hl]                             |
                0xFE, 0x20,         //$201C: cp a, >interrupt_addr (upper byte)     |
                0xC2, 0x88, 0x88,   //$201E: jp nz, test_failed --------------------+
                0xF0, 0x0F,         //$2021: ldh a, [$ff0f]                         |
                0xE6, 0x04,         //$2023: and a, $04                             |
                0xC2, 0x88, 0x88,   //$2025: jp nz, test_failed --------------------|
                0x76                //$2028: halt (success)                         |
            };                      //                                              |
            //offset the program so its upper byte isn't $00                        |
            var memory = new Memory(padding.Concat(program).ToArray()); //          |
            memory[0x8888] = 0x76;  //$8888: test_failed: halt  <-------------------+
            memory[0x0050] = 0x3C;  //$0050: inc a              ;timer interrupt handler
            memory[0x0051] = 0xC9;  //$0051: ret
            var cpu = new CPU(new Registers() { PC = 0x2000, SP = 0xFFFE }, memory);

            while (!cpu.IsHalted && !ProbablyInInfiniteLoop(cpu))
            {
                Console.WriteLine($"CPU state before executing instruction at address {cpu.Registers.PC:x4}:");
                Console.WriteLine(cpu + Environment.NewLine);
                cpu.Tick();
            }

            Assert.AreNotEqual(0x8888, cpu.Registers.LastPC, "CPU halting at address $8888 indicates failure.");
        }
    }
}
