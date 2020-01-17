namespace GBDotNet.Core
{
    /// <summary>
    /// Future design notes: <see cref="MemoryBus"/> should direct reads/writes to the appropriate addresses here.
    /// </summary>
    public class PPURegisters
    {
        public byte Control { get; set; }           //$ff40, http://bgb.bircd.org/pandocs.htm#lcdcontrolregister
        public byte Status { get; set; }            //$ff41, http://bgb.bircd.org/pandocs.htm#lcdstatusregister
        public byte ScrollY { get; set; }           //$ff42, http://bgb.bircd.org/pandocs.htm#lcdpositionandscrolling
        public byte ScrollX { get; set; }           //$ff43, http://bgb.bircd.org/pandocs.htm#lcdpositionandscrolling
        public byte CurrentScanline { get; set; }   //$ff44, http://bgb.bircd.org/pandocs.htm#lcdpositionandscrolling
        public byte CompareScanline { get; set; }   //$ff45, http://bgb.bircd.org/pandocs.htm#lcdpositionandscrolling
        public byte BackgroundPalette { get; set; } //$ff47, http://bgb.bircd.org/pandocs.htm#lcdmonochromepalettes
        public byte SpritePalette0 { get; set; }    //$ff48, http://bgb.bircd.org/pandocs.htm#lcdmonochromepalettes
        public byte SpritePalette1 { get; set; }    //$ff49, http://bgb.bircd.org/pandocs.htm#lcdmonochromepalettes
        public byte WindowY { get; set; }           //$ff4a, http://bgb.bircd.org/pandocs.htm#lcdpositionandscrolling
        public byte WindowX { get; set; }           //$ff4b, http://bgb.bircd.org/pandocs.htm#lcdpositionandscrolling
    }
}
