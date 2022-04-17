using System;


namespace axGB.CPU
{
    public partial class InstructionSet
    {
        private void Call(ushort address)
        {
            Push(processor.registers.PC);
            processor.registers.PC = address;
        }

        private void Call(ushort address, Flags flags, bool condition)
        {
            if (HasFlags(flags) == condition)
            {
                Call(address);
                processor.cycles += 12;
            }
        }

        private void Ret(Flags flags, bool condition)
        {
            if (HasFlags(flags) == condition)
            {
                processor.registers.PC  = Pop();
                processor.cycles       += 12;
            }
        }

        private void Reti()
        {
            processor.interuptHandler.IME = true;
            processor.registers.PC        = Pop();
        }

        private void Jump(ushort value)
        {
            processor.registers.PC = value;
        }

        private void Jump(ushort value, Flags flags, bool condition)
        {
            if (HasFlags(flags) == condition)
            {
                Jump(value);
                processor.cycles += 4;
            }
        }

        private void JumpRelative(byte value)
        {
            // value can be negative here, it's possible to jump backwards
            var pc                 = (int)(processor.registers.PC) + (sbyte)value;
            processor.registers.PC = (ushort)pc;
        }

        private void JumpRelative(byte value, Flags flags, bool condition)
        {
            if (HasFlags(flags) == condition)
            {
                JumpRelative(value);
                processor.cycles += 4;
            }
        }

        private void Cp(byte value)
        {
            var result = processor.registers.A - value;

            var zero   = (byte)result == 0;
            var half   = ((processor.registers.A & 0x0F) - (value & 0x0F)) < 0;
            var carry  = result < 0;

            SetFlags(Flags.Zero,      zero);
            SetFlags(Flags.Subtract,  true);
            SetFlags(Flags.HalfCarry, half);
            SetFlags(Flags.Carry,     carry);
        }

        private void Rst(ushort value)
        {
            Call(value);
        }
    }
}
