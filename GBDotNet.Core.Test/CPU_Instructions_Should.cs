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
                    cpu.Registers.PC -= 3;  //rewind
                }
            }
        }
    }
}
