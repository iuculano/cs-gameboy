using System;


namespace axGB.CPU
{
    public partial class InstructionSet
    {
        // Opcode implementation helpers
        private byte Inc(byte register)
        {
            var result = register + 1;
            
            var zero   = (byte)result == 0;
            var half   = (((register & 0x0F) + 1) & 0x10) == 0x10; // Limited to a 4 bit operation on both sides

            SetFlags(zero, Flags.Zero);
            ClearFlags(Flags.Subtract);
            SetFlags(half, Flags.HalfCarry);

            return (byte)result;
        }

        private byte Dec(byte register)
        {
            var result = register - 1;
            
            var zero   = (byte)result == 0;           // Half carry is set if it's less than 0
            var half   = ((register & 0x0F) - 1) < 0; // https://gbdev.io/pandocs/CPU_Registers_and_Flags.html

            SetFlags(zero, Flags.Zero);
            SetFlags(Flags.Subtract);
            SetFlags(half, Flags.HalfCarry);

            return (byte)result;
        }

         private byte Add(byte register, byte value)
        {
            var result = register + value;

            var zero   = (byte)result  == 0;
            var half   = (((register & 0x0F) + (value & 0x0F)) & 0x10) == 0x10;
            var carry  = result > 0xFF;
            
            SetFlags(zero,  Flags.Zero);
            ClearFlags(Flags.Subtract);
            SetFlags(half,  Flags.HalfCarry);
            SetFlags(carry, Flags.Carry);
            
            return (byte)result;
        }

        private ushort Add(ushort register, ushort value)
        {
            var result = register + value;

            var zero   = (byte)result  == 0;
            var half   = (((register & 0x0FFF) + (value & 0x0FFF)) & 0x1000) == 0x1000;
            var carry  = result > 0xFFFF;
            
            ClearFlags(Flags.Subtract);
            SetFlags(half,  Flags.HalfCarry);
            SetFlags(carry, Flags.Carry);
            
            return (ushort)result;
        }

        private byte Adc(byte register, byte value)
        {
            var hasCarry = Convert.ToInt32(HasFlags(Flags.Carry));
            var result   = (register + value) + hasCarry;  

            var zero     = (byte)result == 0;
            var half     = (((register & 0x0F) + (value & 0x0F) + hasCarry) & 0x10) == 0x10;
            var carry    = result > 0xFF;

            SetFlags(zero, Flags.Zero);
            ClearFlags(Flags.Subtract);
            SetFlags(half, Flags.HalfCarry);
            SetFlags(carry, Flags.Carry);

            return (byte)result;
        }
        
        private byte Sub(byte register, byte value)
        {
            var result = register - value;
            
            var zero   = (byte)result == 0;
            var half   = ((register & 0x0F) - (value & 0x0F)) < 0;
            var carry  = result < 0;

            SetFlags(zero,  Flags.Zero);
            SetFlags(Flags.Subtract);
            SetFlags(half,  Flags.HalfCarry);
            SetFlags(carry, Flags.Carry);

            return (byte)result;
        }

        private ushort Sub(ushort register, ushort value)
        {
            var result = register - value;
            
            var zero   = (byte)result == 0;
            var half   = ((register & 0x0FFF) + (value & 0x0FFF)) < 0;
            var carry  = result < 0;

            SetFlags(zero,  Flags.Zero);
            SetFlags(Flags.Subtract);
            SetFlags(half,  Flags.HalfCarry);
            SetFlags(carry, Flags.Carry);

            return (ushort)result;
        }

        private byte Sbc(byte register, byte value)
        {
            var hasCarry = Convert.ToInt32(HasFlags(Flags.Carry));
            var result   = (register - value) - hasCarry;

            var zero     = (byte)result == 0;
            var half     = ((register & 0x0F) - (value & 0x0F) - hasCarry) < 0;
            var carry    = result < 0;

            SetFlags(zero, Flags.Zero);
            SetFlags(Flags.Subtract);
            SetFlags(half, Flags.HalfCarry);
            SetFlags(carry, Flags.Carry);

            return (byte)result;
        }

        private byte Res(byte bit, byte value)
        {
            unchecked
            {
                return (byte)(value & (byte)~(1 << bit));
            }
        }

        private void Bit(byte bit, byte value)
        {
            var result = (value & (1 << bit)) == 0;
           
            SetFlags(result, Flags.Zero);
            SetFlags(Flags.HalfCarry);
            ClearFlags(Flags.Subtract);
        }


        private void Rlca()
        {
            var bit7   = processor.registers.A >> 7; // bit 7 to bit 0;
            var result = processor.registers.A << 1 | bit7;

            SetFlags(bit7 == 1, Flags.Carry);

            ClearFlags(Flags.Zero);
            ClearFlags(Flags.Subtract);
            ClearFlags(Flags.HalfCarry);

            processor.registers.A = (byte)result;
        }

        private void Rrca()
        {
            var bit0   = processor.registers.A & 0b_00000001;
            var result = processor.registers.A >> 1 | (bit0 << 7);

            SetFlags(bit0 == 1, Flags.Carry);

            ClearFlags(Flags.Zero);
            ClearFlags(Flags.Subtract);
            ClearFlags(Flags.HalfCarry);

            processor.registers.A = (byte)result;
        }

        private void Rla()
        {
            var carry  = HasFlags(Flags.Carry) ? 1 : 0;
            var bit7   = processor.registers.A & 0b_10000000;
            var result = processor.registers.A << 1 | carry;
            
            SetFlags(bit7 == 0b_10000000, Flags.Carry);

            ClearFlags(Flags.Zero);
            ClearFlags(Flags.Subtract);
            ClearFlags(Flags.HalfCarry);

            processor.registers.A = (byte)result;
        }

        private void Rra()
        {
            var carry  = HasFlags(Flags.Carry) ? 1 : 0;
            var bit0   = processor.registers.A & 0b_00000001;
            var result = processor.registers.A >> 1 | (carry << 7);

            SetFlags(bit0 == 0b_00000001, Flags.Carry);

            ClearFlags(Flags.Zero);
            ClearFlags(Flags.Subtract);
            ClearFlags(Flags.HalfCarry);

            processor.registers.A = (byte)result;
        }

        private byte Rr(byte value)
        {
            var carry  = HasFlags(Flags.Carry) ? 1 : 0;
            var bit0   = value & 0b_00000001;
            var result = value >> 1 | (carry << 7);

            SetFlags(result == 0, Flags.Zero);
            SetFlags(bit0 == 0b_00000001, Flags.Carry);
            
            ClearFlags(Flags.Subtract);
            ClearFlags(Flags.HalfCarry);

            return (byte)result;
        }

        private byte Rl(byte value)
        {
            var carry  = HasFlags(Flags.Carry) ? 1 : 0;
            var bit7   = value & 0b_10000000;
            var result = value << 1 | carry;

            SetFlags(result == 0, Flags.Zero);
            SetFlags(bit7 == 0b_10000000, Flags.Carry);

            ClearFlags(Flags.Subtract);
            ClearFlags(Flags.HalfCarry);

            return (byte)result;
        }


        private byte Srl(byte value)
        {
            var result = value >> 1;

            var zero   = (byte)result == 0;
            var carry  = (value & 0x01) == 0x01;

            SetFlags(zero, Flags.Zero);
            ClearFlags(Flags.Subtract);
            ClearFlags(Flags.HalfCarry);
            SetFlags(carry, Flags.Carry);
           

            return (byte)result;
        }
        
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
            }

            // Pay the conditional tax
            processor.cycles += 12;
        }

        private void Ret(Flags flags, bool condition)
        {
            if (HasFlags(flags) == condition)
            {
                processor.registers.PC = Pop();
            }

            processor.cycles += 12;
        }

        private void Push(ushort register)
        {
            var address = processor.registers.SP -= 2;
            processor.memory.WriteWord(address, register);
        }

        private ushort Pop()
        {
            var data                = processor.memory.ReadWord(processor.registers.SP);
            processor.registers.SP += 2;

            return data;
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
            }

            // Pay the conditional tax
            processor.cycles += 4;
        }

        private void JumpRelative(byte value)
        {
            // value can be negative here, it's possible to jump backwards
            var pc                 = (int)(processor.registers.PC) + (sbyte)value;
            processor.registers.PC = (ushort)pc;

            return;
        }

        private void JumpRelative(byte value, Flags flags, bool condition)
        {
            if (HasFlags(flags) == condition)
            {
                JumpRelative(value);
            }

            // Pay the conditional tax
            processor.cycles += 4;
        }

        private void And(byte value)
        {
            var result = (byte)(processor.registers.A & value);

            SetFlags((byte)result == 0, Flags.Zero);
            ClearFlags(Flags.Subtract);
            SetFlags(Flags.HalfCarry);
            ClearFlags(Flags.Carry);

            processor.registers.A = result;
        }

        private void Xor(byte value)
        {
            var result = (byte)(processor.registers.A ^ value);

            SetFlags((byte)result == 0, Flags.Zero);
            ClearFlags(Flags.Subtract);
            ClearFlags(Flags.HalfCarry);
            ClearFlags(Flags.Carry);

            processor.registers.A = result;
        }

        private void Or(byte value)
        {
            var result = (byte)(processor.registers.A | value);

            SetFlags((byte)result == 0, Flags.Zero);
            ClearFlags(Flags.Subtract);  
            ClearFlags(Flags.HalfCarry);
            ClearFlags(Flags.Carry);

            processor.registers.A = result;
        }

        private void Cp(byte value)
        {
            var result = processor.registers.A - value;

            var zero   = (byte)result == 0;
            var half   = ((processor.registers.A & 0x0F) - (value & 0x0F)) < 0;
            var carry  = result < 0;

            SetFlags(zero,  Flags.Zero);
            SetFlags(Flags.Subtract);
            SetFlags(half,  Flags.HalfCarry);
            SetFlags(carry, Flags.Carry);
        }

        private void Cpl()
        {
            var result = ~processor.registers.A;
            SetFlags(Flags.Subtract);
            SetFlags(Flags.HalfCarry);

            processor.registers.A = (byte)result;
        }

        private byte Swap(byte value)
        {
            var result = value << 4 | value >> 4;

            SetFlags(result == 0, Flags.Zero);

            ClearFlags(Flags.Subtract);
            ClearFlags(Flags.HalfCarry);
            ClearFlags(Flags.Carry);

            return (byte)result;
        }

        private void Rst(ushort value)
        {
            Call(value);
        }

        private void Scf()
        {
            SetFlags(Flags.Carry);

            ClearFlags(Flags.Subtract);
            ClearFlags(Flags.HalfCarry);
        }

        private void Ccf()
        {
            SetFlags(!HasFlags(Flags.Carry), Flags.Carry);

            ClearFlags(Flags.Subtract);
            ClearFlags(Flags.HalfCarry);
        }
    }
}
