using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GBDotNet.Core.Test
{
    [TestClass]
    public class CPU_Instruction_Reset_Vector_Tests
    {
        [TestMethod]
        public void Instruction_0xC7_Should_Call_Reset_Vector_Zero()
        {
            TestResetVector(opcode: 0xC7, expectedAddress: 0x0000);
        }

        [TestMethod]
        public void Instruction_0xCF_Should_Call_Reset_Vector_Eight()
        {
            TestResetVector(opcode: 0xCF, expectedAddress: 0x0008);
        }

        [TestMethod]
        public void Instruction_0xD7_Should_Call_Reset_Vector_Ten()
        {
            TestResetVector(opcode: 0xD7, expectedAddress: 0x0010);
        }

        [TestMethod]
        public void Instruction_0xDF_Should_Call_Reset_Vector_Eighteen()
        {
            TestResetVector(opcode: 0xDF, expectedAddress: 0x0018);
        }

        [TestMethod]
        public void Instruction_0xE7_Should_Call_Reset_Vector_Twenty()
        {
            TestResetVector(opcode: 0xE7, expectedAddress: 0x0020);
        }

        [TestMethod]
        public void Instruction_0xEF_Should_Call_Reset_Vector_Twenty_Eight()
        {
            TestResetVector(opcode: 0xEF, expectedAddress: 0x0028);
        }

        [TestMethod]
        public void Instruction_0xF7_Should_Call_Reset_Vector_Thirty()
        {
            TestResetVector(opcode: 0xF7, expectedAddress: 0x0030);
        }

        [TestMethod]
        public void Instruction_0xFF_Should_Call_Reset_Vector_Thirty_Eight()
        {
            TestResetVector(opcode: 0xFF, expectedAddress: 0x0038);
        }

        private static void TestResetVector(byte opcode, ushort expectedAddress, ushort initialStackPointer = 0xFFFE)
        {
            var memory = new Memory(opcode);
            var cpu = new CPU(new Registers() { SP = initialStackPointer }, memory);
            var initialProgramCounter = cpu.Registers.PC;

            cpu.Tick();

            Assert.AreEqual(expectedAddress, cpu.Registers.PC, $"Expected rst instruction to set program counter to address {expectedAddress:x4}.");
            Assert.AreEqual(16, cpu.CyclesLastTick);

            var expectedReturnAddress = initialProgramCounter + 1;  //rst instructions are 1 byte long
            var pushedReturnAddress = Common.FromLittleEndian(memory[cpu.Registers.SP], memory[cpu.Registers.SP + 1]);
            Assert.AreEqual(expectedReturnAddress, pushedReturnAddress, "Expected rst instruction to push correct return address onto stack.");
        }
    }
}
