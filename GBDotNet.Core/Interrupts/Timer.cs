using System;

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

        public byte DividerRegister { get; set; }   //$FF04, aka DIV
        public byte TimerCounter { get; set; }      //$FF05, aka TIMA
        public byte TimerModulo { get; set; }       //$FF06, aka TMA
        public byte TimerControl { get; set; }      //$FF07, aka TAC

        public bool IsTimerEnabled => TimerControl.IsBitSet(2);

        public int CyclesPerTimerIncrement
        {
            get
            {
                int speedSelect = (TimerControl.IsBitSet(1) ? 2 : 0) + (TimerControl.IsBitSet(0) ? 1 : 0);
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

        public bool MappedToAddress(int address) => address >= 0xFF04 && address <= 0xFF07;

        public byte this[int address]
        {
            get
            {
                if (address == 0xFF04) return DividerRegister;
                else if (address == 0xFF05) return TimerCounter;
                else if (address == 0xFF06) return TimerModulo;
                else if (address == 0xFF07) return TimerControl;
                else throw new ArgumentOutOfRangeException(nameof(address), $"Unable to read: address ${address:X4} is not supported by the timer module.");
            }
            set
            {
                if (address == 0xFF04) DividerRegister = 0; //writing any value to DIV resets it to 0
                else if (address == 0xFF05) TimerCounter = value;
                else if (address == 0xFF06) TimerModulo = value;
                else if (address == 0xFF07) TimerControl = (byte)(value & 0b0000_00111);
                else throw new ArgumentOutOfRangeException(nameof(address), $"Unable to write: address ${address:X4} is not supported by the timer module.");
            }
        }

        public void Tick(int elapsedCycles)
        {
            IncrementDividerRegisterAtFixedSpeed(elapsedCycles);
            IncrementTimerCounterAtVariableSpeedAndFireInterruptOnOverflow(elapsedCycles);
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

        private void IncrementTimerCounterAtVariableSpeedAndFireInterruptOnOverflow(int elapsedCycles)
        {
            if (!IsTimerEnabled) return;
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
