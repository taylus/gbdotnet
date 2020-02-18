namespace GBDotNet.Core
{
    /// <summary>
    /// Registers which manage and request timer interrupts at fixed frequencies
    /// (measured in Hz in real life, but cycle counts in this emulator).
    /// http://bgb.bircd.org/pandocs.htm#timeranddividerregisters
    /// </summary>
    public class Timer
    {
        private const int cyclesPerDividerRegisterIncrement = 256;  //4194304 Hz /  16384 Hz
        private int cyclesSinceLastDivIncrement;
        private int cyclesSinceLastTimerIncrement;
        private readonly IMemory memory;

        public byte DividerRegister { get; set; }   //$FF04
        public byte TimerCounter { get; set; }      //$FF05
        public byte TimerModulo { get; set; }       //$FF06
        public byte TimerControl { get; set; }      //$FF07

        public bool IsTimerEnabled => TimerControl.IsBitSet(2);

        public int CyclesPerTimerIncrement
        {
            get
            {
                int speedSelect = 2 * (TimerControl.IsBitSet(1) ? 1 : 0) + (TimerControl.IsBitSet(0) ? 1 : 0);
                if (speedSelect == 0b00) return 1024;   //4194304 Hz /   4096 Hz
                if (speedSelect == 0b01) return 16;     //4194304 Hz / 262144 Hz
                if (speedSelect == 0b10) return 64;     //4194304 Hz /  65536 Hz
                return 256;                             //4194304 Hz /  16384 Hz
            }
        }

        public Timer(IMemory memory)
        {
            this.memory = memory;
        }

        public void Tick(int elapsedCycles)
        {
            if (!IsTimerEnabled) return;
            IncrementDividerRegisterAtFixedSpeed(elapsedCycles);
            IncrementTimerCounterAtVariableSpeedAndFireInterruptAsAppropriate(elapsedCycles);
        }

        private void IncrementDividerRegisterAtFixedSpeed(int elapsedCycles)
        {
            cyclesSinceLastDivIncrement += elapsedCycles;
            while (cyclesSinceLastDivIncrement >= cyclesPerDividerRegisterIncrement)
            {
                cyclesSinceLastDivIncrement -= cyclesPerDividerRegisterIncrement;
                DividerRegister++;
            }
        }

        private void IncrementTimerCounterAtVariableSpeedAndFireInterruptAsAppropriate(int elapsedCycles)
        {
            cyclesSinceLastTimerIncrement += elapsedCycles;
            while (cyclesSinceLastTimerIncrement >= CyclesPerTimerIncrement)
            {
                cyclesSinceLastTimerIncrement -= CyclesPerTimerIncrement;
                TimerCounter++;
                if (TimerCounter == 0)
                {
                    RequestTimerInterrupt();
                    TimerCounter = TimerModulo;
                }
            }
        }

        private void RequestTimerInterrupt() => memory[0xFF0F] |= 0b0000_0100;
    }
}
