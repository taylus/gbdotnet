namespace GBDotNet.Core
{
    /// <summary>
    /// Implements the single 8-bit register that conveys joypad input in its lower 4 bits.
    /// When buttons are pressed, their corresponding values are 0, and which button goes
    /// with which bit depends on which "output line" is selected by writing a 0 to either bit 4
    /// ("I want Start/Select/B/A in bits 3-0") or bit 5 ("I want Down/Up/Left/Right in bits 3-0")
    /// </summary>
    /// <remarks>
    /// Wiring diagram from: http://imrannazar.com/GameBoy-Emulation-in-JavaScript:-Input
    ///                             ┌──────────────┐
    ///                             │ 7            │
    ///                             │ 6            │
    ///          ┌──────────────────┤ 5   P15      │
    ///          │   ┌──────────────┤ 4   P14      │
    ///  ───Down─┼───┼─Start────────┤ 3   P13      │
    ///  ───Up───┼───┼─Select───────┤ 2   P12      │
    ///  ───Left─┼───┼─B────────────┤ 1   P11      │
    ///  ───Right┼───┼─A────────────| 0   P10      │
    ///                             └──────────────┘ 	 
    /// </remarks>
    /// <seealso cref="https://github.com/AntonioND/giibiiadvance/blob/master/docs/TCAGBD.pdf"/>
    /// <seealso cref="http://bgb.bircd.org/pandocs.htm#joypadinput"/>
    public class JoypadRegister
    {
        private OutputLine? selectedButtons;
        private readonly byte[] buttons = { 0x0F, 0x0F };    //two copies of the 4 lower bits of the register

        public byte Read()
        {
            //bits 7-6 are unused and always read as 1
            if (selectedButtons == OutputLine.P14) return (byte)(0xD0 | buttons[0]);
            else if (selectedButtons == OutputLine.P15) return (byte)(0xE0 | buttons[1]);
            else return 0xFF;
        }

        public void Write(byte value)
        {
            bool bit5 = value.IsBitSet(5);
            bool bit4 = value.IsBitSet(4);
            if (bit5 && !bit4) selectedButtons = OutputLine.P14;
            else if (!bit5 && bit4) selectedButtons = OutputLine.P15;
            else selectedButtons = null;    //both, neither => invalid
        }

        public void EmulateButtonPress(Button pressed)
        {
            var P14 = (int)OutputLine.P14;
            var P15 = (int)OutputLine.P15;

            if (pressed.HasFlag(Button.Start)) buttons[P14] = buttons[P14].ClearBit(3);
            else if (pressed.HasFlag(Button.Select)) buttons[P14] = buttons[P14].ClearBit(2);
            else if (pressed.HasFlag(Button.B)) buttons[P14] = buttons[P14].ClearBit(1);
            else if (pressed.HasFlag(Button.A)) buttons[P14] = buttons[P14].ClearBit(0);
            else if (pressed.HasFlag(Button.Down)) buttons[P15] = buttons[P15].ClearBit(3);
            else if (pressed.HasFlag(Button.Up)) buttons[P15] = buttons[P15].ClearBit(2);
            else if (pressed.HasFlag(Button.Left)) buttons[P15] = buttons[P15].ClearBit(1);
            else if (pressed.HasFlag(Button.Right)) buttons[P15] = buttons[P15].ClearBit(0);
        }

        public void EmulateButtonRelease(Button released)
        {
            var P14 = (int)OutputLine.P14;
            var P15 = (int)OutputLine.P15;

            if (released.HasFlag(Button.Start)) buttons[P14] = buttons[P14].SetBit(3);
            else if (released.HasFlag(Button.Select)) buttons[P14] = buttons[P14].SetBit(2);
            else if (released.HasFlag(Button.B)) buttons[P14] = buttons[P14].SetBit(1);
            else if (released.HasFlag(Button.A)) buttons[P14] = buttons[P14].SetBit(0);
            else if (released.HasFlag(Button.Down)) buttons[P15] = buttons[P15].SetBit(3);
            else if (released.HasFlag(Button.Up)) buttons[P15] = buttons[P15].SetBit(2);
            else if (released.HasFlag(Button.Left)) buttons[P15] = buttons[P15].SetBit(1);
            else if (released.HasFlag(Button.Right)) buttons[P15] = buttons[P15].SetBit(0);
        }

        /// <summary>
        /// Represents a group of buttons on the same input line.
        /// </summary>
        private enum OutputLine
        {
            P14 = 1,    //Start/Select/B/A
            P15 = 0     //Down/Up/Left/Right
        }
    }
}
