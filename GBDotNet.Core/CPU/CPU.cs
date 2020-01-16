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
        public IMemory Memory { get; private set; }
        public bool IsHalted { get; private set; }
        public bool InterruptsEnabled { get; private set; }
        public int CyclesLastTick { get; private set; }
        public long TotalElapsedCycles { get; private set; }

        private readonly Action[] instructionSet;
        private readonly Action[] prefixCBInstructions;

        public CPU(Registers registers, IMemory memory)
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
                () => Instruction_0x07_Rotate_A_Left_With_Carry(),
                () => Instruction_0x08_Load_Address_With_Stack_Pointer(),
                () => Instruction_0x09_Add_BC_To_HL(),
                () => Instruction_0x0A_Load_A_From_Address_Pointed_To_By_BC(),
                () => Instruction_0x0B_Decrement_BC(),
                () => Instruction_0x0C_Increment_C(),
                () => Instruction_0x0D_Decrement_C(),
                () => Instruction_0x0E_Load_C_With_8_Bit_Immediate(),
                () => Instruction_0x0F_Rotate_A_Right_With_Carry(),
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
                () => Instruction_0x20_Relative_Jump_By_Signed_Immediate_If_Zero_Flag_Not_Set(),
                () => Instruction_0x21_Load_HL_With_16_Bit_Immediate(),
                () => Instruction_0x22_Load_Address_Pointed_To_By_HL_With_A_Then_Increment_HL(),
                () => Instruction_0x23_Increment_HL(),
                () => Instruction_0x24_Increment_H(),
                () => Instruction_0x25_Decrement_H(),
                () => Instruction_0x26_Load_H_With_8_Bit_Immediate(),
                () => Instruction_0x27_Decimal_Adjust_A_For_Binary_Coded_Decimal_Arithmetic(),
                () => Instruction_0x28_Relative_Jump_By_Signed_Immediate_If_Zero_Flag_Set(),
                () => Instruction_0x29_Add_HL_To_HL(),
                () => Instruction_0x2A_Load_A_With_Address_Pointed_To_By_HL_Then_Increment_HL(),
                () => Instruction_0x2B_Decrement_HL(),
                () => Instruction_0x2C_Increment_L(),
                () => Instruction_0x2D_Decrement_L(),
                () => Instruction_0x2E_Load_L_With_8_Bit_Immediate(),
                () => Instruction_0x2F_Bitwise_Complement_A(),
                //0x30
                () => Instruction_0x30_Relative_Jump_By_Signed_Immediate_If_Carry_Flag_Not_Set(),
                () => Instruction_0x31_Load_SP_With_16_Bit_Immediate(),
                () => Instruction_0x32_Load_Address_Pointed_To_By_HL_With_A_Then_Decrement_HL(),
                () => Instruction_0x33_Increment_SP(),
                () => Instruction_0x34_Increment_Value_Pointed_To_By_HL(),
                () => Instruction_0x35_Decrement_Value_Pointed_To_By_HL(),
                () => Instruction_0x36_Load_Address_Pointed_To_By_HL_With_8_Bit_Immediate(),
                () => Instruction_0x37_Set_Carry_Flag(),
                () => Instruction_0x38_Relative_Jump_By_Signed_Immediate_If_Carry_Flag_Set(),
                () => Instruction_0x39_Add_SP_To_HL(),
                () => Instruction_0x3A_Load_A_With_Address_Pointed_To_By_HL_Then_Decrement_HL(),
                () => Instruction_0x3B_Decrement_SP(),
                () => Instruction_0x3C_Increment_A(),
                () => Instruction_0x3D_Decrement_A(),
                () => Instruction_0x3E_Load_A_With_8_Bit_Immediate(),
                () => Instruction_0x3F_Complement_Carry_Flag(),
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
                () => Instruction_0x6E_Load_L_From_Address_Pointed_To_By_HL(),
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
                () => Instruction_0xC0_Return_From_Subroutine_If_Zero_Flag_Not_Set(),
                () => Instruction_0xC1_Pop_Stack_Into_BC(),
                () => Instruction_0xC2_Jump_To_Immediate_16_Bit_Address_If_Zero_Flag_Not_Set(),
                () => Instruction_0xC3_Jump_To_Immediate_16_Bit_Address(),
                () => Instruction_0xC4_Call_Subroutine_At_Immediate_16_Bit_Address_If_Zero_Flag_Not_Set(),
                () => Instruction_0xC5_Push_BC_Onto_Stack(),
                () => Instruction_0xC6_Add_8_Bit_Immediate_To_A(),
                () => Instruction_0xC7_Call_Reset_Vector_Zero(),
                () => Instruction_0xC8_Return_From_Subroutine_If_Zero_Flag_Set(),
                () => Instruction_0xC9_Return_From_Subroutine(),
                () => Instruction_0xCA_Jump_To_Immediate_16_Bit_Address_If_Zero_Flag_Set(),
                () => prefixCBInstructions[Fetch()](),
                () => Instruction_0xCC_Call_Subroutine_At_Immediate_16_Bit_Address_If_Zero_Flag_Set(),
                () => Instruction_0xCD_Call_Subroutine_At_Immediate_16_Bit_Address(),
                () => Instruction_0xCE_Add_8_Bit_Immediate_Plus_Carry_To_A(),
                () => Instruction_0xCF_Call_Reset_Vector_Eight(),
                //0xD0
                () => Instruction_0xD0_Return_From_Subroutine_If_Carry_Flag_Not_Set(),
                () => Instruction_0xD1_Pop_Stack_Into_DE(),
                () => Instruction_0xD2_Jump_To_Immediate_16_Bit_Address_If_Carry_Flag_Not_Set(),
                () => { throw new NotImplementedException(); },
                () => Instruction_0xD4_Call_Subroutine_At_Immediate_16_Bit_Address_If_Carry_Flag_Not_Set(),
                () => Instruction_0xD5_Push_DE_Onto_Stack(),
                () => Instruction_0xD6_Subtract_8_Bit_Immediate_From_A(),
                () => Instruction_0xD7_Call_Reset_Vector_Ten(),
                () => Instruction_0xD8_Return_From_Subroutine_If_Carry_Flag_Set(),
                () => Instruction_0xD9_Return_From_Subroutine_And_Enable_Interrupts(),
                () => Instruction_0xDA_Jump_To_Immediate_16_Bit_Address_If_Carry_Flag_Set(),
                () => { throw new NotImplementedException(); },
                () => Instruction_0xDC_Call_Subroutine_At_Immediate_16_Bit_Address_If_Carry_Flag_Set(),
                () => { throw new NotImplementedException(); },
                () => Instruction_0xDE_Subtract_8_Bit_Immediate_Plus_Carry_From_A(),
                () => Instruction_0xDF_Call_Reset_Vector_Eighteen(),
                //0xE0
                () => Instruction_0xE0_Load_A_Into_High_Memory_Address_Offset_By_Unsigned_8_Bit_Immediate(),
                () => Instruction_0xE1_Pop_Stack_Into_HL(),
                () => Instruction_0xE2_Load_A_Into_High_Memory_Address_Offset_By_C(),
                () => { throw new NotImplementedException(); },
                () => { throw new NotImplementedException(); },
                () => Instruction_0xE5_Push_HL_Onto_Stack(),
                () => Instruction_0xE6_Bitwise_And_8_Bit_Immediate_With_A(),
                () => Instruction_0xE7_Call_Reset_Vector_Twenty(),
                () => Instruction_0xE8_Add_8_Bit_Signed_Immediate_To_Stack_Pointer(),
                () => Instruction_0xE9_Jump_To_Address_Pointed_To_By_HL(),
                () => Instruction_0xEA_Load_Immediate_Memory_Location_From_A(),
                () => { throw new NotImplementedException(); },
                () => { throw new NotImplementedException(); },
                () => { throw new NotImplementedException(); },
                () => Instruction_0xEE_Bitwise_Exclusive_Or_8_Bit_Immediate_With_A(),
                () => Instruction_0xEF_Call_Reset_Vector_Twenty_Eight(),
                //0xF0
                () => Instruction_0xF0_Load_A_From_High_Memory_Address_Offset_By_8_Bit_Immediate(),
                () => Instruction_0xF1_Pop_Stack_Into_AF(),
                () => Instruction_0xF2_Load_A_From_High_Memory_Address_Offset_By_C(),
                () => Instruction_0xF3_Disable_Interrupts(),
                () => { throw new NotImplementedException(); },
                () => Instruction_0xF5_Push_AF_Onto_Stack(),
                () => Instruction_0xF6_Bitwise_Or_8_Bit_Immediate_With_A(),
                () => Instruction_0xF7_Call_Reset_Vector_Thirty(),
                () => Instruction_0xF8_Add_8_Bit_Signed_Immediate_To_Stack_Pointer_And_Store_Result_In_HL(),
                () => Instruction_0xF9_Load_Stack_Pointer_From_HL(),
                () => Instruction_0xFA_Load_A_From_Immediate_Memory_Location(),
                () => Instruction_0xFB_Enable_Interrupts(),
                () => { throw new NotImplementedException(); },
                () => { throw new NotImplementedException(); },
                () => Instruction_0xFE_Compare_8_Bit_Immediate_With_A_And_Set_Flags_As_If_It_Was_Subtracted_From_A(),
                () => Instruction_0xFF_Call_Reset_Vector_Thirty_Eight()
            };

            prefixCBInstructions = new Action[]
            {
                //0x00
                () => Instruction_0xCB_0x00_Rotate_B_Left_With_Carry(),
                () => Instruction_0xCB_0x01_Rotate_C_Left_With_Carry(),
                () => Instruction_0xCB_0x02_Rotate_D_Left_With_Carry(),
                () => Instruction_0xCB_0x03_Rotate_E_Left_With_Carry(),
                () => Instruction_0xCB_0x04_Rotate_H_Left_With_Carry(),
                () => Instruction_0xCB_0x05_Rotate_L_Left_With_Carry(),
                () => Instruction_0xCB_0x06_Rotate_Address_Pointed_To_By_HL_Left_With_Carry(),
                () => Instruction_0xCB_0x07_Rotate_A_Left_With_Carry(),
                () => Instruction_0xCB_0x08_Rotate_B_Right_With_Carry(),
                () => Instruction_0xCB_0x09_Rotate_C_Right_With_Carry(),
                () => Instruction_0xCB_0x0A_Rotate_D_Right_With_Carry(),
                () => Instruction_0xCB_0x0B_Rotate_E_Right_With_Carry(),
                () => Instruction_0xCB_0x0C_Rotate_H_Right_With_Carry(),
                () => Instruction_0xCB_0x0D_Rotate_L_Right_With_Carry(),
                () => Instruction_0xCB_0x0E_Rotate_Address_Pointed_To_By_HL_Right_With_Carry(),
                () => Instruction_0xCB_0x0F_Rotate_A_Right_With_Carry(),
                //0x10
                () => Instruction_0xCB_0x10_Rotate_B_Left(),
                () => Instruction_0xCB_0x11_Rotate_C_Left(),
                () => Instruction_0xCB_0x12_Rotate_D_Left(),
                () => Instruction_0xCB_0x13_Rotate_E_Left(),
                () => Instruction_0xCB_0x14_Rotate_H_Left(),
                () => Instruction_0xCB_0x15_Rotate_L_Left(),
                () => Instruction_0xCB_0x16_Rotate_Address_Pointed_To_By_HL_Left(),
                () => Instruction_0xCB_0x17_Rotate_A_Left(),
                () => Instruction_0xCB_0x18_Rotate_B_Right(),
                () => Instruction_0xCB_0x19_Rotate_C_Right(),
                () => Instruction_0xCB_0x1A_Rotate_D_Right(),
                () => Instruction_0xCB_0x1B_Rotate_E_Right(),
                () => Instruction_0xCB_0x1C_Rotate_H_Right(),
                () => Instruction_0xCB_0x1D_Rotate_L_Right(),
                () => Instruction_0xCB_0x1E_Rotate_Address_Pointed_To_By_HL_Right(),
                () => Instruction_0xCB_0x1F_Rotate_A_Right(),
                //0x20
                () => Instruction_0xCB_0x20_Shift_B_Left(),
                () => Instruction_0xCB_0x21_Shift_C_Left(),
                () => Instruction_0xCB_0x22_Shift_D_Left(),
                () => Instruction_0xCB_0x23_Shift_E_Left(),
                () => Instruction_0xCB_0x24_Shift_H_Left(),
                () => Instruction_0xCB_0x25_Shift_L_Left(),
                () => Instruction_0xCB_0x26_Shift_Address_Pointed_To_By_HL_Left(),
                () => Instruction_0xCB_0x27_Shift_A_Left(),
                () => Instruction_0xCB_0x28_Should_Arithmetic_Shift_B_Right(),
                () => Instruction_0xCB_0x29_Should_Arithmetic_Shift_C_Right(),
                () => Instruction_0xCB_0x2A_Should_Arithmetic_Shift_D_Right(),
                () => Instruction_0xCB_0x2B_Should_Arithmetic_Shift_E_Right(),
                () => Instruction_0xCB_0x2C_Should_Arithmetic_Shift_H_Right(),
                () => Instruction_0xCB_0x2D_Should_Arithmetic_Shift_L_Right(),
                () => Instruction_0xCB_0x2E_Should_Arithmetic_Shift_Address_Pointed_To_By_HL_Right(),
                () => Instruction_0xCB_0x2F_Should_Arithmetic_Shift_A_Right(),
                //0x30
                () => Instruction_0xCB_0x30_Swap_Nibbles_In_B(),
                () => Instruction_0xCB_0x31_Swap_Nibbles_In_C(),
                () => Instruction_0xCB_0x32_Swap_Nibbles_In_D(),
                () => Instruction_0xCB_0x33_Swap_Nibbles_In_E(),
                () => Instruction_0xCB_0x34_Swap_Nibbles_In_H(),
                () => Instruction_0xCB_0x35_Swap_Nibbles_In_L(),
                () => Instruction_0xCB_0x36_Swap_Nibbles_In_Address_Pointed_To_By_HL(),
                () => Instruction_0xCB_0x36_Swap_Nibbles_In_A(),
                () => Instruction_0xCB_0x38_Shift_B_Right(),
                () => Instruction_0xCB_0x39_Shift_C_Right(),
                () => Instruction_0xCB_0x3A_Shift_D_Right(),
                () => Instruction_0xCB_0x3B_Shift_E_Right(),
                () => Instruction_0xCB_0x3C_Shift_H_Right(),
                () => Instruction_0xCB_0x3D_Shift_L_Right(),
                () => Instruction_0xCB_0x3E_Shift_Address_Pointed_To_By_HL_Right(),
                () => Instruction_0xCB_0x3F_Shift_A_Right(),
                //0x40
                () => Instruction_0xCB_0x40_Test_Bit_0_Of_B_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x41_Test_Bit_0_Of_C_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x42_Test_Bit_0_Of_D_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x43_Test_Bit_0_Of_E_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x44_Test_Bit_0_Of_H_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x45_Test_Bit_0_Of_L_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x46_Test_Bit_0_Of_Address_Pointed_To_By_HL_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x47_Test_Bit_0_Of_A_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x48_Test_Bit_1_Of_B_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x49_Test_Bit_1_Of_C_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x4A_Test_Bit_1_Of_D_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x4B_Test_Bit_1_Of_E_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x4C_Test_Bit_1_Of_H_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x4D_Test_Bit_1_Of_L_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x4E_Test_Bit_1_Of_Address_Pointed_To_By_HL_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x4F_Test_Bit_1_Of_A_And_Set_Zero_Flag_If_It_Was_Zero(),
                //0x50
                () => Instruction_0xCB_0x50_Test_Bit_2_Of_B_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x51_Test_Bit_2_Of_C_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x52_Test_Bit_2_Of_D_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x53_Test_Bit_2_Of_E_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x54_Test_Bit_2_Of_H_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x55_Test_Bit_2_Of_L_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x56_Test_Bit_2_Of_Address_Pointed_To_By_HL_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x57_Test_Bit_2_Of_A_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x58_Test_Bit_3_Of_B_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x59_Test_Bit_3_Of_C_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x5A_Test_Bit_3_Of_D_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x5B_Test_Bit_3_Of_E_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x5C_Test_Bit_3_Of_H_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x5D_Test_Bit_3_Of_L_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x5E_Test_Bit_3_Of_Address_Pointed_To_By_HL_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x5F_Test_Bit_3_Of_A_And_Set_Zero_Flag_If_It_Was_Zero(),
                //0x60
                () => Instruction_0xCB_0x60_Test_Bit_4_Of_B_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x61_Test_Bit_4_Of_C_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x62_Test_Bit_4_Of_D_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x63_Test_Bit_4_Of_E_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x64_Test_Bit_4_Of_H_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x65_Test_Bit_4_Of_L_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x66_Test_Bit_4_Of_Address_Pointed_To_By_HL_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x67_Test_Bit_4_Of_A_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x68_Test_Bit_5_Of_B_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x69_Test_Bit_5_Of_C_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x6A_Test_Bit_5_Of_D_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x6B_Test_Bit_5_Of_E_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x6C_Test_Bit_5_Of_H_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x6D_Test_Bit_5_Of_L_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x6E_Test_Bit_5_Of_Address_Pointed_To_By_HL_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x6F_Test_Bit_5_Of_A_And_Set_Zero_Flag_If_It_Was_Zero(),
                //0x70
                () => Instruction_0xCB_0x70_Test_Bit_6_Of_B_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x71_Test_Bit_6_Of_C_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x72_Test_Bit_6_Of_D_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x73_Test_Bit_6_Of_E_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x74_Test_Bit_6_Of_H_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x75_Test_Bit_6_Of_L_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x76_Test_Bit_6_Of_Address_Pointed_To_By_HL_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x77_Test_Bit_6_Of_A_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x78_Test_Bit_7_Of_B_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x79_Test_Bit_7_Of_C_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x7A_Test_Bit_7_Of_D_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x7B_Test_Bit_7_Of_E_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x7C_Test_Bit_7_Of_H_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x7D_Test_Bit_7_Of_L_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x7E_Test_Bit_7_Of_Address_Pointed_To_By_HL_And_Set_Zero_Flag_If_It_Was_Zero(),
                () => Instruction_0xCB_0x7F_Test_Bit_7_Of_A_And_Set_Zero_Flag_If_It_Was_Zero(),
                //0x80
                () => Instruction_0xCB_0x80_Reset_Bit_0_Of_B(),
                () => Instruction_0xCB_0x81_Reset_Bit_0_Of_C(),
                () => Instruction_0xCB_0x82_Reset_Bit_0_Of_D(),
                () => Instruction_0xCB_0x83_Reset_Bit_0_Of_E(),
                () => Instruction_0xCB_0x84_Reset_Bit_0_Of_H(),
                () => Instruction_0xCB_0x85_Reset_Bit_0_Of_L(),
                () => Instruction_0xCB_0x86_Reset_Bit_0_Of_Address_Pointed_To_By_HL(),
                () => Instruction_0xCB_0x87_Reset_Bit_0_Of_A(),
                () => Instruction_0xCB_0x88_Reset_Bit_1_Of_B(),
                () => Instruction_0xCB_0x89_Reset_Bit_1_Of_C(),
                () => Instruction_0xCB_0x8A_Reset_Bit_1_Of_D(),
                () => Instruction_0xCB_0x8B_Reset_Bit_1_Of_E(),
                () => Instruction_0xCB_0x8C_Reset_Bit_1_Of_H(),
                () => Instruction_0xCB_0x8D_Reset_Bit_1_Of_L(),
                () => Instruction_0xCB_0x8E_Reset_Bit_1_Of_Address_Pointed_To_By_HL(),
                () => Instruction_0xCB_0x8F_Reset_Bit_1_Of_A(),
                //0x90
                () => Instruction_0xCB_0x90_Reset_Bit_2_Of_B(),
                () => Instruction_0xCB_0x91_Reset_Bit_2_Of_C(),
                () => Instruction_0xCB_0x92_Reset_Bit_2_Of_D(),
                () => Instruction_0xCB_0x93_Reset_Bit_2_Of_E(),
                () => Instruction_0xCB_0x94_Reset_Bit_2_Of_H(),
                () => Instruction_0xCB_0x95_Reset_Bit_2_Of_L(),
                () => Instruction_0xCB_0x96_Reset_Bit_2_Of_Address_Pointed_To_By_HL(),
                () => Instruction_0xCB_0x97_Reset_Bit_2_Of_A(),
                () => Instruction_0xCB_0x98_Reset_Bit_3_Of_B(),
                () => Instruction_0xCB_0x99_Reset_Bit_3_Of_C(),
                () => Instruction_0xCB_0x9A_Reset_Bit_3_Of_D(),
                () => Instruction_0xCB_0x9B_Reset_Bit_3_Of_E(),
                () => Instruction_0xCB_0x9C_Reset_Bit_3_Of_H(),
                () => Instruction_0xCB_0x9D_Reset_Bit_3_Of_L(),
                () => Instruction_0xCB_0x9E_Reset_Bit_3_Of_Address_Pointed_To_By_HL(),
                () => Instruction_0xCB_0x9F_Reset_Bit_3_Of_A(),
                //0xA0
                () => Instruction_0xCB_0xA0_Reset_Bit_4_Of_B(),
                () => Instruction_0xCB_0xA1_Reset_Bit_4_Of_C(),
                () => Instruction_0xCB_0xA2_Reset_Bit_4_Of_D(),
                () => Instruction_0xCB_0xA3_Reset_Bit_4_Of_E(),
                () => Instruction_0xCB_0xA4_Reset_Bit_4_Of_H(),
                () => Instruction_0xCB_0xA5_Reset_Bit_4_Of_L(),
                () => Instruction_0xCB_0xA6_Reset_Bit_4_Of_Address_Pointed_To_By_HL(),
                () => Instruction_0xCB_0xA7_Reset_Bit_4_Of_A(),
                () => Instruction_0xCB_0xA8_Reset_Bit_5_Of_B(),
                () => Instruction_0xCB_0xA9_Reset_Bit_5_Of_C(),
                () => Instruction_0xCB_0xAA_Reset_Bit_5_Of_D(),
                () => Instruction_0xCB_0xAB_Reset_Bit_5_Of_E(),
                () => Instruction_0xCB_0xAC_Reset_Bit_5_Of_H(),
                () => Instruction_0xCB_0xAD_Reset_Bit_5_Of_L(),
                () => Instruction_0xCB_0xAE_Reset_Bit_5_Of_Address_Pointed_To_By_HL(),
                () => Instruction_0xCB_0xAF_Reset_Bit_5_Of_A(),
                //0xB0
                () => Instruction_0xCB_0xB0_Reset_Bit_6_Of_B(),
                () => Instruction_0xCB_0xB1_Reset_Bit_6_Of_C(),
                () => Instruction_0xCB_0xB2_Reset_Bit_6_Of_D(),
                () => Instruction_0xCB_0xB3_Reset_Bit_6_Of_E(),
                () => Instruction_0xCB_0xB4_Reset_Bit_6_Of_H(),
                () => Instruction_0xCB_0xB5_Reset_Bit_6_Of_L(),
                () => Instruction_0xCB_0xB6_Reset_Bit_6_Of_Address_Pointed_To_By_HL(),
                () => Instruction_0xCB_0xB7_Reset_Bit_6_Of_A(),
                () => Instruction_0xCB_0xB8_Reset_Bit_7_Of_B(),
                () => Instruction_0xCB_0xB9_Reset_Bit_7_Of_C(),
                () => Instruction_0xCB_0xBA_Reset_Bit_7_Of_D(),
                () => Instruction_0xCB_0xBB_Reset_Bit_7_Of_E(),
                () => Instruction_0xCB_0xBC_Reset_Bit_7_Of_H(),
                () => Instruction_0xCB_0xBD_Reset_Bit_7_Of_L(),
                () => Instruction_0xCB_0xBE_Reset_Bit_7_Of_Address_Pointed_To_By_HL(),
                () => Instruction_0xCB_0xBF_Reset_Bit_7_Of_A(),
                //0xC0
                () => Instruction_0xCB_0xC0_Set_Bit_0_Of_B(),
                () => Instruction_0xCB_0xC1_Set_Bit_0_Of_C(),
                () => Instruction_0xCB_0xC2_Set_Bit_0_Of_D(),
                () => Instruction_0xCB_0xC3_Set_Bit_0_Of_E(),
                () => Instruction_0xCB_0xC4_Set_Bit_0_Of_H(),
                () => Instruction_0xCB_0xC5_Set_Bit_0_Of_L(),
                () => Instruction_0xCB_0xC6_Set_Bit_0_Of_Address_Pointed_To_By_HL(),
                () => Instruction_0xCB_0xC7_Set_Bit_0_Of_A(),
                () => Instruction_0xCB_0xC8_Set_Bit_1_Of_B(),
                () => Instruction_0xCB_0xC9_Set_Bit_1_Of_C(),
                () => Instruction_0xCB_0xCA_Set_Bit_1_Of_D(),
                () => Instruction_0xCB_0xCB_Set_Bit_1_Of_E(),
                () => Instruction_0xCB_0xCC_Set_Bit_1_Of_H(),
                () => Instruction_0xCB_0xCD_Set_Bit_1_Of_L(),
                () => Instruction_0xCB_0xCE_Set_Bit_1_Of_Address_Pointed_To_By_HL(),
                () => Instruction_0xCB_0xCF_Set_Bit_1_Of_A(),
                //0xD0
                () => Instruction_0xCB_0xD0_Set_Bit_2_Of_B(),
                () => Instruction_0xCB_0xD1_Set_Bit_2_Of_C(),
                () => Instruction_0xCB_0xD2_Set_Bit_2_Of_D(),
                () => Instruction_0xCB_0xD3_Set_Bit_2_Of_E(),
                () => Instruction_0xCB_0xD4_Set_Bit_2_Of_H(),
                () => Instruction_0xCB_0xD5_Set_Bit_2_Of_L(),
                () => Instruction_0xCB_0xD6_Set_Bit_2_Of_Address_Pointed_To_By_HL(),
                () => Instruction_0xCB_0xD7_Set_Bit_2_Of_A(),
                () => Instruction_0xCB_0xD8_Set_Bit_3_Of_B(),
                () => Instruction_0xCB_0xD9_Set_Bit_3_Of_C(),
                () => Instruction_0xCB_0xDA_Set_Bit_3_Of_D(),
                () => Instruction_0xCB_0xDB_Set_Bit_3_Of_E(),
                () => Instruction_0xCB_0xDC_Set_Bit_3_Of_H(),
                () => Instruction_0xCB_0xDD_Set_Bit_3_Of_L(),
                () => Instruction_0xCB_0xDE_Set_Bit_3_Of_Address_Pointed_To_By_HL(),
                () => Instruction_0xCB_0xDF_Set_Bit_3_Of_A(),
                //0xE0
                () => Instruction_0xCB_0xE0_Set_Bit_4_Of_B(),
                () => Instruction_0xCB_0xE1_Set_Bit_4_Of_C(),
                () => Instruction_0xCB_0xE2_Set_Bit_4_Of_D(),
                () => Instruction_0xCB_0xE3_Set_Bit_4_Of_E(),
                () => Instruction_0xCB_0xE4_Set_Bit_4_Of_H(),
                () => Instruction_0xCB_0xE5_Set_Bit_4_Of_L(),
                () => Instruction_0xCB_0xE6_Set_Bit_4_Of_Address_Pointed_To_By_HL(),
                () => Instruction_0xCB_0xE7_Set_Bit_4_Of_A(),
                () => Instruction_0xCB_0xE8_Set_Bit_5_Of_B(),
                () => Instruction_0xCB_0xE9_Set_Bit_5_Of_C(),
                () => Instruction_0xCB_0xEA_Set_Bit_5_Of_D(),
                () => Instruction_0xCB_0xEB_Set_Bit_5_Of_E(),
                () => Instruction_0xCB_0xEC_Set_Bit_5_Of_H(),
                () => Instruction_0xCB_0xED_Set_Bit_5_Of_L(),
                () => Instruction_0xCB_0xEE_Set_Bit_5_Of_Address_Pointed_To_By_HL(),
                () => Instruction_0xCB_0xEF_Set_Bit_5_Of_A(),
                //0xF0
                () => Instruction_0xCB_0xF0_Set_Bit_6_Of_B(),
                () => Instruction_0xCB_0xF1_Set_Bit_6_Of_C(),
                () => Instruction_0xCB_0xF2_Set_Bit_6_Of_D(),
                () => Instruction_0xCB_0xF3_Set_Bit_6_Of_E(),
                () => Instruction_0xCB_0xF4_Set_Bit_6_Of_H(),
                () => Instruction_0xCB_0xF5_Set_Bit_6_Of_L(),
                () => Instruction_0xCB_0xF6_Set_Bit_6_Of_Address_Pointed_To_By_HL(),
                () => Instruction_0xCB_0xF7_Set_Bit_6_Of_A(),
                () => Instruction_0xCB_0xF8_Set_Bit_7_Of_B(),
                () => Instruction_0xCB_0xF9_Set_Bit_7_Of_C(),
                () => Instruction_0xCB_0xFA_Set_Bit_7_Of_D(),
                () => Instruction_0xCB_0xFB_Set_Bit_7_Of_E(),
                () => Instruction_0xCB_0xFC_Set_Bit_7_Of_H(),
                () => Instruction_0xCB_0xFD_Set_Bit_7_Of_L(),
                () => Instruction_0xCB_0xFE_Set_Bit_7_Of_Address_Pointed_To_By_HL(),
                () => Instruction_0xCB_0xFF_Set_Bit_7_Of_A(),
            };
        }

        /// <summary>
        /// Initializes the CPU's internal state to what it would be immediately
        /// after executing the boot ROM.
        /// </summary>
        /// <see cref="https://gbdev.gg8.se/wiki/articles/Gameboy_Bootstrap_ROM"/>
        /// <remarks>
        /// TODO: Maybe change this later to actually execute the boot ROM for that
        /// classic logo scroll and sfx: https://github.com/taylus/gbdotnet/issues/13
        /// </remarks>
        public void Boot()
        {
            Registers.AF = 0x0100;
            Registers.BC = 0x0014;
            Registers.DE = 0x0000;
            Registers.HL = 0xC060;
            Registers.SP = 0xFFFE;
            Registers.PC = 0x0100;
        }

        /// <summary>
        /// Implements a single iteration of the processor's fetch/decode/execute cycle.
        /// </summary>
        public void Tick()
        {
            if (IsHalted) return;
            CyclesLastTick = 0;
            byte opcode = Fetch();
            Execute(opcode);
            TotalElapsedCycles += CyclesLastTick;
        }

        /// <summary>
        /// Retrieves the next instruction from memory and increments the program counter.
        /// </summary>
        private byte Fetch()
        {
            CyclesLastTick += 4;
            return Memory[Registers.PC++];
        }

        /// <summary>
        /// Retrieves the data at the given memory address.
        /// </summary>
        private byte MemoryRead(int address)
        {
            CyclesLastTick += 4;
            return Memory[address];
        }

        /// <summary>
        /// Writes the given byte to the given memory address.
        /// </summary>
        private void MemoryWrite(int address, byte value)
        {
            CyclesLastTick += 4;
            Memory[address] = value;
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
        internal void PushOntoStack(byte high, byte low)
        {
            Registers.SP--;
            Memory[Registers.SP] = high;
            Registers.SP--;
            Memory[Registers.SP] = low;
        }

        /// <summary>
        /// Pushes the given 16-bit value onto the stack and decrements the stack pointer by 2.
        /// </summary>
        internal void PushOntoStack(ushort value)
        {
            PushOntoStack((byte)(value >> 8), (byte)value);
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

        private void RelativeJump(sbyte signedImmediate)
        {
            Registers.PC += (ushort)signedImmediate;
            CyclesLastTick += 4;
        }

        private void AbsoluteJump(ushort address)
        {
            Registers.PC = address;
        }

        private void Call(ushort destinationAddress, ushort returnAddress)
        {
            PushOntoStack(returnAddress);
            AbsoluteJump(destinationAddress);
        }

        private void Return()
        {
            AbsoluteJump(PopStack());
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
            MemoryWrite(address: Registers.BC, value: Registers.A);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#INC_r16
        /// </summary>
        private void Instruction_0x03_Increment_BC()
        {
            CyclesLastTick += 4;
            Registers.BC++;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#INC_r8
        /// </summary>
        private void Instruction_0x04_Increment_B()
        {
            Registers.B = Increment8BitValueAndSetFlags(Registers.B);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#DEC_r8
        /// </summary>
        private void Instruction_0x05_Decrement_B()
        {
            Registers.B = Decrement8BitValueAndSetFlags(Registers.B);
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
        private void Instruction_0x07_Rotate_A_Left_With_Carry()
        {
            Registers.A = RotateLeftWithCarryAndSetFlags(Registers.A, clearZeroFlag: true);
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

            MemoryWrite(address, (byte)(Registers.SP & 0xFF));
            MemoryWrite(address + 1, (byte)(Registers.SP >> 8));
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
            Registers.A = MemoryRead(Registers.BC);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#DEC_r16
        /// </summary>
        private void Instruction_0x0B_Decrement_BC()
        {
            CyclesLastTick += 4;
            Registers.BC--;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#INC_r8
        /// </summary>
        private void Instruction_0x0C_Increment_C()
        {
            Registers.C = Increment8BitValueAndSetFlags(Registers.C);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#DEC_r8
        /// </summary>
        private void Instruction_0x0D_Decrement_C()
        {
            Registers.C = Decrement8BitValueAndSetFlags(Registers.C);
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
        private void Instruction_0x0F_Rotate_A_Right_With_Carry()
        {
            Registers.A = RotateRightWithCarryAndSetFlags(Registers.A, clearZeroFlag: true);
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
            CyclesLastTick += 4;
            Registers.DE++;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#INC_r8
        /// </summary>
        private void Instruction_0x14_Increment_D()
        {
            Registers.D = Increment8BitValueAndSetFlags(Registers.D);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#DEC_r8
        /// </summary>
        private void Instruction_0x15_Decrement_D()
        {
            Registers.D = Decrement8BitValueAndSetFlags(Registers.D);
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
            Registers.A = RotateLeftAndSetFlags(Registers.A, clearZeroFlag: true);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#JR_e8
        /// </summary>
        private void Instruction_0x18_Relative_Jump_By_8_Bit_Signed_Immediate()
        {
            var signedImmediate = (sbyte)Fetch();
            RelativeJump(signedImmediate);
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
            Registers.A = MemoryRead(Registers.DE);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#DEC_r16
        /// </summary>
        private void Instruction_0x1B_Decrement_DE()
        {
            CyclesLastTick += 4;
            Registers.DE--;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#INC_r8
        /// </summary>
        private void Instruction_0x1C_Increment_E()
        {
            Registers.E = Increment8BitValueAndSetFlags(Registers.E);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#DEC_r8
        /// </summary>
        private void Instruction_0x1D_Decrement_E()
        {
            Registers.E = Decrement8BitValueAndSetFlags(Registers.E);
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
            Registers.A = RotateRightAndSetFlags(Registers.A, clearZeroFlag: true);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#JR_cc,e8
        /// </summary>
        private void Instruction_0x20_Relative_Jump_By_Signed_Immediate_If_Zero_Flag_Not_Set()
        {
            var signedImmediate = (sbyte)Fetch();
            if (!Registers.HasFlag(Flags.Zero)) RelativeJump(signedImmediate);
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
            MemoryWrite(Registers.HL, Registers.A);
            Registers.HL++;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#INC_r16
        /// </summary>
        private void Instruction_0x23_Increment_HL()
        {
            CyclesLastTick += 4;
            Registers.HL++;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#INC_r8
        /// </summary>
        private void Instruction_0x24_Increment_H()
        {
            Registers.H = Increment8BitValueAndSetFlags(Registers.H);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#DEC_r8
        /// </summary>
        private void Instruction_0x25_Decrement_H()
        {
            Registers.H = Decrement8BitValueAndSetFlags(Registers.H);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,n8
        /// </summary>
        private void Instruction_0x26_Load_H_With_8_Bit_Immediate()
        {
            Registers.H = Fetch();
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#DAA
        /// https://forums.nesdev.com/viewtopic.php?f=20&t=15944
        /// </summary>
        private void Instruction_0x27_Decimal_Adjust_A_For_Binary_Coded_Decimal_Arithmetic()
        {
            const byte bcdTensDigitAdjust = 0x60;
            const byte bcdOnesDigitAdjust = 0x06;

            if (!Registers.HasFlag(Flags.AddSubtract))
            {
                //after addition, adjust if a carry occurred or result is out of bounds
                if (Registers.HasFlag(Flags.Carry) || Registers.A > 0x99)
                {
                    Registers.A += bcdTensDigitAdjust;
                    Registers.SetFlag(Flags.Carry);
                }
                if (Registers.HasFlag(Flags.HalfCarry) || (Registers.A & 0x0F) > 0x09)
                {
                    Registers.A += bcdOnesDigitAdjust;
                }
            }
            else
            {
                //after subtraction, adjust if a carry occurred
                if (Registers.HasFlag(Flags.Carry))
                {
                    Registers.A -= bcdTensDigitAdjust;
                }
                if (Registers.HasFlag(Flags.HalfCarry))
                {
                    Registers.A -= bcdOnesDigitAdjust;
                }
            }

            Registers.ClearFlag(Flags.HalfCarry);
            Registers.SetFlagTo(Flags.Zero, Registers.A == 0);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#JR_cc,e8
        /// </summary>
        private void Instruction_0x28_Relative_Jump_By_Signed_Immediate_If_Zero_Flag_Set()
        {
            var signedImmediate = (sbyte)Fetch();
            if (Registers.HasFlag(Flags.Zero)) RelativeJump(signedImmediate);
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
            Registers.A = MemoryRead(Registers.HL);
            Registers.HL++;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#DEC_r16
        /// </summary>
        private void Instruction_0x2B_Decrement_HL()
        {
            CyclesLastTick += 4;
            Registers.HL--;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#INC_r8
        /// </summary>
        private void Instruction_0x2C_Increment_L()
        {
            Registers.L = Increment8BitValueAndSetFlags(Registers.L);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#DEC_r8
        /// </summary>
        private void Instruction_0x2D_Decrement_L()
        {
            Registers.L = Decrement8BitValueAndSetFlags(Registers.L);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,n8
        /// </summary>
        private void Instruction_0x2E_Load_L_With_8_Bit_Immediate()
        {
            Registers.L = Fetch();
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#CPL
        /// </summary>
        private void Instruction_0x2F_Bitwise_Complement_A()
        {
            Registers.A = (byte)~Registers.A;
            Registers.SetFlag(Flags.AddSubtract | Flags.HalfCarry);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#JR_cc,e8
        /// </summary>
        private void Instruction_0x30_Relative_Jump_By_Signed_Immediate_If_Carry_Flag_Not_Set()
        {
            var signedImmediate = (sbyte)Fetch();
            if (!Registers.HasFlag(Flags.Carry)) RelativeJump(signedImmediate);
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
            MemoryWrite(Registers.HL, Registers.A);
            Registers.HL--;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#INC_SP
        /// </summary>
        private void Instruction_0x33_Increment_SP()
        {
            CyclesLastTick += 4;
            Registers.SP++;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#INC__HL_
        /// </summary>
        private void Instruction_0x34_Increment_Value_Pointed_To_By_HL()
        {
            MemoryWrite(Registers.HL, Increment8BitValueAndSetFlags(MemoryRead(Registers.HL)));
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#DEC__HL_
        /// </summary>
        private void Instruction_0x35_Decrement_Value_Pointed_To_By_HL()
        {
            MemoryWrite(Registers.HL, Decrement8BitValueAndSetFlags(MemoryRead(Registers.HL)));
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD__HL_,n8
        /// </summary>
        private void Instruction_0x36_Load_Address_Pointed_To_By_HL_With_8_Bit_Immediate()
        {
            MemoryWrite(Registers.HL, Fetch());
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SCF
        /// </summary>
        private void Instruction_0x37_Set_Carry_Flag()
        {
            Registers.SetFlag(Flags.Carry);
            Registers.ClearFlag(Flags.AddSubtract | Flags.HalfCarry);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#JR_cc,e8
        /// </summary>
        private void Instruction_0x38_Relative_Jump_By_Signed_Immediate_If_Carry_Flag_Set()
        {
            var signedImmediate = (sbyte)Fetch();
            if (Registers.HasFlag(Flags.Carry)) RelativeJump(signedImmediate);
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
            Registers.A = MemoryRead(Registers.HL);
            Registers.HL--;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#DEC_SP
        /// </summary>
        private void Instruction_0x3B_Decrement_SP()
        {
            CyclesLastTick += 4;
            Registers.SP--;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#INC_r8
        /// </summary>
        private void Instruction_0x3C_Increment_A()
        {
            Registers.A = Increment8BitValueAndSetFlags(Registers.A);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#DEC_r8
        /// </summary>
        private void Instruction_0x3D_Decrement_A()
        {
            Registers.A = Decrement8BitValueAndSetFlags(Registers.A);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD_r8,n8
        /// </summary>
        private void Instruction_0x3E_Load_A_With_8_Bit_Immediate()
        {
            Registers.A = Fetch();
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#CCF
        /// </summary>
        private void Instruction_0x3F_Complement_Carry_Flag()
        {
            Registers.SetFlagTo(Flags.Carry, !Registers.HasFlag(Flags.Carry));
            Registers.ClearFlag(Flags.AddSubtract | Flags.HalfCarry);
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
            Registers.B = MemoryRead(Registers.HL);
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
            Registers.C = MemoryRead(Registers.HL);
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
            Registers.D = MemoryRead(Registers.HL);
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
            Registers.E = MemoryRead(Registers.HL);
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
            Registers.H = MemoryRead(Registers.HL);
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
        private void Instruction_0x6E_Load_L_From_Address_Pointed_To_By_HL()
        {
            Registers.L = MemoryRead(Registers.HL);
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
            MemoryWrite(Registers.HL, Registers.B);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD__HL_,r8
        /// </summary>
        private void Instruction_0x71_Load_Address_Pointed_To_By_HL_With_C()
        {
            MemoryWrite(Registers.HL, Registers.C);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD__HL_,r8
        /// </summary>
        private void Instruction_0x72_Load_Address_Pointed_To_By_HL_With_D()
        {
            MemoryWrite(Registers.HL, Registers.D);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD__HL_,r8
        /// </summary>
        private void Instruction_0x73_Load_Address_Pointed_To_By_HL_With_E()
        {
            MemoryWrite(Registers.HL, Registers.E);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD__HL_,r8
        /// </summary>
        private void Instruction_0x74_Load_Address_Pointed_To_By_HL_With_H()
        {
            MemoryWrite(Registers.HL, Registers.H);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD__HL_,r8
        /// </summary>
        private void Instruction_0x75_Load_Address_Pointed_To_By_HL_With_L()
        {
            MemoryWrite(Registers.HL, Registers.L);
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
            MemoryWrite(Registers.HL, Registers.A);
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
            Registers.A = MemoryRead(Registers.HL);
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
        /// https://rednex.github.io/rgbds/gbz80.7.html#RET_cc
        /// </summary>
        private void Instruction_0xC0_Return_From_Subroutine_If_Zero_Flag_Not_Set()
        {
            if (!Registers.HasFlag(Flags.Zero)) Return();
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#POP_r16
        /// </summary>
        private void Instruction_0xC1_Pop_Stack_Into_BC()
        {
            Registers.BC = PopStack();
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#JP_cc,n16
        /// </summary>
        private void Instruction_0xC2_Jump_To_Immediate_16_Bit_Address_If_Zero_Flag_Not_Set()
        {
            ushort address = Common.FromLittleEndian(Fetch(), Fetch());
            if (!Registers.HasFlag(Flags.Zero)) AbsoluteJump(address);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#JP_n16
        /// </summary>
        private void Instruction_0xC3_Jump_To_Immediate_16_Bit_Address()
        {
            AbsoluteJump(Common.FromLittleEndian(Fetch(), Fetch()));
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#CALL_cc,n16
        /// </summary>
        private void Instruction_0xC4_Call_Subroutine_At_Immediate_16_Bit_Address_If_Zero_Flag_Not_Set()
        {
            ushort address = Common.FromLittleEndian(Fetch(), Fetch());
            if (!Registers.HasFlag(Flags.Zero)) Call(address, returnAddress: Registers.PC);
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
        /// https://rednex.github.io/rgbds/gbz80.7.html#RST_vec
        /// </summary>
        private void Instruction_0xC7_Call_Reset_Vector_Zero()
        {
            Call(0x0000, returnAddress: Registers.PC);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RET_cc
        /// </summary>
        private void Instruction_0xC8_Return_From_Subroutine_If_Zero_Flag_Set()
        {
            if (Registers.HasFlag(Flags.Zero)) Return();
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RET
        /// </summary>
        private void Instruction_0xC9_Return_From_Subroutine()
        {
            Return();
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#JP_cc,n16
        /// </summary>
        private void Instruction_0xCA_Jump_To_Immediate_16_Bit_Address_If_Zero_Flag_Set()
        {
            ushort address = Common.FromLittleEndian(Fetch(), Fetch());
            if (Registers.HasFlag(Flags.Zero)) AbsoluteJump(address);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#CALL_cc,n16
        /// </summary>
        private void Instruction_0xCC_Call_Subroutine_At_Immediate_16_Bit_Address_If_Zero_Flag_Set()
        {
            ushort address = Common.FromLittleEndian(Fetch(), Fetch());
            if (Registers.HasFlag(Flags.Zero)) Call(address, returnAddress: Registers.PC);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#CALL_n16
        /// </summary>
        private void Instruction_0xCD_Call_Subroutine_At_Immediate_16_Bit_Address()
        {
            ushort address = Common.FromLittleEndian(Fetch(), Fetch());
            Call(address, returnAddress: Registers.PC);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#ADC_A,n8
        /// </summary>
        private void Instruction_0xCE_Add_8_Bit_Immediate_Plus_Carry_To_A()
        {
            AddToAccumulatorAndSetFlags(Fetch(), carryBit: Registers.HasFlag(Flags.Carry));
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RST_vec
        /// </summary>
        private void Instruction_0xCF_Call_Reset_Vector_Eight()
        {
            Call(0x0008, returnAddress: Registers.PC);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RET_cc
        /// </summary>
        private void Instruction_0xD0_Return_From_Subroutine_If_Carry_Flag_Not_Set()
        {
            if (!Registers.HasFlag(Flags.Carry)) Return();
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#POP_r16
        /// </summary>
        private void Instruction_0xD1_Pop_Stack_Into_DE()
        {
            Registers.DE = PopStack();
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#JP_cc,n16
        /// </summary>
        private void Instruction_0xD2_Jump_To_Immediate_16_Bit_Address_If_Carry_Flag_Not_Set()
        {
            ushort address = Common.FromLittleEndian(Fetch(), Fetch());
            if (!Registers.HasFlag(Flags.Carry)) AbsoluteJump(address);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#CALL_cc,n16
        /// </summary>
        private void Instruction_0xD4_Call_Subroutine_At_Immediate_16_Bit_Address_If_Carry_Flag_Not_Set()
        {
            ushort address = Common.FromLittleEndian(Fetch(), Fetch());
            if (!Registers.HasFlag(Flags.Carry)) Call(address, returnAddress: Registers.PC);
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
        /// https://rednex.github.io/rgbds/gbz80.7.html#RST_vec
        /// </summary>
        private void Instruction_0xD7_Call_Reset_Vector_Ten()
        {
            Call(0x0010, returnAddress: Registers.PC);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RET_cc
        /// </summary>
        private void Instruction_0xD8_Return_From_Subroutine_If_Carry_Flag_Set()
        {
            if (Registers.HasFlag(Flags.Carry)) Return();
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RETI
        /// </summary>
        private void Instruction_0xD9_Return_From_Subroutine_And_Enable_Interrupts()
        {
            Return();
            InterruptsEnabled = true;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#JP_cc,n16
        /// </summary>
        private void Instruction_0xDA_Jump_To_Immediate_16_Bit_Address_If_Carry_Flag_Set()
        {
            ushort address = Common.FromLittleEndian(Fetch(), Fetch());
            if (Registers.HasFlag(Flags.Carry)) Call(address, returnAddress: Registers.PC);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#CALL_cc,n16
        /// </summary>
        private void Instruction_0xDC_Call_Subroutine_At_Immediate_16_Bit_Address_If_Carry_Flag_Set()
        {
            ushort address = Common.FromLittleEndian(Fetch(), Fetch());
            if (Registers.HasFlag(Flags.Carry)) Call(address, returnAddress: Registers.PC);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SBC_A,n8
        /// </summary>
        private void Instruction_0xDE_Subtract_8_Bit_Immediate_Plus_Carry_From_A()
        {
            SubtractFromAccumulatorAndSetFlags(Fetch(), carryBit: Registers.HasFlag(Flags.Carry));
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RST_vec
        /// </summary>
        private void Instruction_0xDF_Call_Reset_Vector_Eighteen()
        {
            Call(0x0018, returnAddress: Registers.PC);
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
        /// https://rednex.github.io/rgbds/gbz80.7.html#RST_vec
        /// </summary>
        private void Instruction_0xE7_Call_Reset_Vector_Twenty()
        {
            Call(0x0020, returnAddress: Registers.PC);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#ADD_SP,e8
        /// </summary>
        private void Instruction_0xE8_Add_8_Bit_Signed_Immediate_To_Stack_Pointer()
        {
            var immediate = (sbyte)Fetch();
            Registers.SP = (ushort)(Registers.SP + immediate);
            Registers.ClearFlag(Flags.AddSubtract | Flags.Zero);
            Registers.SetFlagTo(Flags.HalfCarry, ((Registers.SP & 0xF) + (immediate & 0xF) > 0xF));
            Registers.SetFlagTo(Flags.Carry, ((Registers.SP & 0xFF) + immediate > 0xFF));
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#JP_HL
        /// </summary>
        private void Instruction_0xE9_Jump_To_Address_Pointed_To_By_HL()
        {
            AbsoluteJump(Registers.HL);
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
        /// https://rednex.github.io/rgbds/gbz80.7.html#RST_vec
        /// </summary>
        private void Instruction_0xEF_Call_Reset_Vector_Twenty_Eight()
        {
            Call(0x0028, returnAddress: Registers.PC);
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
        /// https://rednex.github.io/rgbds/gbz80.7.html#DI
        /// </summary>
        private void Instruction_0xF3_Disable_Interrupts()
        {
            InterruptsEnabled = false;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#PUSH_r16
        /// </summary>
        private void Instruction_0xF5_Push_AF_Onto_Stack()
        {
            PushOntoStack(Registers.A, Registers.F);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#OR_A,n8
        /// </summary>
        private void Instruction_0xF6_Bitwise_Or_8_Bit_Immediate_With_A()
        {
            OrWithAccumulatorAndSetFlags(Fetch());
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RST_vec
        /// </summary>
        private void Instruction_0xF7_Call_Reset_Vector_Thirty()
        {
            Call(0x0030, returnAddress: Registers.PC);
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

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#EI
        /// </summary>
        private void Instruction_0xFB_Enable_Interrupts()
        {
            InterruptsEnabled = true;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#CP_A,n8
        /// </summary>
        private void Instruction_0xFE_Compare_8_Bit_Immediate_With_A_And_Set_Flags_As_If_It_Was_Subtracted_From_A()
        {
            CompareToAccumulatorAndSetFlags(Fetch());
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RST_vec
        /// </summary>
        private void Instruction_0xFF_Call_Reset_Vector_Thirty_Eight()
        {
            Call(0x0038, returnAddress: Registers.PC);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RLC_r8
        /// </summary>
        private void Instruction_0xCB_0x00_Rotate_B_Left_With_Carry()
        {
            Registers.B = RotateLeftWithCarryAndSetFlags(Registers.B);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RLC_r8
        /// </summary>
        private void Instruction_0xCB_0x01_Rotate_C_Left_With_Carry()
        {
            Registers.C = RotateLeftWithCarryAndSetFlags(Registers.C);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RLC_r8
        /// </summary>
        private void Instruction_0xCB_0x02_Rotate_D_Left_With_Carry()
        {
            Registers.D = RotateLeftWithCarryAndSetFlags(Registers.D);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RLC_r8
        /// </summary>
        private void Instruction_0xCB_0x03_Rotate_E_Left_With_Carry()
        {
            Registers.E = RotateLeftWithCarryAndSetFlags(Registers.E);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RLC_r8
        /// </summary>
        private void Instruction_0xCB_0x04_Rotate_H_Left_With_Carry()
        {
            Registers.H = RotateLeftWithCarryAndSetFlags(Registers.H);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RLC_r8
        /// </summary>
        private void Instruction_0xCB_0x05_Rotate_L_Left_With_Carry()
        {
            Registers.L = RotateLeftWithCarryAndSetFlags(Registers.L);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RLC__HL_
        /// </summary>
        private void Instruction_0xCB_0x06_Rotate_Address_Pointed_To_By_HL_Left_With_Carry()
        {
            Memory[Registers.HL] = RotateLeftWithCarryAndSetFlags(Memory[Registers.HL]);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RLC_r8
        /// </summary>
        private void Instruction_0xCB_0x07_Rotate_A_Left_With_Carry()
        {
            Registers.A = RotateLeftWithCarryAndSetFlags(Registers.A);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RRC_r8
        /// </summary>
        private void Instruction_0xCB_0x08_Rotate_B_Right_With_Carry()
        {
            Registers.B = RotateRightWithCarryAndSetFlags(Registers.B);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RRC_r8
        /// </summary>
        private void Instruction_0xCB_0x09_Rotate_C_Right_With_Carry()
        {
            Registers.C = RotateRightWithCarryAndSetFlags(Registers.C);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RRC_r8
        /// </summary>
        private void Instruction_0xCB_0x0A_Rotate_D_Right_With_Carry()
        {
            Registers.D = RotateRightWithCarryAndSetFlags(Registers.D);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RRC_r8
        /// </summary>
        private void Instruction_0xCB_0x0B_Rotate_E_Right_With_Carry()
        {
            Registers.E = RotateRightWithCarryAndSetFlags(Registers.E);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RRC_r8
        /// </summary>
        private void Instruction_0xCB_0x0C_Rotate_H_Right_With_Carry()
        {
            Registers.H = RotateRightWithCarryAndSetFlags(Registers.H);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RRC_r8
        /// </summary>
        private void Instruction_0xCB_0x0D_Rotate_L_Right_With_Carry()
        {
            Registers.L = RotateRightWithCarryAndSetFlags(Registers.L);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RRC__HL_
        /// </summary>
        private void Instruction_0xCB_0x0E_Rotate_Address_Pointed_To_By_HL_Right_With_Carry()
        {
            Memory[Registers.HL] = RotateRightWithCarryAndSetFlags(Memory[Registers.HL]);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RRC_r8
        /// </summary>
        private void Instruction_0xCB_0x0F_Rotate_A_Right_With_Carry()
        {
            Registers.A = RotateRightWithCarryAndSetFlags(Registers.A);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RL_r8
        /// </summary>
        private void Instruction_0xCB_0x10_Rotate_B_Left()
        {
            Registers.B = RotateLeftAndSetFlags(Registers.B);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RL_r8
        /// </summary>
        private void Instruction_0xCB_0x11_Rotate_C_Left()
        {
            Registers.C = RotateLeftAndSetFlags(Registers.C);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RL_r8
        /// </summary>
        private void Instruction_0xCB_0x12_Rotate_D_Left()
        {
            Registers.D = RotateLeftAndSetFlags(Registers.D);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RL_r8
        /// </summary>
        private void Instruction_0xCB_0x13_Rotate_E_Left()
        {
            Registers.E = RotateLeftAndSetFlags(Registers.E);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RL_r8
        /// </summary>
        private void Instruction_0xCB_0x14_Rotate_H_Left()
        {
            Registers.H = RotateLeftAndSetFlags(Registers.H);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RL_r8
        /// </summary>
        private void Instruction_0xCB_0x15_Rotate_L_Left()
        {
            Registers.L = RotateLeftAndSetFlags(Registers.L);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RL__HL_
        /// </summary>
        private void Instruction_0xCB_0x16_Rotate_Address_Pointed_To_By_HL_Left()
        {
            Memory[Registers.HL] = RotateLeftAndSetFlags(Memory[Registers.HL]);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RL_r8
        /// </summary>
        private void Instruction_0xCB_0x17_Rotate_A_Left()
        {
            Registers.A = RotateLeftAndSetFlags(Registers.A);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RR_r8
        /// </summary>
        private void Instruction_0xCB_0x18_Rotate_B_Right()
        {
            Registers.B = RotateRightAndSetFlags(Registers.B);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RR_r8
        /// </summary>
        private void Instruction_0xCB_0x19_Rotate_C_Right()
        {
            Registers.C = RotateRightAndSetFlags(Registers.C);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RR_r8
        /// </summary>
        private void Instruction_0xCB_0x1A_Rotate_D_Right()
        {
            Registers.D = RotateRightAndSetFlags(Registers.D);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RR_r8
        /// </summary>
        private void Instruction_0xCB_0x1B_Rotate_E_Right()
        {
            Registers.E = RotateRightAndSetFlags(Registers.E);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RR_r8
        /// </summary>
        private void Instruction_0xCB_0x1C_Rotate_H_Right()
        {
            Registers.H = RotateRightAndSetFlags(Registers.H);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RR_r8
        /// </summary>
        private void Instruction_0xCB_0x1D_Rotate_L_Right()
        {
            Registers.L = RotateRightAndSetFlags(Registers.L);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RR__HL_
        /// </summary>
        private void Instruction_0xCB_0x1E_Rotate_Address_Pointed_To_By_HL_Right()
        {
            Memory[Registers.HL] = RotateRightAndSetFlags(Memory[Registers.HL]);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RR_r8
        /// </summary>
        private void Instruction_0xCB_0x1F_Rotate_A_Right()
        {
            Registers.A = RotateRightAndSetFlags(Registers.A);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SLA_r8
        /// </summary>
        private void Instruction_0xCB_0x20_Shift_B_Left()
        {
            Registers.B = ShiftLeftAndSetFlags(Registers.B);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SLA_r8
        /// </summary>
        private void Instruction_0xCB_0x21_Shift_C_Left()
        {
            Registers.C = ShiftLeftAndSetFlags(Registers.C);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SLA_r8
        /// </summary>
        private void Instruction_0xCB_0x22_Shift_D_Left()
        {
            Registers.D = ShiftLeftAndSetFlags(Registers.D);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SLA_r8
        /// </summary>
        private void Instruction_0xCB_0x23_Shift_E_Left()
        {
            Registers.E = ShiftLeftAndSetFlags(Registers.E);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SLA_r8
        /// </summary>
        private void Instruction_0xCB_0x24_Shift_H_Left()
        {
            Registers.H = ShiftLeftAndSetFlags(Registers.H);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SLA_r8
        /// </summary>
        private void Instruction_0xCB_0x25_Shift_L_Left()
        {
            Registers.L = ShiftLeftAndSetFlags(Registers.L);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SLA__HL_
        /// </summary>
        private void Instruction_0xCB_0x26_Shift_Address_Pointed_To_By_HL_Left()
        {
            Memory[Registers.HL] = ShiftLeftAndSetFlags(Memory[Registers.HL]);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SLA_r8
        /// </summary>
        private void Instruction_0xCB_0x27_Shift_A_Left()
        {
            Registers.A = ShiftLeftAndSetFlags(Registers.A);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SRA_r8
        /// </summary>
        private void Instruction_0xCB_0x28_Should_Arithmetic_Shift_B_Right()
        {
            Registers.B = ArithmeticShiftRightAndSetFlags(Registers.B);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SRA_r8
        /// </summary>
        private void Instruction_0xCB_0x29_Should_Arithmetic_Shift_C_Right()
        {
            Registers.C = ArithmeticShiftRightAndSetFlags(Registers.C);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SRA_r8
        /// </summary>
        private void Instruction_0xCB_0x2A_Should_Arithmetic_Shift_D_Right()
        {
            Registers.D = ArithmeticShiftRightAndSetFlags(Registers.D);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SRA_r8
        /// </summary>
        private void Instruction_0xCB_0x2B_Should_Arithmetic_Shift_E_Right()
        {
            Registers.E = ArithmeticShiftRightAndSetFlags(Registers.E);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SRA_r8
        /// </summary>
        private void Instruction_0xCB_0x2C_Should_Arithmetic_Shift_H_Right()
        {
            Registers.H = ArithmeticShiftRightAndSetFlags(Registers.H);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SRA_r8
        /// </summary>
        private void Instruction_0xCB_0x2D_Should_Arithmetic_Shift_L_Right()
        {
            Registers.L = ArithmeticShiftRightAndSetFlags(Registers.L);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SRA__HL_
        /// </summary>
        private void Instruction_0xCB_0x2E_Should_Arithmetic_Shift_Address_Pointed_To_By_HL_Right()
        {
            Memory[Registers.HL] = ArithmeticShiftRightAndSetFlags(Memory[Registers.HL]);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SRA_r8
        /// </summary>
        private void Instruction_0xCB_0x2F_Should_Arithmetic_Shift_A_Right()
        {
            Registers.A = ArithmeticShiftRightAndSetFlags(Registers.A);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SWAP_r8
        /// </summary>
        private void Instruction_0xCB_0x30_Swap_Nibbles_In_B()
        {
            Registers.B = SwapNibblesAndSetFlags(Registers.B);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SWAP_r8
        /// </summary>
        private void Instruction_0xCB_0x31_Swap_Nibbles_In_C()
        {
            Registers.C = SwapNibblesAndSetFlags(Registers.C);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SWAP_r8
        /// </summary>
        private void Instruction_0xCB_0x32_Swap_Nibbles_In_D()
        {
            Registers.D = SwapNibblesAndSetFlags(Registers.D);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SWAP_r8
        /// </summary>
        private void Instruction_0xCB_0x33_Swap_Nibbles_In_E()
        {
            Registers.E = SwapNibblesAndSetFlags(Registers.E);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SWAP_r8
        /// </summary>
        private void Instruction_0xCB_0x34_Swap_Nibbles_In_H()
        {
            Registers.H = SwapNibblesAndSetFlags(Registers.H);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SWAP_r8
        /// </summary>
        private void Instruction_0xCB_0x35_Swap_Nibbles_In_L()
        {
            Registers.L = SwapNibblesAndSetFlags(Registers.L);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SWAP__HL_
        /// </summary>
        private void Instruction_0xCB_0x36_Swap_Nibbles_In_Address_Pointed_To_By_HL()
        {
            Memory[Registers.HL] = SwapNibblesAndSetFlags(Memory[Registers.HL]);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SWAP_r8
        /// </summary>
        private void Instruction_0xCB_0x36_Swap_Nibbles_In_A()
        {
            Registers.A = SwapNibblesAndSetFlags(Registers.A);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SRL_r8
        /// </summary>
        private void Instruction_0xCB_0x38_Shift_B_Right()
        {
            Registers.B = ShiftRightAndSetFlags(Registers.B);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SRL_r8
        /// </summary>
        private void Instruction_0xCB_0x39_Shift_C_Right()
        {
            Registers.C = ShiftRightAndSetFlags(Registers.C);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SRL_r8
        /// </summary>
        private void Instruction_0xCB_0x3A_Shift_D_Right()
        {
            Registers.D = ShiftRightAndSetFlags(Registers.D);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SRL_r8
        /// </summary>
        private void Instruction_0xCB_0x3B_Shift_E_Right()
        {
            Registers.E = ShiftRightAndSetFlags(Registers.E);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SRL_r8
        /// </summary>
        private void Instruction_0xCB_0x3C_Shift_H_Right()
        {
            Registers.H = ShiftRightAndSetFlags(Registers.H);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SRL_r8
        /// </summary>
        private void Instruction_0xCB_0x3D_Shift_L_Right()
        {
            Registers.L = ShiftRightAndSetFlags(Registers.L);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SRL__HL_
        /// </summary>
        private void Instruction_0xCB_0x3E_Shift_Address_Pointed_To_By_HL_Right()
        {
            Memory[Registers.HL] = ShiftRightAndSetFlags(Memory[Registers.HL]);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SRL_r8
        /// </summary>
        private void Instruction_0xCB_0x3F_Shift_A_Right()
        {
            Registers.A = ShiftRightAndSetFlags(Registers.A);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x40_Test_Bit_0_Of_B_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.B, bitToTest: 0);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x41_Test_Bit_0_Of_C_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.C, bitToTest: 0);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x42_Test_Bit_0_Of_D_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.D, bitToTest: 0);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x43_Test_Bit_0_Of_E_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.E, bitToTest: 0);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x44_Test_Bit_0_Of_H_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.H, bitToTest: 0);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x45_Test_Bit_0_Of_L_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.L, bitToTest: 0);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,_HL_
        /// </summary>
        private void Instruction_0xCB_0x46_Test_Bit_0_Of_Address_Pointed_To_By_HL_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Memory[Registers.HL], bitToTest: 0);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x47_Test_Bit_0_Of_A_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.A, bitToTest: 0);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x48_Test_Bit_1_Of_B_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.B, bitToTest: 1);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x49_Test_Bit_1_Of_C_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.C, bitToTest: 1);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x4A_Test_Bit_1_Of_D_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.D, bitToTest: 1);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x4B_Test_Bit_1_Of_E_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.E, bitToTest: 1);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x4C_Test_Bit_1_Of_H_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.H, bitToTest: 1);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x4D_Test_Bit_1_Of_L_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.L, bitToTest: 1);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,_HL_
        /// </summary>
        private void Instruction_0xCB_0x4E_Test_Bit_1_Of_Address_Pointed_To_By_HL_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Memory[Registers.HL], bitToTest: 1);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x4F_Test_Bit_1_Of_A_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.A, bitToTest: 1);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x50_Test_Bit_2_Of_B_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.B, bitToTest: 2);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x51_Test_Bit_2_Of_C_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.C, bitToTest: 2);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x52_Test_Bit_2_Of_D_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.D, bitToTest: 2);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x53_Test_Bit_2_Of_E_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.E, bitToTest: 2);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x54_Test_Bit_2_Of_H_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.H, bitToTest: 2);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x55_Test_Bit_2_Of_L_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.L, bitToTest: 2);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,_HL_
        /// </summary>
        private void Instruction_0xCB_0x56_Test_Bit_2_Of_Address_Pointed_To_By_HL_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Memory[Registers.HL], bitToTest: 2);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x57_Test_Bit_2_Of_A_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.A, bitToTest: 2);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x58_Test_Bit_3_Of_B_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.B, bitToTest: 3);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x59_Test_Bit_3_Of_C_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.C, bitToTest: 3);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x5A_Test_Bit_3_Of_D_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.D, bitToTest: 3);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x5B_Test_Bit_3_Of_E_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.E, bitToTest: 3);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x5C_Test_Bit_3_Of_H_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.H, bitToTest: 3);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x5D_Test_Bit_3_Of_L_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.L, bitToTest: 3);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,_HL_
        /// </summary>
        private void Instruction_0xCB_0x5E_Test_Bit_3_Of_Address_Pointed_To_By_HL_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Memory[Registers.HL], bitToTest: 3);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x5F_Test_Bit_3_Of_A_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.A, bitToTest: 3);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x60_Test_Bit_4_Of_B_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.B, bitToTest: 4);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x61_Test_Bit_4_Of_C_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.C, bitToTest: 4);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x62_Test_Bit_4_Of_D_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.D, bitToTest: 4);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x63_Test_Bit_4_Of_E_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.E, bitToTest: 4);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x64_Test_Bit_4_Of_H_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.H, bitToTest: 4);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x65_Test_Bit_4_Of_L_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.L, bitToTest: 4);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,_HL_
        /// </summary>
        private void Instruction_0xCB_0x66_Test_Bit_4_Of_Address_Pointed_To_By_HL_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Memory[Registers.HL], bitToTest: 4);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x67_Test_Bit_4_Of_A_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.A, bitToTest: 4);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x68_Test_Bit_5_Of_B_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.B, bitToTest: 5);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x69_Test_Bit_5_Of_C_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.C, bitToTest: 5);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x6A_Test_Bit_5_Of_D_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.D, bitToTest: 5);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x6B_Test_Bit_5_Of_E_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.E, bitToTest: 5);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x6C_Test_Bit_5_Of_H_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.H, bitToTest: 5);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x6D_Test_Bit_5_Of_L_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.L, bitToTest: 5);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,_HL_
        /// </summary>
        private void Instruction_0xCB_0x6E_Test_Bit_5_Of_Address_Pointed_To_By_HL_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Memory[Registers.HL], bitToTest: 5);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x6F_Test_Bit_5_Of_A_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.A, bitToTest: 5);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x70_Test_Bit_6_Of_B_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.B, bitToTest: 6);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x71_Test_Bit_6_Of_C_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.C, bitToTest: 6);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x72_Test_Bit_6_Of_D_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.D, bitToTest: 6);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x73_Test_Bit_6_Of_E_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.E, bitToTest: 6);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x74_Test_Bit_6_Of_H_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.H, bitToTest: 6);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x75_Test_Bit_6_Of_L_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.L, bitToTest: 6);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,_HL_
        /// </summary>
        private void Instruction_0xCB_0x76_Test_Bit_6_Of_Address_Pointed_To_By_HL_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Memory[Registers.HL], bitToTest: 6);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x77_Test_Bit_6_Of_A_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.A, bitToTest: 6);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x78_Test_Bit_7_Of_B_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.B, bitToTest: 7);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x79_Test_Bit_7_Of_C_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.C, bitToTest: 7);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x7A_Test_Bit_7_Of_D_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.D, bitToTest: 7);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x7B_Test_Bit_7_Of_E_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.E, bitToTest: 7);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x7C_Test_Bit_7_Of_H_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.H, bitToTest: 7);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x7D_Test_Bit_7_Of_L_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.L, bitToTest: 7);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,_HL_
        /// </summary>
        private void Instruction_0xCB_0x7E_Test_Bit_7_Of_Address_Pointed_To_By_HL_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Memory[Registers.HL], bitToTest: 7);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x7F_Test_Bit_7_Of_A_And_Set_Zero_Flag_If_It_Was_Zero()
        {
            TestBitAndSetFlags(Registers.A, bitToTest: 7);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x80_Reset_Bit_0_Of_B()
        {
            Registers.B = ClearBit(Registers.B, 0);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x81_Reset_Bit_0_Of_C()
        {
            Registers.C = ClearBit(Registers.C, 0);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x82_Reset_Bit_0_Of_D()
        {
            Registers.D = ClearBit(Registers.D, 0);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x83_Reset_Bit_0_Of_E()
        {
            Registers.E = ClearBit(Registers.E, 0);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x84_Reset_Bit_0_Of_H()
        {
            Registers.H = ClearBit(Registers.H, 0);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x85_Reset_Bit_0_Of_L()
        {
            Registers.L = ClearBit(Registers.L, 0);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,_HL_
        /// </summary>
        private void Instruction_0xCB_0x86_Reset_Bit_0_Of_Address_Pointed_To_By_HL()
        {
            Memory[Registers.HL] = ClearBit(Memory[Registers.HL], 0);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x87_Reset_Bit_0_Of_A()
        {
            Registers.A = ClearBit(Registers.A, 0);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x88_Reset_Bit_1_Of_B()
        {
            Registers.B = ClearBit(Registers.B, 1);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x89_Reset_Bit_1_Of_C()
        {
            Registers.C = ClearBit(Registers.C, 1);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x8A_Reset_Bit_1_Of_D()
        {
            Registers.D = ClearBit(Registers.D, 1);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x8B_Reset_Bit_1_Of_E()
        {
            Registers.E = ClearBit(Registers.E, 1);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x8C_Reset_Bit_1_Of_H()
        {
            Registers.H = ClearBit(Registers.H, 1);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x8D_Reset_Bit_1_Of_L()
        {
            Registers.L = ClearBit(Registers.L, 1);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,_HL_
        /// </summary>
        private void Instruction_0xCB_0x8E_Reset_Bit_1_Of_Address_Pointed_To_By_HL()
        {
            Memory[Registers.HL] = ClearBit(Memory[Registers.HL], 1);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x8F_Reset_Bit_1_Of_A()
        {
            Registers.A = ClearBit(Registers.A, 1);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x90_Reset_Bit_2_Of_B()
        {
            Registers.B = ClearBit(Registers.B, 2);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x91_Reset_Bit_2_Of_C()
        {
            Registers.C = ClearBit(Registers.C, 2);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x92_Reset_Bit_2_Of_D()
        {
            Registers.D = ClearBit(Registers.D, 2);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x93_Reset_Bit_2_Of_E()
        {
            Registers.E = ClearBit(Registers.E, 2);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x94_Reset_Bit_2_Of_H()
        {
            Registers.H = ClearBit(Registers.H, 2);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x95_Reset_Bit_2_Of_L()
        {
            Registers.L = ClearBit(Registers.L, 2);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,_HL_
        /// </summary>
        private void Instruction_0xCB_0x96_Reset_Bit_2_Of_Address_Pointed_To_By_HL()
        {
            Memory[Registers.HL] = ClearBit(Memory[Registers.HL], 2);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x97_Reset_Bit_2_Of_A()
        {
            Registers.A = ClearBit(Registers.A, 2);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x98_Reset_Bit_3_Of_B()
        {
            Registers.B = ClearBit(Registers.B, 3);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x99_Reset_Bit_3_Of_C()
        {
            Registers.C = ClearBit(Registers.C, 3);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x9A_Reset_Bit_3_Of_D()
        {
            Registers.D = ClearBit(Registers.D, 3);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x9B_Reset_Bit_3_Of_E()
        {
            Registers.E = ClearBit(Registers.E, 3);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x9C_Reset_Bit_3_Of_H()
        {
            Registers.H = ClearBit(Registers.H, 3);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x9D_Reset_Bit_3_Of_L()
        {
            Registers.L = ClearBit(Registers.L, 3);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,_HL_
        /// </summary>
        private void Instruction_0xCB_0x9E_Reset_Bit_3_Of_Address_Pointed_To_By_HL()
        {
            Memory[Registers.HL] = ClearBit(Memory[Registers.HL], 3);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0x9F_Reset_Bit_3_Of_A()
        {
            Registers.A = ClearBit(Registers.A, 3);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xA0_Reset_Bit_4_Of_B()
        {
            Registers.B = ClearBit(Registers.B, 4);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xA1_Reset_Bit_4_Of_C()
        {
            Registers.C = ClearBit(Registers.C, 4);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xA2_Reset_Bit_4_Of_D()
        {
            Registers.D = ClearBit(Registers.D, 4);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xA3_Reset_Bit_4_Of_E()
        {
            Registers.E = ClearBit(Registers.E, 4);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xA4_Reset_Bit_4_Of_H()
        {
            Registers.H = ClearBit(Registers.H, 4);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xA5_Reset_Bit_4_Of_L()
        {
            Registers.L = ClearBit(Registers.L, 4);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,_HL_
        /// </summary>
        private void Instruction_0xCB_0xA6_Reset_Bit_4_Of_Address_Pointed_To_By_HL()
        {
            Memory[Registers.HL] = ClearBit(Memory[Registers.HL], 4);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xA7_Reset_Bit_4_Of_A()
        {
            Registers.A = ClearBit(Registers.A, 4);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xA8_Reset_Bit_5_Of_B()
        {
            Registers.B = ClearBit(Registers.B, 5);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xA9_Reset_Bit_5_Of_C()
        {
            Registers.C = ClearBit(Registers.C, 5);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xAA_Reset_Bit_5_Of_D()
        {
            Registers.D = ClearBit(Registers.D, 5);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xAB_Reset_Bit_5_Of_E()
        {
            Registers.E = ClearBit(Registers.E, 5);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xAC_Reset_Bit_5_Of_H()
        {
            Registers.H = ClearBit(Registers.H, 5);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xAD_Reset_Bit_5_Of_L()
        {
            Registers.L = ClearBit(Registers.L, 5);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,_HL_
        /// </summary>
        private void Instruction_0xCB_0xAE_Reset_Bit_5_Of_Address_Pointed_To_By_HL()
        {
            Memory[Registers.HL] = ClearBit(Memory[Registers.HL], 5);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xAF_Reset_Bit_5_Of_A()
        {
            Registers.A = ClearBit(Registers.A, 5);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xB0_Reset_Bit_6_Of_B()
        {
            Registers.B = ClearBit(Registers.B, 6);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xB1_Reset_Bit_6_Of_C()
        {
            Registers.C = ClearBit(Registers.C, 6);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xB2_Reset_Bit_6_Of_D()
        {
            Registers.D = ClearBit(Registers.D, 6);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xB3_Reset_Bit_6_Of_E()
        {
            Registers.E = ClearBit(Registers.E, 6);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xB4_Reset_Bit_6_Of_H()
        {
            Registers.H = ClearBit(Registers.H, 6);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xB5_Reset_Bit_6_Of_L()
        {
            Registers.L = ClearBit(Registers.L, 6);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,_HL_
        /// </summary>
        private void Instruction_0xCB_0xB6_Reset_Bit_6_Of_Address_Pointed_To_By_HL()
        {
            Memory[Registers.HL] = ClearBit(Memory[Registers.HL], 6);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xB7_Reset_Bit_6_Of_A()
        {
            Registers.A = ClearBit(Registers.A, 6);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xB8_Reset_Bit_7_Of_B()
        {
            Registers.B = ClearBit(Registers.B, 7);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xB9_Reset_Bit_7_Of_C()
        {
            Registers.C = ClearBit(Registers.C, 7);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xBA_Reset_Bit_7_Of_D()
        {
            Registers.D = ClearBit(Registers.D, 7);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xBB_Reset_Bit_7_Of_E()
        {
            Registers.E = ClearBit(Registers.E, 7);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xBC_Reset_Bit_7_Of_H()
        {
            Registers.H = ClearBit(Registers.H, 7);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xBD_Reset_Bit_7_Of_L()
        {
            Registers.L = ClearBit(Registers.L, 7);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,_HL_
        /// </summary>
        private void Instruction_0xCB_0xBE_Reset_Bit_7_Of_Address_Pointed_To_By_HL()
        {
            Memory[Registers.HL] = ClearBit(Memory[Registers.HL], 7);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xBF_Reset_Bit_7_Of_A()
        {
            Registers.A = ClearBit(Registers.A, 7);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xC0_Set_Bit_0_Of_B()
        {
            Registers.B = SetBit(Registers.B, 0);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xC1_Set_Bit_0_Of_C()
        {
            Registers.C = SetBit(Registers.C, 0);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xC2_Set_Bit_0_Of_D()
        {
            Registers.D = SetBit(Registers.D, 0);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xC3_Set_Bit_0_Of_E()
        {
            Registers.E = SetBit(Registers.E, 0);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xC4_Set_Bit_0_Of_H()
        {
            Registers.H = SetBit(Registers.H, 0);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xC5_Set_Bit_0_Of_L()
        {
            Registers.L = SetBit(Registers.L, 0);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,_HL_
        /// </summary>
        private void Instruction_0xCB_0xC6_Set_Bit_0_Of_Address_Pointed_To_By_HL()
        {
            Memory[Registers.HL] = SetBit(Memory[Registers.HL], 0);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xC7_Set_Bit_0_Of_A()
        {
            Registers.A = SetBit(Registers.A, 0);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xC8_Set_Bit_1_Of_B()
        {
            Registers.B = SetBit(Registers.B, 1);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xC9_Set_Bit_1_Of_C()
        {
            Registers.C = SetBit(Registers.C, 1);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xCA_Set_Bit_1_Of_D()
        {
            Registers.D = SetBit(Registers.D, 1);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xCB_Set_Bit_1_Of_E()
        {
            Registers.E = SetBit(Registers.E, 1);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xCC_Set_Bit_1_Of_H()
        {
            Registers.H = SetBit(Registers.H, 1);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xCD_Set_Bit_1_Of_L()
        {
            Registers.L = SetBit(Registers.L, 1);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,_HL_
        /// </summary>
        private void Instruction_0xCB_0xCE_Set_Bit_1_Of_Address_Pointed_To_By_HL()
        {
            Memory[Registers.HL] = SetBit(Memory[Registers.HL], 1);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xCF_Set_Bit_1_Of_A()
        {
            Registers.A = SetBit(Registers.A, 1);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xD0_Set_Bit_2_Of_B()
        {
            Registers.B = SetBit(Registers.B, 2);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xD1_Set_Bit_2_Of_C()
        {
            Registers.C = SetBit(Registers.C, 2);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xD2_Set_Bit_2_Of_D()
        {
            Registers.D = SetBit(Registers.D, 2);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xD3_Set_Bit_2_Of_E()
        {
            Registers.E = SetBit(Registers.E, 2);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xD4_Set_Bit_2_Of_H()
        {
            Registers.H = SetBit(Registers.H, 2);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xD5_Set_Bit_2_Of_L()
        {
            Registers.L = SetBit(Registers.L, 2);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,_HL_
        /// </summary>
        private void Instruction_0xCB_0xD6_Set_Bit_2_Of_Address_Pointed_To_By_HL()
        {
            Memory[Registers.HL] = SetBit(Memory[Registers.HL], 2);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xD7_Set_Bit_2_Of_A()
        {
            Registers.A = SetBit(Registers.A, 2);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xD8_Set_Bit_3_Of_B()
        {
            Registers.B = SetBit(Registers.B, 3);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xD9_Set_Bit_3_Of_C()
        {
            Registers.C = SetBit(Registers.C, 3);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xDA_Set_Bit_3_Of_D()
        {
            Registers.D = SetBit(Registers.D, 3);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xDB_Set_Bit_3_Of_E()
        {
            Registers.E = SetBit(Registers.E, 3);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xDC_Set_Bit_3_Of_H()
        {
            Registers.H = SetBit(Registers.H, 3);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xDD_Set_Bit_3_Of_L()
        {
            Registers.L = SetBit(Registers.L, 3);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,_HL_
        /// </summary>
        private void Instruction_0xCB_0xDE_Set_Bit_3_Of_Address_Pointed_To_By_HL()
        {
            Memory[Registers.HL] = SetBit(Memory[Registers.HL], 3);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xDF_Set_Bit_3_Of_A()
        {
            Registers.A = SetBit(Registers.A, 3);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xE0_Set_Bit_4_Of_B()
        {
            Registers.B = SetBit(Registers.B, 4);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xE1_Set_Bit_4_Of_C()
        {
            Registers.C = SetBit(Registers.C, 4);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xE2_Set_Bit_4_Of_D()
        {
            Registers.D = SetBit(Registers.D, 4);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xE3_Set_Bit_4_Of_E()
        {
            Registers.E = SetBit(Registers.E, 4);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xE4_Set_Bit_4_Of_H()
        {
            Registers.H = SetBit(Registers.H, 4);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xE5_Set_Bit_4_Of_L()
        {
            Registers.L = SetBit(Registers.L, 4);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,_HL_
        /// </summary>
        private void Instruction_0xCB_0xE6_Set_Bit_4_Of_Address_Pointed_To_By_HL()
        {
            Memory[Registers.HL] = SetBit(Memory[Registers.HL], 4);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xE7_Set_Bit_4_Of_A()
        {
            Registers.A = SetBit(Registers.A, 4);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xE8_Set_Bit_5_Of_B()
        {
            Registers.B = SetBit(Registers.B, 5);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xE9_Set_Bit_5_Of_C()
        {
            Registers.C = SetBit(Registers.C, 5);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xEA_Set_Bit_5_Of_D()
        {
            Registers.D = SetBit(Registers.D, 5);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xEB_Set_Bit_5_Of_E()
        {
            Registers.E = SetBit(Registers.E, 5);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xEC_Set_Bit_5_Of_H()
        {
            Registers.H = SetBit(Registers.H, 5);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xED_Set_Bit_5_Of_L()
        {
            Registers.L = SetBit(Registers.L, 5);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,_HL_
        /// </summary>
        private void Instruction_0xCB_0xEE_Set_Bit_5_Of_Address_Pointed_To_By_HL()
        {
            Memory[Registers.HL] = SetBit(Memory[Registers.HL], 5);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xEF_Set_Bit_5_Of_A()
        {
            Registers.A = SetBit(Registers.A, 5);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xF0_Set_Bit_6_Of_B()
        {
            Registers.B = SetBit(Registers.B, 6);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xF1_Set_Bit_6_Of_C()
        {
            Registers.C = SetBit(Registers.C, 6);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xF2_Set_Bit_6_Of_D()
        {
            Registers.D = SetBit(Registers.D, 6);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xF3_Set_Bit_6_Of_E()
        {
            Registers.E = SetBit(Registers.E, 6);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xF4_Set_Bit_6_Of_H()
        {
            Registers.H = SetBit(Registers.H, 6);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xF5_Set_Bit_6_Of_L()
        {
            Registers.L = SetBit(Registers.L, 6);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,_HL_
        /// </summary>
        private void Instruction_0xCB_0xF6_Set_Bit_6_Of_Address_Pointed_To_By_HL()
        {
            Memory[Registers.HL] = SetBit(Memory[Registers.HL], 6);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xF7_Set_Bit_6_Of_A()
        {
            Registers.A = SetBit(Registers.A, 6);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xF8_Set_Bit_7_Of_B()
        {
            Registers.B = SetBit(Registers.B, 7);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xF9_Set_Bit_7_Of_C()
        {
            Registers.C = SetBit(Registers.C, 7);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xFA_Set_Bit_7_Of_D()
        {
            Registers.D = SetBit(Registers.D, 7);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xFB_Set_Bit_7_Of_E()
        {
            Registers.E = SetBit(Registers.E, 7);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xFC_Set_Bit_7_Of_H()
        {
            Registers.H = SetBit(Registers.H, 7);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xFD_Set_Bit_7_Of_L()
        {
            Registers.L = SetBit(Registers.L, 7);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,_HL_
        /// </summary>
        private void Instruction_0xCB_0xFE_Set_Bit_7_Of_Address_Pointed_To_By_HL()
        {
            Memory[Registers.HL] = SetBit(Memory[Registers.HL], 7);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private void Instruction_0xCB_0xFF_Set_Bit_7_Of_A()
        {
            Registers.A = SetBit(Registers.A, 7);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RES_u3,r8
        /// </summary>
        private byte ClearBit(byte value, int bitToClear)
        {
            return (byte)(value & ~(1 << bitToClear));
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SET_u3,r8
        /// </summary>
        private byte SetBit(byte value, int bitToSet)
        {
            return (byte)(value | (1 << bitToSet));
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#BIT_u3,r8
        /// </summary>
        private void TestBitAndSetFlags(byte value, int bitToTest)
        {
            Registers.SetFlagTo(Flags.Zero, !value.IsBitSet(bitToTest));
            Registers.ClearFlag(Flags.AddSubtract);
            Registers.SetFlag(Flags.HalfCarry);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RLC_r8
        /// https://rednex.github.io/rgbds/gbz80.7.html#RLC__HL_
        /// </summary>
        private byte RotateLeftWithCarryAndSetFlags(byte value, bool clearZeroFlag = false)
        {
            Registers.SetFlagTo(Flags.Carry, (value & 0b1000_0000) != 0); //capture MSB in carry flag before rotating
            Registers.ClearFlag(Flags.AddSubtract | Flags.HalfCarry);

            var rotated = (byte)((value << 1) | (value >> 7));

            if (clearZeroFlag)
            {
                Registers.ClearFlag(Flags.Zero);
            }
            else
            {
                Registers.SetFlagTo(Flags.Zero, rotated == 0);
            }

            return rotated;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RL_r8
        /// </summary>
        private byte RotateLeftAndSetFlags(byte value, bool clearZeroFlag = false)
        {
            bool oldCarry = Registers.HasFlag(Flags.Carry);
            Registers.SetFlagTo(Flags.Carry, (value & 0b1000_0000) != 0); //capture MSB in carry flag before rotating
            Registers.ClearFlag(Flags.AddSubtract | Flags.HalfCarry);

            var rotated = (byte)((value << 1) | (oldCarry ? 1 : 0));

            if (clearZeroFlag)
            {
                Registers.ClearFlag(Flags.Zero);
            }
            else
            {
                Registers.SetFlagTo(Flags.Zero, rotated == 0);
            }

            return rotated;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SLA_r8
        /// </summary>
        private byte ShiftLeftAndSetFlags(byte value)
        {
            Registers.SetFlagTo(Flags.Carry, (value & 0b1000_0000) != 0); //capture MSB in carry flag before shifting
            Registers.ClearFlag(Flags.AddSubtract | Flags.HalfCarry);

            var shifted = (byte)(value << 1);

            Registers.SetFlagTo(Flags.Zero, shifted == 0);

            return shifted;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SRL_r8
        /// </summary>
        private byte ShiftRightAndSetFlags(byte value)
        {
            Registers.SetFlagTo(Flags.Carry, (value & 0b0000_0001) != 0); //capture LSB in carry flag before shifting
            Registers.ClearFlag(Flags.AddSubtract | Flags.HalfCarry);

            var shifted = (byte)(value >> 1);

            Registers.SetFlagTo(Flags.Zero, shifted == 0);

            return shifted;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SRA_r8
        /// "Arithmetic" shift preserves the sign bit https://stackoverflow.com/a/6269641
        /// </summary>
        private byte ArithmeticShiftRightAndSetFlags(byte value)
        {
            Registers.SetFlagTo(Flags.Carry, (value & 0b0000_0001) != 0); //capture LSB in carry flag before shifting
            Registers.ClearFlag(Flags.AddSubtract | Flags.HalfCarry);

            var shifted = (byte)((value >> 1) | (value & 0b1000_0000));

            Registers.SetFlagTo(Flags.Zero, shifted == 0);

            return shifted;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#SWAP_r8
        /// </summary>
        private byte SwapNibblesAndSetFlags(byte value)
        {
            byte swapped = (byte)((value << 4) | (value >> 4));
            Registers.ClearFlag(Flags.AddSubtract | Flags.HalfCarry | Flags.Carry);
            Registers.SetFlagTo(Flags.Zero, swapped == 0);
            return swapped;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RRC_r8
        /// https://rednex.github.io/rgbds/gbz80.7.html#RRC__HL_
        /// </summary>
        private byte RotateRightWithCarryAndSetFlags(byte value, bool clearZeroFlag = false)
        {
            Registers.SetFlagTo(Flags.Carry, (value & 0b0000_0001) != 0); //capture LSB in carry flag before rotating
            Registers.ClearFlag(Flags.AddSubtract | Flags.HalfCarry);

            var rotated = (byte)((value >> 1) | (value << 7));

            if (clearZeroFlag)
            {
                Registers.ClearFlag(Flags.Zero);
            }
            else
            {
                Registers.SetFlagTo(Flags.Zero, rotated == 0);
            }

            return rotated;
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RR_r8
        /// </summary>
        private byte RotateRightAndSetFlags(byte value, bool clearZeroFlag = false)
        {
            bool oldCarry = Registers.HasFlag(Flags.Carry);
            Registers.SetFlagTo(Flags.Carry, (value & 0b0000_0001) != 0); //capture LSB in carry flag before rotating
            Registers.ClearFlag(Flags.AddSubtract | Flags.HalfCarry);

            var rotated = (byte)((value >> 1) | (oldCarry ? 1 << 7 : 0));

            if (clearZeroFlag)
            {
                Registers.ClearFlag(Flags.Zero);
            }
            else
            {
                Registers.SetFlagTo(Flags.Zero, rotated == 0);
            }

            return rotated;
        }

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
            CyclesLastTick += 4;
        }

        /// <see cref="https://rednex.github.io/rgbds/gbz80.7.html#INC_r8"/>
        /// <see cref="https://github.com/TASVideos/BizHawk/blob/6d0973ca7ea3907abdcf482e6ce8f2767ae6f297/BizHawk.Emulation.Cores/CPUs/Z80A/Operations.cs#L467"/>
        private byte Increment8BitValueAndSetFlags(byte oldValue)
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
        private byte Decrement8BitValueAndSetFlags(byte oldValue)
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
