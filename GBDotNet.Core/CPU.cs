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

        private Action[] instructionSet;

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
                () => { },
                () => { },
                () => { },
                () => Instruction_0x0C_Increment_C(),
                () => { },
                () => { },
                () => { },
                //0x10
                () => { },
                () => { },
                () => { },
                () => { },
                () => Instruction_0x14_Increment_D(),
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => Instruction_0x1C_Increment_E(),
                () => { },
                () => { },
                () => { },
                //0x20
                () => { },
                () => { },
                () => { },
                () => { },
                () => Instruction_0x24_Increment_H(),
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => { },
                () => Instruction_0x2C_Increment_L(),
                () => { },
                () => { },
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
            Registers.B++;
            SetFlagsForIncrement(Registers.B);
        }

        private void Instruction_0x05_Decrement_B()
        {
            Registers.B--;
            SetFlagsForDecrement(Registers.B);
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

        private void Instruction_0x0C_Increment_C()
        {
            Registers.C++;
            SetFlagsForIncrement(Registers.C);
        }

        private void Instruction_0x14_Increment_D()
        {
            Registers.D++;
            SetFlagsForIncrement(Registers.D);
        }

        private void Instruction_0x1C_Increment_E()
        {
            Registers.E++;
            SetFlagsForIncrement(Registers.E);
        }

        private void Instruction_0x24_Increment_H()
        {
            Registers.H++;
            SetFlagsForIncrement(Registers.H);
        }

        private void Instruction_0x2C_Increment_L()
        {
            Registers.L++;
            SetFlagsForIncrement(Registers.L);
        }

        private void Instruction_0x3C_Increment_A()
        {
            Registers.A++;
            SetFlagsForIncrement(Registers.A);
        }

        //...

        private void Instruction_0x76_Halt()
        {
            IsHalted = true;
        }

        /// <see cref="https://rednex.github.io/rgbds/gbz80.7.html#INC_r8"/>
        /// <see cref="https://github.com/TASVideos/BizHawk/blob/6d0973ca7ea3907abdcf482e6ce8f2767ae6f297/BizHawk.Emulation.Cores/CPUs/Z80A/Operations.cs#L467"/>
        private void SetFlagsForIncrement(byte register)
        {
            //https://robdor.com/2016/08/10/gameboy-emulator-half-carry-flag/
            //var halfCarry = (((register & 0xf) + 1) & 0x10) == 0x10;
            //TODO: am I doing this half carry stuff right? (compare to execution in bgb)

            Registers.SetFlagTo(Flags.Zero, register == 0);
            Registers.SetFlagTo(Flags.HalfCarry, (register & 0b0000_1111) == 0);
            Registers.ClearFlag(Flags.AddSubtract);
        }

        /// <see cref="https://rednex.github.io/rgbds/gbz80.7.html#DEC_r8"/>
        /// <see cref="https://github.com/TASVideos/BizHawk/blob/6d0973ca7ea3907abdcf482e6ce8f2767ae6f297/BizHawk.Emulation.Cores/CPUs/Z80A/Operations.cs#L491"/>
        private void SetFlagsForDecrement(byte register)
        {
            Registers.SetFlagTo(Flags.Zero, register == 0);
            Registers.SetFlagTo(Flags.HalfCarry, (register & 0b0000_1111) == 0);
            Registers.SetFlag(Flags.AddSubtract);
        }

        public override string ToString() => Registers.ToString();
    }
}
