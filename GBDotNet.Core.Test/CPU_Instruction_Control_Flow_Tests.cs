using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GBDotNet.Core.Test
{
    [TestClass]
    public class CPU_Instruction_Control_Flow_Tests
    {
        [TestMethod]
        public void Instruction_0x18_Should_Relative_Jump_By_Signed_Immediate_Negative_Offset()
        {
            var memory = new Memory(0x18, 0xFE);
            var cpu = new CPU(new Registers(), memory);
            var addressOfJump = cpu.Registers.PC;

            cpu.Tick();

            Assert.AreEqual(addressOfJump, cpu.Registers.PC);
        }

        [TestMethod]
        public void Instruction_0x18_Should_Relative_Jump_By_Signed_Immediate_Positive_Offset()
        {
            var memory = new Memory(0x18, 0x01);
            var cpu = new CPU(new Registers(), memory);
            var addressOfJump = cpu.Registers.PC;

            cpu.Tick();

            Assert.AreEqual(addressOfJump + 3, cpu.Registers.PC);
        }
    }
}
