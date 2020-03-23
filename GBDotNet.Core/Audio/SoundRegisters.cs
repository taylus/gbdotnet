using System;

namespace GBDotNet.Core
{
    /// <summary>
    /// TODO: implement audio synthesis, use better names, split these into separate classes, etc.
    /// For now, this class just holds sound register values to flesh out the memory map so ROMs can run.
    /// </summary>
    /// <see cref="http://bgb.bircd.org/pandocs.htm#soundcontroller"/>
    public class SoundRegisters
    {
        //sound channel #1: tone & sweep
        public byte NR10 { get; set; }  //$FF10
        public byte NR11 { get; set; }  //$FF11
        public byte NR12 { get; set; }  //$FF12
        public byte NR13 { get; set; }  //$FF13
        public byte NR14 { get; set; }  //$FF14

        //sound channel #2: tone
        public byte NR21 { get; set; }  //$FF16
        public byte NR22 { get; set; }  //$FF17
        public byte NR23 { get; set; }  //$FF18
        public byte NR24 { get; set; }  //$FF19

        //sound channel #3: wave output
        public byte NR30 { get; set; }  //$FF1A
        public byte NR31 { get; set; }  //$FF1B
        public byte NR32 { get; set; }  //$FF1C
        public byte NR33 { get; set; }  //$FF1D
        public byte NR34 { get; set; }  //$FF1E
        public byte[] WavePatternRAM { get; set; } = new byte[16];  //$FF30-$FF3F (32 x 4-bit samples)

        //sound channel #4: noise
        public byte NR41 { get; set; }  //$FF20
        public byte NR42 { get; set; }  //$FF21
        public byte NR43 { get; set; }  //$FF22
        public byte NR44 { get; set; }  //$FF23

        //sound control registers
        public byte NR50 { get; set; }  //$FF24
        public byte NR51 { get; set; }  //$FF25
        public byte NR52 { get; set; }  //$FF26

        public bool MappedToAddress(int address) => address >= 0xFF10 && address <= 0xFF3F;

        public byte this[int address]
        {
            get
            {
                if (address == 0xFF10) return NR10;
                else if (address == 0xFF11) return NR11;
                else if (address == 0xFF12) return NR12;
                else if (address == 0xFF13) return NR13;
                else if (address == 0xFF14) return NR14;
                else if (address == 0xFF16) return NR21;
                else if (address == 0xFF17) return NR22;
                else if (address == 0xFF18) return NR23;
                else if (address == 0xFF19) return NR24;
                else if (address == 0xFF1A) return NR30;
                else if (address == 0xFF1B) return NR31;
                else if (address == 0xFF1C) return NR32;
                else if (address == 0xFF1D) return NR33;
                else if (address == 0xFF1E) return NR34;
                else if (address == 0xFF20) return NR41;
                else if (address == 0xFF21) return NR42;
                else if (address == 0xFF22) return NR43;
                else if (address == 0xFF23) return NR44;
                else if (address == 0xFF24) return NR50;
                else if (address == 0xFF25) return NR51;
                else if (address == 0xFF26) return NR52;
                else if (address >= 0xFF30 && address <= 0xFF3F) return WavePatternRAM[address - 0xFF30];
                else throw new ArgumentOutOfRangeException(nameof(address), $"Unable to read: address ${address:X4} is not supported by the sound controller.");
            }
            set
            {
                //TODO: https://gbdev.gg8.se/wiki/articles/Gameboy_sound_hardware#Trigger_Event
                if (address == 0xFF10) NR10 = value;
                else if (address == 0xFF11) NR11 = value;
                else if (address == 0xFF12) NR12 = value;
                else if (address == 0xFF13) NR13 = value;
                else if (address == 0xFF14) NR14 = value;
                else if (address == 0xFF16) NR21 = value;
                else if (address == 0xFF17) NR22 = value;
                else if (address == 0xFF18) NR23 = value;
                else if (address == 0xFF19) NR24 = value;
                else if (address == 0xFF1A) NR30 = value;
                else if (address == 0xFF1B) NR31 = value;
                else if (address == 0xFF1C) NR32 = value;
                else if (address == 0xFF1D) NR33 = value;
                else if (address == 0xFF1E) NR34 = value;
                else if (address == 0xFF20) NR41 = value;
                else if (address == 0xFF21) NR42 = value;
                else if (address == 0xFF22) NR43 = value;
                else if (address == 0xFF23) NR44 = value;
                else if (address == 0xFF24) NR50 = value;
                else if (address == 0xFF25) NR51 = value;
                else if (address == 0xFF26) NR52 = value;
                else if (address >= 0xFF30 && address <= 0xFF3F) WavePatternRAM[address - 0xFF30] = value;
                else throw new ArgumentOutOfRangeException(nameof(address), $"Unable to write: address ${address:X4} is not supported by the sound controller.");
            }
        }
    }
}
