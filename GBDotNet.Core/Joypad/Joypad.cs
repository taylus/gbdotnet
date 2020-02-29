namespace GBDotNet.Core
{
    /// <summary>
    /// An emulated joypad for pressing and releasing the Game Boy's buttons.
    /// Communicates directly with the <see cref="JoypadRegister"/> located in
    /// memory at $FF00 to set/clear the appropriate bits.
    /// </summary>
    public class Joypad
    {
        private readonly JoypadRegister register;

        public Joypad(JoypadRegister register)
        {
            this.register = register;
        }

        public void Press(Button pressed)
        {
            register.EmulateButtonPress(pressed);
        }

        public void Release(Button released)
        {
            register.EmulateButtonRelease(released);
        }
    }
}
