namespace GBDotNet.Core
{
    /// <summary>
    /// Indicates which interrupts are enabled (can be triggered).
    /// $FFFF http://bgb.bircd.org/pandocs.htm#interrupts
    /// aka IE or ISWITCH
    /// </summary>
    public class InterruptEnable
    {
        public byte Data { get; set; }

        public bool VBlankInterruptEnabled
        {
            get => Data.IsBitSet(0);
            set => Data = Data.SetBitTo(0, value);
        }

        public bool LCDStatInterruptEnabled
        {
            get => Data.IsBitSet(1);
            set => Data = Data.SetBitTo(1, value);
        }

        public bool TimerInterruptEnabled
        {
            get => Data.IsBitSet(2);
            set => Data = Data.SetBitTo(2, value);
        }

        public bool SerialInterruptEnabled
        {
            get => Data.IsBitSet(3);
            set => Data = Data.SetBitTo(3, value);
        }

        public bool JoypadInterruptEnabled
        {
            get => Data.IsBitSet(4);
            set => Data = Data.SetBitTo(4, value);
        }
    }
}
