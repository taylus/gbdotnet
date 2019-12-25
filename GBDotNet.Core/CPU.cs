using System;

namespace GBDotNet.Core
{
    /// <summary>
    /// Implements the Game Boy's processor, the 8-bit Sharp LR35902.
    /// </summary>
    /// <remarks>
    /// Like most computer processors, it:
    /// * maintains state in a number of registers,
    /// * implements an instruction set which updates that state, performs calculations, and interacts w/ memory, and
    /// * performs a fetch/decode/execute cycle in order to run programs.
    /// </remarks>
    /// <see cref="http://www.pastraiser.com/cpu/gameboy/gameboy_opcodes.html"/>
    /// <see cref="https://rednex.github.io/rgbds/gbz80.7.html"/>
    public partial class CPU
    {
        public Registers Registers { get; private set; }
        public Memory Memory { get; private set; }
        public bool IsHalted { get; private set; }

        private readonly Action[] instructionSet;

        public CPU(Registers registers, Memory memory)
        {
            Registers = registers;
            Memory = memory;

            //index instructions by opcode
            instructionSet = new Action[]
            {
                //0x00
                () => { }, //nop
                () => Instruction_0x01_Load_BC_With_16_Bit_Immediate(),
                () => Instruction_0x02_Load_Address_Pointed_To_By_BC_With_A(),
                () => Instruction_0x03_Increment_BC(),
                () => Instruction_0x04_Increment_B(),
                () => Instruction_0x05_Decrement_B(),
                () => Instruction_0x06_Load_B_With_8_Bit_Immediate(),
                () => Instruction_0x07_Rotate_A_Left_Circular(),
                () => Instruction_0x08_Load_Address_With_Stack_Pointer(),
                () => Instruction_0x09_Add_BC_To_HL(),
                () => Instruction_0x0A_Load_A_From_Address_Pointed_To_By_BC(),
                () => Instruction_0x0B_Decrement_BC(),
                () => Instruction_0x0C_Increment_C(),
                () => Instruction_0x0D_Decrement_C(),
                () => Instruction_0x0E_Load_C_With_8_Bit_Immediate(),
                () => Instruction_0x0F_Rotate_A_Right_Circular(),
                //0x10
                () => Instruction_0x10_Stop(),
                () => Instruction_0x11_Load_DE_With_16_Bit_Immediate(),
                () => Instruction_0x12_Load_Address_Pointed_To_By_DE_With_A(),
                () => Instruction_0x13_Increment_DE(),
                () => Instruction_0x14_Increment_D(),
                () => Instruction_0x15_Decrement_D(),
                () => Instruction_0x16_Load_D_With_8_Bit_Immediate(),
                () => Instruction_0x17_Rotate_A_Left(),
                () => Instruction_0x18_Relative_Jump_By_8_Bit_Signed_Immediate(),
                () => Instruction_0x19_Add_DE_To_HL(),
                () => Instruction_0x1A_Load_A_From_Address_Pointed_To_By_DE(),
                () => Instruction_0x1B_Decrement_DE(),
                () => Instruction_0x1C_Increment_E(),
                () => Instruction_0x1D_Decrement_E(),
                () => Instruction_0x1E_Load_E_With_8_Bit_Immediate(),
                () => Instruction_0x1F_Rotate_A_Right(),
                //0x20
                () => { throw new NotImplementedException(); },
                () => Instruction_0x21_Load_HL_With_16_Bit_Immediate(),
                () => Instruction_0x22_Load_Address_Pointed_To_By_HL_With_A_Then_Increment_HL(),
                () => Instruction_0x23_Increment_HL(),
                () => Instruction_0x24_Increment_H(),
                () => Instruction_0x25_Decrement_H(),
                () => Instruction_0x26_Load_H_With_8_Bit_Immediate(),
                () => { throw new NotImplementedException(); },
                () => { throw new NotImplementedException(); },
                () => Instruction_0x29_Add_HL_To_HL(),
                () => Instruction_0x2A_Load_A_With_Address_Pointed_To_By_HL_Then_Increment_HL(),
                () => Instruction_0x2B_Decrement_HL(),
                () => Instruction_0x2C_Increment_L(),
                () => Instruction_0x2D_Decrement_L(),
                () => Instruction_0x2E_Load_L_With_8_Bit_Immediate(),
                () => { throw new NotImplementedException(); },
                //0x30
                () => { throw new NotImplementedException(); },
                () => Instruction_0x31_Load_SP_With_16_Bit_Immediate(),
                () => Instruction_0x32_Load_Address_Pointed_To_By_HL_With_A_Then_Decrement_HL(),
                () => Instruction_0x33_Increment_SP(),
                () => { throw new NotImplementedException(); },
                () => { throw new NotImplementedException(); },
                () => Instruction_0x36_Load_Address_Pointed_To_By_HL_With_8_Bit_Immediate(),
                () => { throw new NotImplementedException(); },
                () => { throw new NotImplementedException(); },
                () => Instruction_0x39_Add_SP_To_HL(),
                () => Instruction_0x3A_Load_A_With_Address_Pointed_To_By_HL_Then_Decrement_HL(),
                () => Instruction_0x3B_Decrement_SP(),
                () => Instruction_0x3C_Increment_A(),
                () => Instruction_0x3D_Decrement_A(),
                () => Instruction_0x3E_Load_A_With_8_Bit_Immediate(),
                () => { throw new NotImplementedException(); },
                //0x40
                () => { },  //ld b, b => nop
                () => Instruction_0x41_Load_B_From_C(),
                () => Instruction_0x42_Load_B_From_D(),
                () => Instruction_0x43_Load_B_From_E(),
                () => Instruction_0x44_Load_B_From_H(),
                () => Instruction_0x45_Load_B_From_L(),
                () => Instruction_0x46_Load_B_From_Address_Pointed_To_By_HL(),
                () => Instruction_0x47_Load_B_From_A(),
                () => Instruction_0x48_Load_C_From_B(),
                () => { },  //ld c, c => nop
                () => Instruction_0x4A_Load_C_From_D(),
                () => Instruction_0x4B_Load_C_From_E(),
                () => Instruction_0x4C_Load_C_From_H(),
                () => Instruction_0x4D_Load_C_From_L(),
                () => Instruction_0x4E_Load_C_From_Address_Pointed_To_By_HL(),
                () => Instruction_0x4F_Load_C_From_A(),
                //0x50
                () => Instruction_0x50_Load_D_From_B(),
                () => Instruction_0x51_Load_D_From_C(),
                () => { },  //ld d, d => nop
                () => Instruction_0x53_Load_D_From_E(),
                () => Instruction_0x54_Load_D_From_H(),
                () => Instruction_0x55_Load_D_From_L(),
                () => Instruction_0x56_Load_D_From_Address_Pointed_To_By_HL(),
                () => Instruction_0x57_Load_D_From_A(),
                () => Instruction_0x58_Load_E_From_B(),
                () => Instruction_0x59_Load_E_From_C(),
                () => Instruction_0x5A_Load_E_From_D(),
                () => { },  //ld e, e => nop
                () => Instruction_0x5C_Load_E_From_H(),
                () => Instruction_0x5D_Load_E_From_L(),
                () => Instruction_0x5E_Load_E_From_Address_Pointed_To_By_HL(),
                () => Instruction_0x5F_Load_E_From_A(),
                //0x60
                () => Instruction_0x60_Load_H_From_B(),
                () => Instruction_0x61_Load_H_From_C(),
                () => Instruction_0x62_Load_H_From_D(),
                () => Instruction_0x63_Load_H_From_E(),
                () => { },  //ld h, h => nop
                () => Instruction_0x65_Load_H_From_L(),
                () => Instruction_0x66_Load_H_From_Address_Pointed_To_By_HL(),
                () => Instruction_0x67_Load_H_From_A(),
                () => Instruction_0x68_Load_L_From_B(),
                () => Instruction_0x69_Load_L_From_C(),
                () => Instruction_0x6A_Load_L_From_D(),
                () => Instruction_0x6B_Load_L_From_E(),
                () => Instruction_0x6C_Load_L_From_H(),
                () => { },  //ld l, l => nop
                () => Instruction_0x6C_Load_L_From_Address_Pointed_To_By_HL(),
                () => Instruction_0x6F_Load_L_From_A(),
                //0x70
                () => Instruction_0x70_Load_Address_Pointed_To_By_HL_With_B(),
                () => Instruction_0x71_Load_Address_Pointed_To_By_HL_With_C(),
                () => Instruction_0x72_Load_Address_Pointed_To_By_HL_With_D(),
                () => Instruction_0x73_Load_Address_Pointed_To_By_HL_With_E(),
                () => Instruction_0x74_Load_Address_Pointed_To_By_HL_With_H(),
                () => Instruction_0x75_Load_Address_Pointed_To_By_HL_With_L(),
                () => Instruction_0x76_Halt(),
                () => Instruction_0x77_Load_Address_Pointed_To_By_HL_With_A(),
                () => Instruction_0x78_Load_A_From_B(),
                () => Instruction_0x79_Load_A_From_C(),
                () => Instruction_0x7A_Load_A_From_D(),
                () => Instruction_0x7B_Load_A_From_E(),
                () => Instruction_0x7C_Load_A_From_H(),
                () => Instruction_0x7D_Load_A_From_L(),
                () => Instruction_0x7E_Load_A_From_Address_Pointed_To_By_HL(),
                () => { },  //ld a, a => nop (https://stackoverflow.com/questions/50187678/whats-the-purpose-of-instructions-for-loading-a-register-to-itself)
                //0x80
                () => Instruction_0x80_Add_B_To_A(),
                () => Instruction_0x81_Add_C_To_A(),
                () => Instruction_0x82_Add_D_To_A(),
                () => Instruction_0x83_Add_E_To_A(),
                () => Instruction_0x84_Add_H_To_A(),
                () => Instruction_0x85_Add_L_To_A(),
                () => Instruction_0x86_Add_Address_Pointed_To_By_HL_To_A(),
                () => Instruction_0x87_Add_A_To_A(),
                () => Instruction_0x88_Add_B_Plus_Carry_To_A(),
                () => Instruction_0x89_Add_C_Plus_Carry_To_A(),
                () => Instruction_0x8A_Add_D_Plus_Carry_To_A(),
                () => Instruction_0x8B_Add_E_Plus_Carry_To_A(),
                () => Instruction_0x8C_Add_H_Plus_Carry_To_A(),
                () => Instruction_0x8D_Add_L_Plus_Carry_To_A(),
                () => Instruction_0x8E_Add_Address_Pointed_To_By_HL_Plus_Carry_To_A(),
                () => Instruction_0x8F_Add_A_Plus_Carry_To_A(),
                //0x90
                () => Instruction_0x90_Subtract_B_From_A(),
                () => Instruction_0x91_Subtract_C_From_A(),
                () => Instruction_0x92_Subtract_D_From_A(),
                () => Instruction_0x93_Subtract_E_From_A(),
                () => Instruction_0x94_Subtract_H_From_A(),
                () => Instruction_0x95_Subtract_L_From_A(),
                () => Instruction_0x96_Subtract_Address_Pointed_To_By_HL_From_A(),
                () => Instruction_0x97_Subtract_A_From_A(),
                () => Instruction_0x98_Subtract_B_Plus_Carry_From_A(),
                () => Instruction_0x99_Subtract_C_Plus_Carry_From_A(),
                () => Instruction_0x9A_Subtract_D_Plus_Carry_From_A(),
                () => Instruction_0x9B_Subtract_E_Plus_Carry_From_A(),
                () => Instruction_0x9C_Subtract_H_Plus_Carry_From_A(),
                () => Instruction_0x9D_Subtract_L_Plus_Carry_From_A(),
                () => Instruction_0x9E_Subtract_Address_Pointed_To_By_HL_Plus_Carry_From_A(),
                () => Instruction_0x9F_Subtract_A_Plus_Carry_From_A(),
                //0xA0
                () => Instruction_0xA0_Bitwise_And_B_With_A(),
                () => Instruction_0xA1_Bitwise_And_C_With_A(),
                () => Instruction_0xA2_Bitwise_And_D_With_A(),
                () => Instruction_0xA3_Bitwise_And_E_With_A(),
                () => Instruction_0xA4_Bitwise_And_H_With_A(),
                () => Instruction_0xA5_Bitwise_And_L_With_A(),
                () => Instruction_0xA6_Bitwise_And_Address_Pointed_To_By_HL_With_A(),
                () => Instruction_0xA7_Bitwise_And_A_With_A(),
                () => Instruction_0xA8_Bitwise_Exclusive_Or_B_With_A(),
                () => Instruction_0xA9_Bitwise_Exclusive_Or_C_With_A(),
                () => Instruction_0xAA_Bitwise_Exclusive_Or_D_With_A(),
                () => Instruction_0xAB_Bitwise_Exclusive_Or_E_With_A(),
                () => Instruction_0xAC_Bitwise_Exclusive_Or_H_With_A(),
                () => Instruction_0xAD_Bitwise_Exclusive_Or_L_With_A(),
                () => Instruction_0xAE_Bitwise_Exclusive_Or_Address_Pointed_To_By_HL_With_A(),
                () => Instruction_0xAF_Bitwise_Exclusive_Or_A_With_A(),
                //0xB0
                () => Instruction_0xB0_Bitwise_Or_B_With_A(),
                () => Instruction_0xB1_Bitwise_Or_C_With_A(),
                () => Instruction_0xB2_Bitwise_Or_D_With_A(),
                () => Instruction_0xB3_Bitwise_Or_E_With_A(),
                () => Instruction_0xB4_Bitwise_Or_H_With_A(),
                () => Instruction_0xB5_Bitwise_Or_L_With_A(),
                () => Instruction_0xB6_Bitwise_Or_Address_Pointed_To_By_HL_With_A(),
                () => Instruction_0xB7_Bitwise_Or_A_With_A(),
                () => Instruction_0xB8_Compare_B_To_A_And_Set_Flags_As_If_It_Was_Subtracted_From_A(),
                () => Instruction_0xB9_Compare_C_To_A_And_Set_Flags_As_If_It_Was_Subtracted_From_A(),
                () => Instruction_0xBA_Compare_D_To_A_And_Set_Flags_As_If_It_Was_Subtracted_From_A(),
                () => Instruction_0xBB_Compare_E_To_A_And_Set_Flags_As_If_It_Was_Subtracted_From_A(),
                () => Instruction_0xBC_Compare_H_To_A_And_Set_Flags_As_If_It_Was_Subtracted_From_A(),
                () => Instruction_0xBD_Compare_L_To_A_And_Set_Flags_As_If_It_Was_Subtracted_From_A(),
                () => Instruction_0xBE_Compare_Address_Pointed_To_By_HL_To_A_And_Set_Flags_As_If_It_Was_Subtracted_From_A(),
                () => Instruction_0xBF_Compare_A_To_A_And_Set_Flags_As_If_It_Was_Subtracted_From_A(),
                //0xC0
                () => { throw new NotImplementedException(); },
                () => Instruction_0xC1_Pop_Stack_Into_BC(),
                () => { throw new NotImplementedException(); },
                () => { throw new NotImplementedException(); },
                () => { throw new NotImplementedException(); },
                () => Instruction_0xC5_Push_BC_Onto_Stack(),
                () => Instruction_0xC6_Add_8_Bit_Immediate_To_A(),
                () => { throw new NotImplementedException(); },
                () => { throw new NotImplementedException(); },
                () => { throw new NotImplementedException(); },
                () => { throw new NotImplementedException(); },
                () => { throw new NotImplementedException(); }, //CB prefix instructions
                () => { throw new NotImplementedException(); },
                () => { throw new NotImplementedException(); },
                () => Instruction_0xCE_Add_8_Bit_Immediate_Plus_Carry_To_A(),
                () => { throw new NotImplementedException(); },
                //0xD0
                () => { throw new NotImplementedException(); },
                () => Instruction_0xD1_Pop_Stack_Into_DE(),
                () => { throw new NotImplementedException(); },
                () => { throw new NotImplementedException(); },
                () => { throw new NotImplementedException(); },
                () => Instruction_0xD5_Push_DE_Onto_Stack(),
                () => Instruction_0xD6_Subtract_8_Bit_Immediate_From_A(),
                () => { throw new NotImplementedException(); },
                () => { throw new NotImplementedException(); },
                () => { throw new NotImplementedException(); },
                () => { throw new NotImplementedException(); },
                () => { throw new NotImplementedException(); },
                () => { throw new NotImplementedException(); },
                () => { throw new NotImplementedException(); },
                () => Instruction_0xDE_Subtract_8_Bit_Immediate_Plus_Carry_From_A(),
                () => { throw new NotImplementedException(); },
                //0xE0
                () => Instruction_0xE0_Load_A_Into_High_Memory_Address_Offset_By_Unsigned_8_Bit_Immediate(),
                () => Instruction_0xE1_Pop_Stack_Into_HL(),
                () => Instruction_0xE2_Load_A_Into_High_Memory_Address_Offset_By_C(),
                () => { throw new NotImplementedException(); },
                () => { throw new NotImplementedException(); },
                () => Instruction_0xE5_Push_HL_Onto_Stack(),
                () => Instruction_0xE6_Bitwise_And_8_Bit_Immediate_With_A(),
                () => { throw new NotImplementedException(); },
                () => { throw new NotImplementedException(); },
                () => { throw new NotImplementedException(); },
                () => Instruction_0xEA_Load_Immediate_Memory_Location_From_A(),
                () => { throw new NotImplementedException(); },
                () => { throw new NotImplementedException(); },
                () => { throw new NotImplementedException(); },
                () => Instruction_0xEE_Bitwise_Exclusive_Or_8_Bit_Immediate_With_A(),
                () => { throw new NotImplementedException(); },
                //0xF0
                () => Instruction_0xF0_Load_A_From_High_Memory_Address_Offset_By_8_Bit_Immediate(),
                () => Instruction_0xF1_Pop_Stack_Into_AF(),
                () => Instruction_0xF2_Load_A_From_High_Memory_Address_Offset_By_C(),
                () => { throw new NotImplementedException(); },
                () => { throw new NotImplementedException(); },
                () => Instruction_0xF5_Push_AF_Onto_Stack(),
                () => { throw new NotImplementedException(); },
                () => { throw new NotImplementedException(); },
                () => Instruction_0xF8_Add_8_Bit_Signed_Immediate_To_Stack_Pointer_And_Store_Result_In_HL(),
                () => Instruction_0xF9_Load_Stack_Pointer_From_HL(),
                () => Instruction_0xFA_Load_A_From_Immediate_Memory_Location(),
                () => { throw new NotImplementedException(); },
                () => { throw new NotImplementedException(); },
                () => { throw new NotImplementedException(); },
                () => { throw new NotImplementedException(); },
                () => { throw new NotImplementedException(); },
            };
        }

        /// <summary>
        /// Implements a single iteration of the processor's fetch/decode/execute cycle.
        /// </summary>
        public void Tick()
        {
            if (IsHalted) return;
            byte opcode = Fetch();
            Execute(opcode);
        }

        /// <summary>
        /// Retrieves the next instruction from memory and increments the program counter.
        /// </summary>
        private byte Fetch()
        {
            return Memory[Registers.PC++];
        }

        /// <summary>
        /// Executes the instruction represented by the given opcode.
        /// </summary>
        private void Execute(byte opcode)
        {
            instructionSet[opcode]();
        }

        /// <summary>
        /// Pushes the given register pair onto the stack and decrements the stack pointer by 2.
        /// </summary>
        private void PushOntoStack(byte high, byte low)
        {
            Registers.SP--;
            Memory[Registers.SP] = high;
            Registers.SP--;
            Memory[Registers.SP] = low;
        }

        /// <summary>
        /// Returns the 16-bit value at the top of the stack and increments the stack pointer by 2.
        /// </summary>
        private ushort PopStack()
        {
            byte low = Memory[Registers.SP++];
            byte high = Memory[Registers.SP++];
            return Common.FromLittleEndian(low, high);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r16,n16
        /// </summary>
        private void Instruction_0x01_Load_BC_With_16_Bit_Immediate()
        {
            Registers.BC = Common.FromLittleEndian(Fetch(), Fetch());
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD__r16_,A
        /// </summary>
        private void Instruction_0x02_Load_Address_Pointed_To_By_BC_With_A()
        {
            Memory[Registers.BC] = Registers.A;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#INC_r16
        /// </summary>
        private void Instruction_0x03_Increment_BC()
        {
            Registers.BC++;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#INC_r8
        /// </summary>
        private void Instruction_0x04_Increment_B()
        {
            Registers.B = Increment8BitRegisterAndSetFlags(Registers.B);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#DEC_r8
        /// </summary>
        private void Instruction_0x05_Decrement_B()
        {
            Registers.B = Decrement8BitRegisterAndSetFlags(Registers.B);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,n8
        /// </summary>
        private void Instruction_0x06_Load_B_With_8_Bit_Immediate()
        {
            Registers.B = Fetch();
        }

        /// <summary>
        /// Shifts every bit of A left by one position. The 7th bit of A
        /// is also copied into the carry flag and the 0th bit of A, e.g.
        /// C <- [7 <- 0] <- [7]
        /// </summary>
        /// <remarks>
        /// RLCA differs from RLA in that RLCA is "circular", meaning the
        /// bits of A always circle around and are preserved. RLA copies the
        /// old carry value into bit 0 *before* copying bit 7 into the carry.
        /// </remarks>
        /// <see cref="https://rednex.github.io/rgbds/gbz80.7.html#RLCA"/>
        /// <see cref="https://ez80.readthedocs.io/en/latest/docs/bit-shifts/rla.html"/>
        /// <see cref="https://stackoverflow.com/questions/812022/c-sharp-bitwise-rotate-left-and-rotate-right"/>
        /// <see cref="https://github.com/sinamas/gambatte/blob/master/libgambatte/src/cpu.cpp#L561"/>
        private void Instruction_0x07_Rotate_A_Left_Circular()
        {
            Registers.A = (byte)((Registers.A << 1) | (Registers.A >> 7));
            Registers.SetFlagTo(Flags.Carry, (Registers.A & 0b0000_0001) != 0); //set carry to LSB (original MSB)
            Registers.ClearFlag(Flags.Zero | Flags.AddSubtract | Flags.HalfCarry);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD__n16_,SP
        /// https://github.com/sinamas/gambatte/blob/master/libgambatte/src/cpu.cpp#L570
        /// </summary>
        private void Instruction_0x08_Load_Address_With_Stack_Pointer()
        {
            byte addressLow = Fetch();
            byte addressHigh = Fetch();
            ushort address = Common.FromLittleEndian(addressLow, addressHigh);

            Memory[address] = (byte)(Registers.SP & 0xFF);
            Memory[address + 1] = (byte)(Registers.SP >> 8);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#ADD_HL,r16
        /// </summary>
        private void Instruction_0x09_Add_BC_To_HL()
        {
            AddToHLAndSetFlags(Registers.BC);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_A,_r16_
        /// </summary>
        private void Instruction_0x0A_Load_A_From_Address_Pointed_To_By_BC()
        {
            Registers.A = Memory[Registers.BC];
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#DEC_r16
        /// </summary>
        private void Instruction_0x0B_Decrement_BC()
        {
            Registers.BC--;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#INC_r8
        /// </summary>
        private void Instruction_0x0C_Increment_C()
        {
            Registers.C = Increment8BitRegisterAndSetFlags(Registers.C);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#DEC_r8
        /// </summary>
        private void Instruction_0x0D_Decrement_C()
        {
            Registers.C = Decrement8BitRegisterAndSetFlags(Registers.C);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,n8
        /// </summary>
        private void Instruction_0x0E_Load_C_With_8_Bit_Immediate()
        {
            Registers.C = Fetch();
        }

        /// <summary>
        /// Shifts every bit of A right by one position. The 0th bit of A
        /// is also copied into the carry flag and the 7th bit of A, e.g.
        /// [0] -> [7 -> 0] -> C
        /// </summary>
        /// <see cref="https://rednex.github.io/rgbds/gbz80.7.html#RRCA"/>
        private void Instruction_0x0F_Rotate_A_Right_Circular()
        {
            Registers.A = (byte)((Registers.A >> 1) | (Registers.A << 7));
            Registers.SetFlagTo(Flags.Carry, (Registers.A & 0b1000_0000) != 0); //set carry to MSB (original LSB)
            Registers.ClearFlag(Flags.Zero | Flags.AddSubtract | Flags.HalfCarry);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#STOP
        /// https://github.com/sinamas/gambatte/blob/master/libgambatte/src/cpu.cpp#L613
        /// </summary>
        private void Instruction_0x10_Stop()
        {
            //is this all this should do?
            IsHalted = true;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r16,n16
        /// </summary>
        private void Instruction_0x11_Load_DE_With_16_Bit_Immediate()
        {
            Registers.DE = Common.FromLittleEndian(Fetch(), Fetch());
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD__r16_,A
        /// </summary>
        private void Instruction_0x12_Load_Address_Pointed_To_By_DE_With_A()
        {
            Memory[Registers.DE] = Registers.A;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#INC_r16
        /// </summary>
        private void Instruction_0x13_Increment_DE()
        {
            Registers.DE++;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#INC_r8
        /// </summary>
        private void Instruction_0x14_Increment_D()
        {
            Registers.D = Increment8BitRegisterAndSetFlags(Registers.D);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#DEC_r8
        /// </summary>
        private void Instruction_0x15_Decrement_D()
        {
            Registers.D = Decrement8BitRegisterAndSetFlags(Registers.D);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,n8
        /// </summary>
        private void Instruction_0x16_Load_D_With_8_Bit_Immediate()
        {
            Registers.D = Fetch();
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RLA
        /// https://ez80.readthedocs.io/en/latest/docs/bit-shifts/rla.html
        /// </summary>
        private void Instruction_0x17_Rotate_A_Left()
        {
            bool oldCarry = Registers.HasFlag(Flags.Carry);
            Registers.SetFlagTo(Flags.Carry, (Registers.A & 0b1000_0000) != 0);
            Registers.A = (byte)((Registers.A << 1) | (oldCarry ? 1 : 0));
            Registers.ClearFlag(Flags.Zero | Flags.AddSubtract | Flags.HalfCarry);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#JR_e8
        /// </summary>
        private void Instruction_0x18_Relative_Jump_By_8_Bit_Signed_Immediate()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#ADD_HL,r16
        /// </summary>
        private void Instruction_0x19_Add_DE_To_HL()
        {
            AddToHLAndSetFlags(Registers.DE);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_A,_r16_
        /// </summary>
        private void Instruction_0x1A_Load_A_From_Address_Pointed_To_By_DE()
        {
            Registers.A = Memory[Registers.DE];
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#DEC_r16
        /// </summary>
        private void Instruction_0x1B_Decrement_DE()
        {
            Registers.DE--;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#INC_r8
        /// </summary>
        private void Instruction_0x1C_Increment_E()
        {
            Registers.E = Increment8BitRegisterAndSetFlags(Registers.E);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#DEC_r8
        /// </summary>
        private void Instruction_0x1D_Decrement_E()
        {
            Registers.E = Decrement8BitRegisterAndSetFlags(Registers.E);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,n8
        /// </summary>
        private void Instruction_0x1E_Load_E_With_8_Bit_Immediate()
        {
            Registers.E = Fetch();
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RRA
        /// </summary>
        private void Instruction_0x1F_Rotate_A_Right()
        {
            bool oldCarry = Registers.HasFlag(Flags.Carry);
            Registers.SetFlagTo(Flags.Carry, (Registers.A & 0b0000_0001) != 0);
            Registers.A = (byte)((Registers.A >> 1) | (oldCarry ? 1 << 7 : 0));
            Registers.ClearFlag(Flags.Zero | Flags.AddSubtract | Flags.HalfCarry);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r16,n16
        /// </summary>
        private void Instruction_0x21_Load_HL_With_16_Bit_Immediate()
        {
            Registers.HL = Common.FromLittleEndian(Fetch(), Fetch());
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD__HL+_,A
        /// </summary>
        /// <remarks>
        /// Also known as: ldi [hl], a
        /// </remarks>
        private void Instruction_0x22_Load_Address_Pointed_To_By_HL_With_A_Then_Increment_HL()
        {
            Memory[Registers.HL++] = Registers.A;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#INC_r16
        /// </summary>
        private void Instruction_0x23_Increment_HL()
        {
            Registers.HL++;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#INC_r8
        /// </summary>
        private void Instruction_0x24_Increment_H()
        {
            Registers.H = Increment8BitRegisterAndSetFlags(Registers.H);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#DEC_r8
        /// </summary>
        private void Instruction_0x25_Decrement_H()
        {
            Registers.H = Decrement8BitRegisterAndSetFlags(Registers.H);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,n8
        /// </summary>
        private void Instruction_0x26_Load_H_With_8_Bit_Immediate()
        {
            Registers.H = Fetch();
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#ADD_HL,r16
        /// </summary>
        private void Instruction_0x29_Add_HL_To_HL()
        {
            AddToHLAndSetFlags(Registers.HL);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_A,_HL+_
        /// </summary>
        /// <remarks>
        /// Also known as: ldi a, [hl]
        /// </remarks>
        private void Instruction_0x2A_Load_A_With_Address_Pointed_To_By_HL_Then_Increment_HL()
        {
            Registers.A = Memory[Registers.HL++];
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#DEC_r16
        /// </summary>
        private void Instruction_0x2B_Decrement_HL()
        {
            Registers.HL--;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#INC_r8
        /// </summary>
        private void Instruction_0x2C_Increment_L()
        {
            Registers.L = Increment8BitRegisterAndSetFlags(Registers.L);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#DEC_r8
        /// </summary>
        private void Instruction_0x2D_Decrement_L()
        {
            Registers.L = Decrement8BitRegisterAndSetFlags(Registers.L);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,n8
        /// </summary>
        private void Instruction_0x2E_Load_L_With_8_Bit_Immediate()
        {
            Registers.L = Fetch();
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r16,n16
        /// </summary>
        private void Instruction_0x31_Load_SP_With_16_Bit_Immediate()
        {
            Registers.SP = Common.FromLittleEndian(Fetch(), Fetch());
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD__HL-_,A
        /// </summary>
        /// <remarks>
        /// Also known as: ldd [hl], a
        /// </remarks>
        private void Instruction_0x32_Load_Address_Pointed_To_By_HL_With_A_Then_Decrement_HL()
        {
            Memory[Registers.HL--] = Registers.A;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#INC_SP
        /// </summary>
        private void Instruction_0x33_Increment_SP()
        {
            Registers.SP++;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD__HL_,n8
        /// </summary>
        private void Instruction_0x36_Load_Address_Pointed_To_By_HL_With_8_Bit_Immediate()
        {
            Memory[Registers.HL] = Fetch();
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#ADD_HL,r16
        /// </summary>
        private void Instruction_0x39_Add_SP_To_HL()
        {
            AddToHLAndSetFlags(Registers.SP);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_A,_HL-_
        /// </summary>
        /// <remarks>
        /// Also known as: ldd a, [hl]
        /// </remarks>
        private void Instruction_0x3A_Load_A_With_Address_Pointed_To_By_HL_Then_Decrement_HL()
        {
            Registers.A = Memory[Registers.HL--];
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#DEC_SP
        /// </summary>
        private void Instruction_0x3B_Decrement_SP()
        {
            Registers.SP--;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#INC_r8
        /// </summary>
        private void Instruction_0x3C_Increment_A()
        {
            Registers.A = Increment8BitRegisterAndSetFlags(Registers.A);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#DEC_r8
        /// </summary>
        private void Instruction_0x3D_Decrement_A()
        {
            Registers.A = Decrement8BitRegisterAndSetFlags(Registers.A);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,n8
        /// </summary>
        private void Instruction_0x3E_Load_A_With_8_Bit_Immediate()
        {
            Registers.A = Fetch();
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,r8
        /// </summary>
        private void Instruction_0x41_Load_B_From_C()
        {
            Registers.B = Registers.C;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,r8
        /// </summary>
        private void Instruction_0x42_Load_B_From_D()
        {
            Registers.B = Registers.D;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,r8
        /// </summary>
        private void Instruction_0x43_Load_B_From_E()
        {
            Registers.B = Registers.E;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,r8
        /// </summary>
        private void Instruction_0x44_Load_B_From_H()
        {
            Registers.B = Registers.H;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,r8
        /// </summary>
        private void Instruction_0x45_Load_B_From_L()
        {
            Registers.B = Registers.L;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,_HL_
        /// </summary>
        private void Instruction_0x46_Load_B_From_Address_Pointed_To_By_HL()
        {
            Registers.B = Memory[Registers.HL];
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,r8
        /// </summary>
        private void Instruction_0x47_Load_B_From_A()
        {
            Registers.B = Registers.A;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,r8
        /// </summary>
        private void Instruction_0x48_Load_C_From_B()
        {
            Registers.C = Registers.B;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,r8
        /// </summary>
        private void Instruction_0x4A_Load_C_From_D()
        {
            Registers.C = Registers.D;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,r8
        /// </summary>
        private void Instruction_0x4B_Load_C_From_E()
        {
            Registers.C = Registers.E;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,r8
        /// </summary>
        private void Instruction_0x4C_Load_C_From_H()
        {
            Registers.C = Registers.H;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,r8
        /// </summary>
        private void Instruction_0x4D_Load_C_From_L()
        {
            Registers.C = Registers.L;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,_HL_
        /// </summary>
        private void Instruction_0x4E_Load_C_From_Address_Pointed_To_By_HL()
        {
            Registers.C = Memory[Registers.HL];
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,r8
        /// </summary>
        private void Instruction_0x4F_Load_C_From_A()
        {
            Registers.C = Registers.A;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,r8
        /// </summary>
        private void Instruction_0x50_Load_D_From_B()
        {
            Registers.D = Registers.B;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,r8
        /// </summary>
        private void Instruction_0x51_Load_D_From_C()
        {
            Registers.D = Registers.C;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,r8
        /// </summary>
        private void Instruction_0x53_Load_D_From_E()
        {
            Registers.D = Registers.E;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,r8
        /// </summary>
        private void Instruction_0x54_Load_D_From_H()
        {
            Registers.D = Registers.H;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,r8
        /// </summary>
        private void Instruction_0x55_Load_D_From_L()
        {
            Registers.D = Registers.L;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,_HL_
        /// </summary>
        private void Instruction_0x56_Load_D_From_Address_Pointed_To_By_HL()
        {
            Registers.D = Memory[Registers.HL];
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,r8
        /// </summary>
        private void Instruction_0x57_Load_D_From_A()
        {
            Registers.D = Registers.A;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,r8
        /// </summary>
        private void Instruction_0x58_Load_E_From_B()
        {
            Registers.E = Registers.B;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,r8
        /// </summary>
        private void Instruction_0x59_Load_E_From_C()
        {
            Registers.E = Registers.C;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,r8
        /// </summary>
        private void Instruction_0x5A_Load_E_From_D()
        {
            Registers.E = Registers.D;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,r8
        /// </summary>
        private void Instruction_0x5C_Load_E_From_H()
        {
            Registers.E = Registers.H;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,r8
        /// </summary>
        private void Instruction_0x5D_Load_E_From_L()
        {
            Registers.E = Registers.L;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,_HL_
        /// </summary>
        private void Instruction_0x5E_Load_E_From_Address_Pointed_To_By_HL()
        {
            Registers.E = Memory[Registers.HL];
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,r8
        /// </summary>
        private void Instruction_0x5F_Load_E_From_A()
        {
            Registers.E = Registers.A;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,r8
        /// </summary>
        private void Instruction_0x60_Load_H_From_B()
        {
            Registers.H = Registers.B;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,r8
        /// </summary>
        private void Instruction_0x61_Load_H_From_C()
        {
            Registers.H = Registers.C;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,r8
        /// </summary>
        private void Instruction_0x62_Load_H_From_D()
        {
            Registers.H = Registers.D;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,r8
        /// </summary>
        private void Instruction_0x63_Load_H_From_E()
        {
            Registers.H = Registers.E;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,r8
        /// </summary>
        private void Instruction_0x65_Load_H_From_L()
        {
            Registers.H = Registers.L;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,_HL_
        /// </summary>
        private void Instruction_0x66_Load_H_From_Address_Pointed_To_By_HL()
        {
            Registers.H = Memory[Registers.HL];
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,r8
        /// </summary>
        private void Instruction_0x67_Load_H_From_A()
        {
            Registers.H = Registers.A;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,r8
        /// </summary>
        private void Instruction_0x68_Load_L_From_B()
        {
            Registers.L = Registers.B;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,r8
        /// </summary>
        private void Instruction_0x69_Load_L_From_C()
        {
            Registers.L = Registers.C;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,r8
        /// </summary>
        private void Instruction_0x6A_Load_L_From_D()
        {
            Registers.L = Registers.D;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,r8
        /// </summary>
        private void Instruction_0x6B_Load_L_From_E()
        {
            Registers.L = Registers.E;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,r8
        /// </summary>
        private void Instruction_0x6C_Load_L_From_H()
        {
            Registers.L = Registers.H;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,_HL_
        /// </summary>
        private void Instruction_0x6C_Load_L_From_Address_Pointed_To_By_HL()
        {
            Registers.L = Memory[Registers.HL];
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,r8
        /// </summary>
        private void Instruction_0x6F_Load_L_From_A()
        {
            Registers.L = Registers.A;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD__HL_,r8
        /// </summary>
        private void Instruction_0x70_Load_Address_Pointed_To_By_HL_With_B()
        {
            Memory[Registers.HL] = Registers.B;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD__HL_,r8
        /// </summary>
        private void Instruction_0x71_Load_Address_Pointed_To_By_HL_With_C()
        {
            Memory[Registers.HL] = Registers.C;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD__HL_,r8
        /// </summary>
        private void Instruction_0x72_Load_Address_Pointed_To_By_HL_With_D()
        {
            Memory[Registers.HL] = Registers.D;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD__HL_,r8
        /// </summary>
        private void Instruction_0x73_Load_Address_Pointed_To_By_HL_With_E()
        {
            Memory[Registers.HL] = Registers.E;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD__HL_,r8
        /// </summary>
        private void Instruction_0x74_Load_Address_Pointed_To_By_HL_With_H()
        {
            Memory[Registers.HL] = Registers.H;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD__HL_,r8
        /// </summary>
        private void Instruction_0x75_Load_Address_Pointed_To_By_HL_With_L()
        {
            Memory[Registers.HL] = Registers.L;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#HALT
        /// </summary>
        private void Instruction_0x76_Halt()
        {
            IsHalted = true;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD__HL_,r8
        /// </summary>
        private void Instruction_0x77_Load_Address_Pointed_To_By_HL_With_A()
        {
            Memory[Registers.HL] = Registers.A;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,r8
        /// </summary>
        private void Instruction_0x78_Load_A_From_B()
        {
            Registers.A = Registers.B;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,r8
        /// </summary>
        private void Instruction_0x79_Load_A_From_C()
        {
            Registers.A = Registers.C;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,r8
        /// </summary>
        private void Instruction_0x7A_Load_A_From_D()
        {
            Registers.A = Registers.D;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,r8
        /// </summary>
        private void Instruction_0x7B_Load_A_From_E()
        {
            Registers.A = Registers.E;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,r8
        /// </summary>
        private void Instruction_0x7C_Load_A_From_H()
        {
            Registers.A = Registers.H;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,r8
        /// </summary>
        private void Instruction_0x7D_Load_A_From_L()
        {
            Registers.A = Registers.L;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,_HL_
        /// </summary>
        private void Instruction_0x7E_Load_A_From_Address_Pointed_To_By_HL()
        {
            Registers.A = Memory[Registers.HL];
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#ADD_A,r8
        /// </summary>
        private void Instruction_0x80_Add_B_To_A()
        {
            AddToAccumulatorAndSetFlags(Registers.B);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#ADD_A,r8
        /// </summary>
        private void Instruction_0x81_Add_C_To_A()
        {
            AddToAccumulatorAndSetFlags(Registers.C);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#ADD_A,r8
        /// </summary>
        private void Instruction_0x82_Add_D_To_A()
        {
            AddToAccumulatorAndSetFlags(Registers.D);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#ADD_A,r8
        /// </summary>
        private void Instruction_0x83_Add_E_To_A()
        {
            AddToAccumulatorAndSetFlags(Registers.E);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#ADD_A,r8
        /// </summary>
        private void Instruction_0x84_Add_H_To_A()
        {
            AddToAccumulatorAndSetFlags(Registers.H);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#ADD_A,r8
        /// </summary>
        private void Instruction_0x85_Add_L_To_A()
        {
            AddToAccumulatorAndSetFlags(Registers.L);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#ADD_A,_HL_
        /// </summary>
        private void Instruction_0x86_Add_Address_Pointed_To_By_HL_To_A()
        {
            AddToAccumulatorAndSetFlags(Memory[Registers.HL]);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#ADD_A,r8
        /// </summary>
        private void Instruction_0x87_Add_A_To_A()
        {
            AddToAccumulatorAndSetFlags(Registers.A);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#ADC_A,r8
        /// </summary>
        private void Instruction_0x88_Add_B_Plus_Carry_To_A()
        {
            AddToAccumulatorAndSetFlags(Registers.B, carryBit: Registers.HasFlag(Flags.Carry));
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#ADC_A,r8
        /// </summary>
        private void Instruction_0x89_Add_C_Plus_Carry_To_A()
        {
            AddToAccumulatorAndSetFlags(Registers.C, carryBit: Registers.HasFlag(Flags.Carry));
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#ADC_A,r8
        /// </summary>
        private void Instruction_0x8A_Add_D_Plus_Carry_To_A()
        {
            AddToAccumulatorAndSetFlags(Registers.D, carryBit: Registers.HasFlag(Flags.Carry));
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#ADC_A,r8
        /// </summary>
        private void Instruction_0x8B_Add_E_Plus_Carry_To_A()
        {
            AddToAccumulatorAndSetFlags(Registers.E, carryBit: Registers.HasFlag(Flags.Carry));
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#ADC_A,r8
        /// </summary>
        private void Instruction_0x8C_Add_H_Plus_Carry_To_A()
        {
            AddToAccumulatorAndSetFlags(Registers.H, carryBit: Registers.HasFlag(Flags.Carry));
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#ADC_A,r8
        /// </summary>
        private void Instruction_0x8D_Add_L_Plus_Carry_To_A()
        {
            AddToAccumulatorAndSetFlags(Registers.L, carryBit: Registers.HasFlag(Flags.Carry));
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#ADC_A,_HL_
        /// </summary>
        private void Instruction_0x8E_Add_Address_Pointed_To_By_HL_Plus_Carry_To_A()
        {
            AddToAccumulatorAndSetFlags(Memory[Registers.HL], carryBit: Registers.HasFlag(Flags.Carry));
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#ADC_A,r8
        /// </summary>
        private void Instruction_0x8F_Add_A_Plus_Carry_To_A()
        {
            AddToAccumulatorAndSetFlags(Registers.A, carryBit: Registers.HasFlag(Flags.Carry));
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SUB_A,r8
        /// </summary>
        private void Instruction_0x90_Subtract_B_From_A()
        {
            SubtractFromAccumulatorAndSetFlags(Registers.B);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SUB_A,r8
        /// </summary>
        private void Instruction_0x91_Subtract_C_From_A()
        {
            SubtractFromAccumulatorAndSetFlags(Registers.C);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SUB_A,r8
        /// </summary>
        private void Instruction_0x92_Subtract_D_From_A()
        {
            SubtractFromAccumulatorAndSetFlags(Registers.D);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SUB_A,r8
        /// </summary>
        private void Instruction_0x93_Subtract_E_From_A()
        {
            SubtractFromAccumulatorAndSetFlags(Registers.E);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SUB_A,r8
        /// </summary>
        private void Instruction_0x94_Subtract_H_From_A()
        {
            SubtractFromAccumulatorAndSetFlags(Registers.H);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SUB_A,r8
        /// </summary>
        private void Instruction_0x95_Subtract_L_From_A()
        {
            SubtractFromAccumulatorAndSetFlags(Registers.L);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SUB_A,_HL_
        /// </summary>
        private void Instruction_0x96_Subtract_Address_Pointed_To_By_HL_From_A()
        {
            SubtractFromAccumulatorAndSetFlags(Memory[Registers.HL]);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SUB_A,r8
        /// </summary>
        private void Instruction_0x97_Subtract_A_From_A()
        {
            SubtractFromAccumulatorAndSetFlags(Registers.A);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SBC_A,r8
        /// </summary>
        private void Instruction_0x98_Subtract_B_Plus_Carry_From_A()
        {
            SubtractFromAccumulatorAndSetFlags(Registers.B, carryBit: Registers.HasFlag(Flags.Carry));
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SBC_A,r8
        /// </summary>
        private void Instruction_0x99_Subtract_C_Plus_Carry_From_A()
        {
            SubtractFromAccumulatorAndSetFlags(Registers.C, carryBit: Registers.HasFlag(Flags.Carry));
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SBC_A,r8
        /// </summary>
        private void Instruction_0x9A_Subtract_D_Plus_Carry_From_A()
        {
            SubtractFromAccumulatorAndSetFlags(Registers.D, carryBit: Registers.HasFlag(Flags.Carry));
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SBC_A,r8
        /// </summary>
        private void Instruction_0x9B_Subtract_E_Plus_Carry_From_A()
        {
            SubtractFromAccumulatorAndSetFlags(Registers.E, carryBit: Registers.HasFlag(Flags.Carry));
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SBC_A,r8
        /// </summary>
        private void Instruction_0x9C_Subtract_H_Plus_Carry_From_A()
        {
            SubtractFromAccumulatorAndSetFlags(Registers.H, carryBit: Registers.HasFlag(Flags.Carry));
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SBC_A,r8
        /// </summary>
        private void Instruction_0x9D_Subtract_L_Plus_Carry_From_A()
        {
            SubtractFromAccumulatorAndSetFlags(Registers.L, carryBit: Registers.HasFlag(Flags.Carry));
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SBC_A,_HL_
        /// </summary>
        private void Instruction_0x9E_Subtract_Address_Pointed_To_By_HL_Plus_Carry_From_A()
        {
            SubtractFromAccumulatorAndSetFlags(Memory[Registers.HL], carryBit: Registers.HasFlag(Flags.Carry));
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SBC_A,r8
        /// </summary>
        private void Instruction_0x9F_Subtract_A_Plus_Carry_From_A()
        {
            SubtractFromAccumulatorAndSetFlags(Registers.A, carryBit: Registers.HasFlag(Flags.Carry));
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#AND_A,r8
        /// </summary>
        private void Instruction_0xA0_Bitwise_And_B_With_A()
        {
            AndWithAccumulatorAndSetFlags(Registers.B);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#AND_A,r8
        /// </summary>
        private void Instruction_0xA1_Bitwise_And_C_With_A()
        {
            AndWithAccumulatorAndSetFlags(Registers.C);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#AND_A,r8
        /// </summary>
        private void Instruction_0xA2_Bitwise_And_D_With_A()
        {
            AndWithAccumulatorAndSetFlags(Registers.D);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#AND_A,r8
        /// </summary>
        private void Instruction_0xA3_Bitwise_And_E_With_A()
        {
            AndWithAccumulatorAndSetFlags(Registers.E);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#AND_A,r8
        /// </summary>
        private void Instruction_0xA4_Bitwise_And_H_With_A()
        {
            AndWithAccumulatorAndSetFlags(Registers.H);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#AND_A,r8
        /// </summary>
        private void Instruction_0xA5_Bitwise_And_L_With_A()
        {
            AndWithAccumulatorAndSetFlags(Registers.L);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#AND_A,_HL_
        /// </summary>
        private void Instruction_0xA6_Bitwise_And_Address_Pointed_To_By_HL_With_A()
        {
            AndWithAccumulatorAndSetFlags(Registers.B);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#AND_A,r8
        /// </summary>
        private void Instruction_0xA7_Bitwise_And_A_With_A()
        {
            AndWithAccumulatorAndSetFlags(Registers.A);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#XOR_A,r8
        /// </summary>
        private void Instruction_0xA8_Bitwise_Exclusive_Or_B_With_A()
        {
            XorWithAccumulatorAndSetFlags(Registers.B);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#XOR_A,r8
        /// </summary>
        private void Instruction_0xA9_Bitwise_Exclusive_Or_C_With_A()
        {
            XorWithAccumulatorAndSetFlags(Registers.C);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#XOR_A,r8
        /// </summary>
        private void Instruction_0xAA_Bitwise_Exclusive_Or_D_With_A()
        {
            XorWithAccumulatorAndSetFlags(Registers.D);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#XOR_A,r8
        /// </summary>
        private void Instruction_0xAB_Bitwise_Exclusive_Or_E_With_A()
        {
            XorWithAccumulatorAndSetFlags(Registers.E);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#XOR_A,r8
        /// </summary>
        private void Instruction_0xAC_Bitwise_Exclusive_Or_H_With_A()
        {
            XorWithAccumulatorAndSetFlags(Registers.H);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#XOR_A,r8
        /// </summary>
        private void Instruction_0xAD_Bitwise_Exclusive_Or_L_With_A()
        {
            XorWithAccumulatorAndSetFlags(Registers.L);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#XOR_A,_HL_
        /// </summary>
        private void Instruction_0xAE_Bitwise_Exclusive_Or_Address_Pointed_To_By_HL_With_A()
        {
            XorWithAccumulatorAndSetFlags(Memory[Registers.HL]);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#XOR_A,r8
        /// </summary>
        /// <remarks>
        /// Conventionally used to zero out the accumulator (takes 1 less cycle than ld a, 0)
        /// </remarks>
        private void Instruction_0xAF_Bitwise_Exclusive_Or_A_With_A()
        {
            XorWithAccumulatorAndSetFlags(Registers.A);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#OR_A,r8
        /// </summary>
        private void Instruction_0xB0_Bitwise_Or_B_With_A()
        {
            OrWithAccumulatorAndSetFlags(Registers.B);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#OR_A,r8
        /// </summary>
        private void Instruction_0xB1_Bitwise_Or_C_With_A()
        {
            OrWithAccumulatorAndSetFlags(Registers.C);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#OR_A,r8
        /// </summary>
        private void Instruction_0xB2_Bitwise_Or_D_With_A()
        {
            OrWithAccumulatorAndSetFlags(Registers.D);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#OR_A,r8
        /// </summary>
        private void Instruction_0xB3_Bitwise_Or_E_With_A()
        {
            OrWithAccumulatorAndSetFlags(Registers.E);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#OR_A,r8
        /// </summary>
        private void Instruction_0xB4_Bitwise_Or_H_With_A()
        {
            OrWithAccumulatorAndSetFlags(Registers.H);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#OR_A,r8
        /// </summary>
        private void Instruction_0xB5_Bitwise_Or_L_With_A()
        {
            OrWithAccumulatorAndSetFlags(Registers.L);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#OR_A,_HL_
        /// </summary>
        private void Instruction_0xB6_Bitwise_Or_Address_Pointed_To_By_HL_With_A()
        {
            OrWithAccumulatorAndSetFlags(Memory[Registers.HL]);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#OR_A,r8
        /// </summary>
        private void Instruction_0xB7_Bitwise_Or_A_With_A()
        {
            OrWithAccumulatorAndSetFlags(Registers.A);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#CP_A,r8
        /// </summary>
        private void Instruction_0xB8_Compare_B_To_A_And_Set_Flags_As_If_It_Was_Subtracted_From_A()
        {
            CompareToAccumulatorAndSetFlags(Registers.B);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#CP_A,r8
        /// </summary>
        private void Instruction_0xB9_Compare_C_To_A_And_Set_Flags_As_If_It_Was_Subtracted_From_A()
        {
            CompareToAccumulatorAndSetFlags(Registers.C);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#CP_A,r8
        /// </summary>
        private void Instruction_0xBA_Compare_D_To_A_And_Set_Flags_As_If_It_Was_Subtracted_From_A()
        {
            CompareToAccumulatorAndSetFlags(Registers.D);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#CP_A,r8
        /// </summary>
        private void Instruction_0xBB_Compare_E_To_A_And_Set_Flags_As_If_It_Was_Subtracted_From_A()
        {
            CompareToAccumulatorAndSetFlags(Registers.E);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#CP_A,r8
        /// </summary>
        private void Instruction_0xBC_Compare_H_To_A_And_Set_Flags_As_If_It_Was_Subtracted_From_A()
        {
            CompareToAccumulatorAndSetFlags(Registers.H);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#CP_A,r8
        /// </summary>
        private void Instruction_0xBD_Compare_L_To_A_And_Set_Flags_As_If_It_Was_Subtracted_From_A()
        {
            CompareToAccumulatorAndSetFlags(Registers.L);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#CP_A,r8
        /// </summary>
        private void Instruction_0xBE_Compare_Address_Pointed_To_By_HL_To_A_And_Set_Flags_As_If_It_Was_Subtracted_From_A()
        {
            CompareToAccumulatorAndSetFlags(Memory[Registers.HL]);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#CP_A,r8
        /// </summary>
        private void Instruction_0xBF_Compare_A_To_A_And_Set_Flags_As_If_It_Was_Subtracted_From_A()
        {
            CompareToAccumulatorAndSetFlags(Registers.A);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#POP_r16
        /// </summary>
        private void Instruction_0xC1_Pop_Stack_Into_BC()
        {
            Registers.BC = PopStack();
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#PUSH_r16
        /// </summary>
        private void Instruction_0xC5_Push_BC_Onto_Stack()
        {
            PushOntoStack(Registers.B, Registers.C);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#ADD_A,n8
        /// </summary>
        private void Instruction_0xC6_Add_8_Bit_Immediate_To_A()
        {
            AddToAccumulatorAndSetFlags(Fetch());
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#ADC_A,n8
        /// </summary>
        private void Instruction_0xCE_Add_8_Bit_Immediate_Plus_Carry_To_A()
        {
            AddToAccumulatorAndSetFlags(Fetch(), carryBit: Registers.HasFlag(Flags.Carry));
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#POP_r16
        /// </summary>
        private void Instruction_0xD1_Pop_Stack_Into_DE()
        {
            Registers.DE = PopStack();
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#PUSH_r16
        /// </summary>
        private void Instruction_0xD5_Push_DE_Onto_Stack()
        {
            PushOntoStack(Registers.D, Registers.E);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SUB_A,n8
        /// </summary>
        private void Instruction_0xD6_Subtract_8_Bit_Immediate_From_A()
        {
            SubtractFromAccumulatorAndSetFlags(Fetch());
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SBC_A,n8
        /// </summary>
        private void Instruction_0xDE_Subtract_8_Bit_Immediate_Plus_Carry_From_A()
        {
            SubtractFromAccumulatorAndSetFlags(Fetch(), carryBit: Registers.HasFlag(Flags.Carry));
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD__$FF00+n8_,A
        /// </summary>
        private void Instruction_0xE0_Load_A_Into_High_Memory_Address_Offset_By_Unsigned_8_Bit_Immediate()
        {
            Memory[0xFF00 + Fetch()] = Registers.A;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#POP_r16
        /// </summary>
        private void Instruction_0xE1_Pop_Stack_Into_HL()
        {
            Registers.HL = PopStack();
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD__$FF00+C_,A
        /// </summary>
        private void Instruction_0xE2_Load_A_Into_High_Memory_Address_Offset_By_C()
        {
            Memory[0xFF00 + Registers.C] = Registers.A;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#PUSH_r16
        /// </summary>
        private void Instruction_0xE5_Push_HL_Onto_Stack()
        {
            PushOntoStack(Registers.H, Registers.L);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#AND_A,n8
        /// </summary>
        private void Instruction_0xE6_Bitwise_And_8_Bit_Immediate_With_A()
        {
            AndWithAccumulatorAndSetFlags(Fetch());
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD__n16_,A
        /// </summary>
        private void Instruction_0xEA_Load_Immediate_Memory_Location_From_A()
        {
            var address = Common.FromLittleEndian(Fetch(), Fetch());
            Memory[address] = Registers.A;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#XOR_A,n8
        /// </summary>
        private void Instruction_0xEE_Bitwise_Exclusive_Or_8_Bit_Immediate_With_A()
        {
            XorWithAccumulatorAndSetFlags(Fetch());
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_A,_$FF00+n8_
        /// </summary>
        private void Instruction_0xF0_Load_A_From_High_Memory_Address_Offset_By_8_Bit_Immediate()
        {
            Registers.A = Memory[0xFF00 + Fetch()];
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#POP_AF
        /// </summary>
        private void Instruction_0xF1_Pop_Stack_Into_AF()
        {
            Registers.AF = PopStack();
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_A,_$FF00+C_
        /// </summary>
        private void Instruction_0xF2_Load_A_From_High_Memory_Address_Offset_By_C()
        {
            Registers.A = Memory[0xFF00 + Registers.C];
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#PUSH_r16
        /// </summary>
        private void Instruction_0xF5_Push_AF_Onto_Stack()
        {
            PushOntoStack(Registers.A, Registers.F);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_HL,SP+e8
        /// </summary>
        private void Instruction_0xF8_Add_8_Bit_Signed_Immediate_To_Stack_Pointer_And_Store_Result_In_HL()
        {
            var immediate = (sbyte)Fetch();
            Registers.HL = (ushort)(Registers.SP + immediate);
            Registers.ClearFlag(Flags.AddSubtract | Flags.Zero);
            Registers.SetFlagTo(Flags.HalfCarry, ((Registers.SP & 0xF) + (immediate & 0xF) > 0xF));
            Registers.SetFlagTo(Flags.Carry, ((Registers.SP & 0xFF) + immediate > 0xFF));
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_SP,HL
        /// </summary>
        private void Instruction_0xF9_Load_Stack_Pointer_From_HL()
        {
            Registers.SP = Registers.HL;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_A,_n16_
        /// </summary>
        private void Instruction_0xFA_Load_A_From_Immediate_Memory_Location()
        {
            var address = Common.FromLittleEndian(Fetch(), Fetch());
            Registers.A = Memory[address];
        }

        //...

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#ADD_A,r8 (add w/o carry)
        /// https://rednex.github.io/rgbds/gbz80.7.html#ADC_A,r8 (add w/ carry)
        /// </summary>
        private void AddToAccumulatorAndSetFlags(byte value, bool carryBit = false)
        {
            var carry = (byte)(carryBit ? 1 : 0);
            //half carry (carry into upper nibble) occurs if the sum of the lower nibbles is > 1111
            Registers.SetFlagTo(Flags.HalfCarry, (Registers.A & 0xF) + (value & 0xF) + carry > 0xF);
            Registers.SetFlagTo(Flags.Carry, Registers.A + value + carry > byte.MaxValue);
            Registers.A += value;
            Registers.A += carry;
            Registers.SetFlagTo(Flags.Zero, Registers.A == 0);
            Registers.ClearFlag(Flags.AddSubtract);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SUB_A,r8 (subtract w/o carry)
        /// https://rednex.github.io/rgbds/gbz80.7.html#SBC_A,r8 (subtract w/ carry)
        /// https://rednex.github.io/rgbds/gbz80.7.html#CP_A,r8  (compare - subtract but don't store result)
        /// </summary>
        private void SubtractFromAccumulatorAndSetFlags(byte value, bool carryBit = false, bool storeResult = true)
        {
            var carry = (byte)(carryBit ? 1 : 0);
            //half carry (borrow from upper nibble) occurs if the lower nibble of the value being subtracted is > A's
            Registers.SetFlagTo(Flags.HalfCarry, ((value + carry) & 0xF) > (Registers.A & 0xF));
            Registers.SetFlagTo(Flags.Carry, (value + carry) > Registers.A);
            Registers.SetFlag(Flags.AddSubtract);

            if (storeResult)
            {
                Registers.A -= value;
                Registers.A -= carry;
                Registers.SetFlagTo(Flags.Zero, Registers.A == 0);
            }
            else
            {
                var temp = Registers.A;
                temp -= value;
                temp -= carry;
                Registers.SetFlagTo(Flags.Zero, temp == 0);
            }
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#CP_A,r8
        /// </summary>
        /// <param name="value"></param>
        private void CompareToAccumulatorAndSetFlags(byte value)
        {
            SubtractFromAccumulatorAndSetFlags(value, carryBit: false, storeResult: false);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#AND_A,r8
        /// </summary>
        private void AndWithAccumulatorAndSetFlags(byte value)
        {
            Registers.A &= value;
            Registers.SetFlagTo(Flags.Zero, Registers.A == 0);
            Registers.ClearFlag(Flags.AddSubtract | Flags.Carry);
            Registers.SetFlag(Flags.HalfCarry);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#XOR_A,r8
        /// </summary>
        private void XorWithAccumulatorAndSetFlags(byte value)
        {
            Registers.A ^= value;
            Registers.SetFlagTo(Flags.Zero, Registers.A == 0);
            Registers.ClearFlag(Flags.AddSubtract | Flags.HalfCarry | Flags.Carry);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#OR_A,r8
        /// </summary>
        private void OrWithAccumulatorAndSetFlags(byte value)
        {
            Registers.A |= value;
            Registers.SetFlagTo(Flags.Zero, Registers.A == 0);
            Registers.ClearFlag(Flags.AddSubtract | Flags.HalfCarry | Flags.Carry);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#ADD_HL,r16
        /// </summary>
        private void AddToHLAndSetFlags(ushort value)
        {
            Registers.ClearFlag(Flags.AddSubtract);
            Registers.SetFlagTo(Flags.HalfCarry, ((Registers.HL & 0xFFF) + (value & 0xFFF) > 0xFFF));
            Registers.SetFlagTo(Flags.Carry, (Registers.HL + value > 0xFFFF));
            Registers.HL += value;
        }

        /// <see cref="https://rednex.github.io/rgbds/gbz80.7.html#INC_r8"/>
        /// <see cref="https://github.com/TASVideos/BizHawk/blob/6d0973ca7ea3907abdcf482e6ce8f2767ae6f297/BizHawk.Emulation.Cores/CPUs/Z80A/Operations.cs#L467"/>
        private byte Increment8BitRegisterAndSetFlags(byte oldValue)
        {
            var newValue = (byte)(oldValue + 1);

            Registers.SetFlagTo(Flags.Zero, newValue == 0);
            Registers.ClearFlag(Flags.AddSubtract);

            //half carry will occur if the lower nibble of the old value is all ones
            //this means that adding one must cause a carry into the upper nibble
            //https://github.com/rylev/DMG-01/blob/17c08f5103b52cf06b0a4606ece2f71b48567c0b/lib-dmg-01/src/cpu/mod.rs#L1191
            Registers.SetFlagTo(Flags.HalfCarry, (oldValue & 0xF) == 0xF);

            return newValue;
        }

        /// <see cref="https://rednex.github.io/rgbds/gbz80.7.html#DEC_r8"/>
        /// <see cref="https://github.com/TASVideos/BizHawk/blob/6d0973ca7ea3907abdcf482e6ce8f2767ae6f297/BizHawk.Emulation.Cores/CPUs/Z80A/Operations.cs#L491"/>
        private byte Decrement8BitRegisterAndSetFlags(byte oldValue)
        {
            var newValue = (byte)(oldValue - 1);

            Registers.SetFlagTo(Flags.Zero, newValue == 0);
            Registers.SetFlag(Flags.AddSubtract);

            //half carry will occur if the lower nibble of the old value is zero
            //this means that subtracting one must cause a borrow from the upper nibble
            //https://github.com/rylev/DMG-01/blob/17c08f5103b52cf06b0a4606ece2f71b48567c0b/lib-dmg-01/src/cpu/mod.rs#L1208
            Registers.SetFlagTo(Flags.HalfCarry, (oldValue & 0xF) == 0);

            return newValue;
        }

        public override string ToString() => Registers.ToString();
    }
}
