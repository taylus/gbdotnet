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
                () => Instruction_0x0F_Rotate_A_Right_With_Carry(),
                //0x10
                () => Instruction_0x10_Stop(),
                () => Instruction_0x11_Load_DE_With_16_Bit_Immediate(),
                () => Instruction_0x12_Load_Address_Pointed_To_By_DE_With_A(),
                () => Instruction_0x13_Increment_DE(),
                () => Instruction_0x14_Increment_D(),
                () => Instruction_0x15_Decrement_D(),
                () => Instruction_0x16_Load_D_With_8_Bit_Immediate(),
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => Instruction_0x1C_Increment_E(),
                () => { },
                () => Instruction_0x1E_Load_E_With_8_Bit_Immediate(),
                () => { },
                //0x20
                () => { },
                () => { },
                () => { },
                () => { },
                () => Instruction_0x24_Increment_H(),
                () => { },
                () => Instruction_0x26_Load_H_With_8_Bit_Immediate(),
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => Instruction_0x2C_Increment_L(),
                () => { },
                () => Instruction_0x2E_Load_L_With_8_Bit_Immediate(),
                () => { },
                //0x30
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => Instruction_0x3C_Increment_A(),
                () => { },
                () => { },
                () => { },
                //0x40
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                //0x50
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                //0x60
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                //0x70
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => Instruction_0x76_Halt(),
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
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

        private void Instruction_0x01_Load_BC_With_16_Bit_Immediate()
        {
            byte a = Fetch();
            byte b = Fetch();
            Registers.BC = Common.ToLittleEndian(a, b);
        }

        private void Instruction_0x02_Load_Address_Pointed_To_By_BC_With_A()
        {
            Memory[Registers.BC] = Registers.A;
        }

        private void Instruction_0x03_Increment_BC()
        {
            Registers.BC++;
        }

        private void Instruction_0x04_Increment_B()
        {
            Registers.B = Increment8BitRegisterAndSetFlags(Registers.B);
        }

        private void Instruction_0x05_Decrement_B()
        {
            Registers.B = Decrement8BitRegisterAndSetFlags(Registers.B);
        }

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
        /// <see cref="https://ez80.readthedocs.io/en/latest/docs/bit-shifts/rla.html"/>
        /// <see cref="https://stackoverflow.com/questions/812022/c-sharp-bitwise-rotate-left-and-rotate-right"/>
        /// <see cref="https://github.com/sinamas/gambatte/blob/master/libgambatte/src/cpu.cpp#L561"/>
        private void Instruction_0x07_Rotate_A_Left_Circular()
        {
            Registers.A = (byte)((Registers.A << 1) | (Registers.A >> 7));
            Registers.SetFlagTo(Flags.Carry, (Registers.A & 0b0000_0001) != 0);
            Registers.ClearFlag(Flags.Zero);
            Registers.ClearFlag(Flags.AddSubtract);
            Registers.ClearFlag(Flags.HalfCarry);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#LD__n16_,SP
        /// https://github.com/sinamas/gambatte/blob/master/libgambatte/src/cpu.cpp#L570
        /// </summary>
        private void Instruction_0x08_Load_Address_With_Stack_Pointer()
        {
            byte addressLow = Fetch();
            byte addressHigh = Fetch();
            ushort address = Common.ToLittleEndian(addressLow, addressHigh);

            Memory[address] = (byte)(Registers.SP & 0xFF);
            Memory[address + 1] = (byte)(Registers.SP >> 8);
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#ADD_HL,r16
        /// </summary>
        private void Instruction_0x09_Add_BC_To_HL()
        {
            Registers.ClearFlag(Flags.AddSubtract);
            Registers.SetFlagTo(Flags.HalfCarry, ((Registers.HL & 0xFFF) + (Registers.BC & 0xFFF) > 0xFFF));
            Registers.SetFlagTo(Flags.Carry, (Registers.HL + Registers.BC > 0xFFFF));
            Registers.HL += Registers.BC;
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

        private void Instruction_0x0E_Load_C_With_8_Bit_Immediate()
        {
            Registers.C = Fetch();
        }

        /// <summary>
        /// https://rednex.github.io/rgbds/gbz80.7.html#RRCA
        /// </summary>
        private void Instruction_0x0F_Rotate_A_Right_With_Carry()
        {
            throw new NotImplementedException();
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

        private void Instruction_0x11_Load_DE_With_16_Bit_Immediate()
        {
            throw new NotImplementedException();
        }

        private void Instruction_0x12_Load_Address_Pointed_To_By_DE_With_A()
        {
            throw new NotImplementedException();
        }

        private void Instruction_0x13_Increment_DE()
        {
            throw new NotImplementedException();
        }

        private void Instruction_0x14_Increment_D()
        {
            Registers.D = Increment8BitRegisterAndSetFlags(Registers.D);
        }

        private void Instruction_0x15_Decrement_D()
        {
            throw new NotImplementedException();
        }

        private void Instruction_0x16_Load_D_With_8_Bit_Immediate()
        {
            Registers.D = Fetch();
        }

        private void Instruction_0x1C_Increment_E()
        {
            Registers.E = Increment8BitRegisterAndSetFlags(Registers.E);
        }

        private void Instruction_0x1E_Load_E_With_8_Bit_Immediate()
        {
            Registers.E = Fetch();
        }

        private void Instruction_0x24_Increment_H()
        {
            Registers.H = Increment8BitRegisterAndSetFlags(Registers.H);
        }

        private void Instruction_0x26_Load_H_With_8_Bit_Immediate()
        {
            Registers.H = Fetch();
        }

        private void Instruction_0x2C_Increment_L()
        {
            Registers.L = Increment8BitRegisterAndSetFlags(Registers.L);
        }

        private void Instruction_0x2E_Load_L_With_8_Bit_Immediate()
        {
            Registers.L = Fetch();
        }

        private void Instruction_0x3C_Increment_A()
        {
            Registers.A = Increment8BitRegisterAndSetFlags(Registers.A);
        }

        //...

        private void Instruction_0x76_Halt()
        {
            IsHalted = true;
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
