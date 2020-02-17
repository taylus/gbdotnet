using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GBDotNet.Core.Test
{
    public abstract class Program_Tests_Base
    {
        /// <summary>
        /// If a test makes the CPU execute more than this many cycles, fail it,
        /// assuming an infinite loop, so as not to lock up the test runner.
        /// </summary>
        protected const int maxCycles = 10_000_000;

        protected static bool ProbablyInInfiniteLoop(CPU cpu, int maxCycles = maxCycles)
        {
            if (cpu.TotalElapsedCycles <= maxCycles) return false;
            Assert.Fail($"Possible infinite loop detected, check program for errors or increase max cycle limit ({maxCycles}).");
            return true;
        }
    }
}
