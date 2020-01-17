namespace GBDotNet.Core
{
    public partial class PPU
    {
        private enum Mode
        {
            HBlank = 0,
            VBlank = 1,
            OamScan = 2,
            HDraw = 3
        }
    }
}
