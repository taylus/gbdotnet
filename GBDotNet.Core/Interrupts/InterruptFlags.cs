namespace GBDotNet.Core
{
    /// <summary>
    /// Indicates interrupts which have fired and are waiting to be executed by the CPU.
    /// $FF0F http://bgb.bircd.org/pandocs.htm#interrupts
    /// aka IF or IFLAGS
    /// </summary>
    public class InterruptFlags
    {
        public byte Data { get; set; }

        public bool VBlankInterruptRequested
        {
            get => Data.IsBitSet(0);
            set => Data = Data.SetBitTo(0, value);
        }

        public bool LCDStatInterruptRequested
        {
            get => Data.IsBitSet(1);
            set => Data = Data.SetBitTo(1, value);
        }

        public bool TimerInterruptRequested
        {
            get => Data.IsBitSet(2);
            set => Data = Data.SetBitTo(2, value);
        }

        public bool SerialInterruptRequested
        {
            get => Data.IsBitSet(3);
            set => Data = Data.SetBitTo(3, value);
        }

        public bool JoypadInterruptRequested
        {
            get => Data.IsBitSet(4);
            set => Data = Data.SetBitTo(4, value);
        }
    }
}
