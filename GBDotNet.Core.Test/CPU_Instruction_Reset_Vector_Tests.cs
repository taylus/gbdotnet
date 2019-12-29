using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GBDotNet.Core.Test
{
    [TestClass]
    public class CPU_Instruction_Reset_Vector_Tests
    {
        [TestMethod]
        public void Instruction_0xC7_Should_Call_Reset_Vector_Zero()
        {
            var memory = new Memory(0xC7);
            var cpu = new CPU(new Registers() { SP = 0xFFFE }, memory);
            var oldProgramCounter = cpu.Registers.PC;

            cpu.Tick();

            Assert.AreEqual(0x0000, cpu.Registers.PC, "Expected rst 00 instruction to set program counter to address 0000.");

            var expectedReturnAddress = oldProgramCounter + 1;  //rst instructions are 1 byte long
            var pushedReturnAddress = Common.FromLittleEndian(memory[cpu.Registers.SP], memory[cpu.Registers.SP + 1]);
            Assert.AreEqual(expectedReturnAddress, pushedReturnAddress, "Expected rst 00 instruction to push return address onto stack.");
        }

        [TestMethod]
        public void Instruction_0xCF_Should_Call_Reset_Vector_Eight()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#RST_vec
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xD7_Should_Call_Reset_Vector_Ten()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#RST_vec
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xDF_Should_Call_Reset_Vector_Eighteen()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#RST_vec
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xE7_Should_Call_Reset_Vector_Twenty()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#RST_vec
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xEF_Should_Call_Reset_Vector_Twenty_Eight()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#RST_vec
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xF7_Should_Call_Reset_Vector_Thirty()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#RST_vec
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Instruction_0xFF_Should_Call_Reset_Vector_Thirty_Eight()
        {
            //https://rednex.github.io/rgbds/gbz80.7.html#RST_vec
            throw new NotImplementedException();
        }
    }
}
