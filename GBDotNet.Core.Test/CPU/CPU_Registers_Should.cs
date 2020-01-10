using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GBDotNet.Core.Test
{
    [TestClass]
    public class CPU_Registers_Should
    {
        private CPU cpu;

        [TestInitialize]
        public void BeforeEveryTest()
        {
            cpu = new CPU(new Registers(), new Memory());
        }

        [TestMethod]
        public void Update_A_And_F_When_Setting_AF()
        {
            for (int i = 0; i <= ushort.MaxValue; i++)
            {
                cpu.Registers.AF = (ushort)i;
                Assert.AreEqual(i >> 8, cpu.Registers.A);
                Assert.AreEqual(i & 0xFF, cpu.Registers.F);
                Assert.AreEqual((ushort)i, cpu.Registers.AF);
            }
        }

        [TestMethod]
        public void Update_AF_When_Setting_A_And_F()
        {
            for (int a = 0; a <= byte.MaxValue; a++)
            {
                cpu.Registers.A = (byte)a;
                for (int f = 0; f < byte.MaxValue; f++)
                {
                    cpu.Registers.F = (byte)f;
                    Assert.AreEqual((a << 8) | f, cpu.Registers.AF);
                }
            }
        }

        [TestMethod]
        public void Update_B_And_C_When_Setting_BC()
        {
            for (int i = 0; i <= ushort.MaxValue; i++)
            {
                cpu.Registers.BC = (ushort)i;
                Assert.AreEqual(i >> 8, cpu.Registers.B);
                Assert.AreEqual(i & 0xFF, cpu.Registers.C);
                Assert.AreEqual((ushort)i, cpu.Registers.BC);
            }
        }

        [TestMethod]
        public void Update_BC_When_Setting_B_And_C()
        {
            for (int b = 0; b <= byte.MaxValue; b++)
            {
                cpu.Registers.B = (byte)b;
                for (int c = 0; c < byte.MaxValue; c++)
                {
                    cpu.Registers.C = (byte)c;
                    Assert.AreEqual((b << 8) | c, cpu.Registers.BC);
                }
            }
        }

        [TestMethod]
        public void Update_D_And_E_When_Setting_DE()
        {
            for (int i = 0; i <= ushort.MaxValue; i++)
            {
                cpu.Registers.DE = (ushort)i;
                Assert.AreEqual(i >> 8, cpu.Registers.D);
                Assert.AreEqual(i & 0xFF, cpu.Registers.E);
                Assert.AreEqual((ushort)i, cpu.Registers.DE);
            }
        }

        [TestMethod]
        public void Update_DE_When_Setting_D_And_E()
        {
            for (int d = 0; d <= byte.MaxValue; d++)
            {
                cpu.Registers.D = (byte)d;
                for (int e = 0; e < byte.MaxValue; e++)
                {
                    cpu.Registers.E = (byte)e;
                    Assert.AreEqual((d << 8) | e, cpu.Registers.DE);
                }
            }
        }

        [TestMethod]
        public void Update_H_And_L_When_Setting_HL()
        {
            for (int i = 0; i <= ushort.MaxValue; i++)
            {
                cpu.Registers.HL = (ushort)i;
                Assert.AreEqual(i >> 8, cpu.Registers.H);
                Assert.AreEqual(i & 0xFF, cpu.Registers.L);
                Assert.AreEqual((ushort)i, cpu.Registers.HL);
            }
        }

        [TestMethod]
        public void Update_HL_When_Setting_H_And_L()
        {
            for (int h = 0; h <= byte.MaxValue; h++)
            {
                cpu.Registers.H = (byte)h;
                for (int l = 0; l < byte.MaxValue; l++)
                {
                    cpu.Registers.L = (byte)l;
                    Assert.AreEqual((h << 8) | l, cpu.Registers.HL);
                }
            }
        }
    }
}
