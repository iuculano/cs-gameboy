using Emulator.Core.CPU;

namespace Enulator.Core.CPU;

public partial class InstructionSet
{
    private byte Res(byte bit, byte value)
    {
        unchecked
        {
            return (byte)(value & (byte)~(1 << bit));
        }
    }

    private byte Set(byte bit, byte value)
    {
        return (byte)(value | (1 << bit));
    }

    private void Bit(byte bit, byte value)
    {
        var result = (value & (1 << bit));

        var zero   = result == 0;
       
        SetFlags(CPUFlags.Zero,      zero);
        SetFlags(CPUFlags.HalfCarry, true);
        SetFlags(CPUFlags.Subtract,  false);
    }

    private byte Rlc(byte value, bool cb)
    {
        var bit7   = value >> 7; // bit 7 to bit 0;
        var result = value << 1 | bit7;

        var zero   = cb == true && result == 0;
        var carry  = bit7 == 1;
        SetFlags(CPUFlags.Zero,      zero);
        SetFlags(CPUFlags.Subtract,  false);
        SetFlags(CPUFlags.HalfCarry, false);
        SetFlags(CPUFlags.Carry,     carry);

        return (byte)result;
    }

    private byte Rrc(byte value, bool cb)
    {
        var bit0   = value & 0b_00000001;
        var result = value >> 1 | (bit0 << 7);

        var zero   = cb == true && result == 0;
        var carry  = bit0 == 1;

        SetFlags(CPUFlags.Zero,      zero);
        SetFlags(CPUFlags.Subtract,  false);
        SetFlags(CPUFlags.HalfCarry, false);
        SetFlags(CPUFlags.Carry,     carry);

        return (byte)result;
    }

    private byte Rl(byte value, bool cb)
    {
        var curCF  = HasFlags(CPUFlags.Carry) ? 1 : 0; 
        var result = (byte)((value << 1) | curCF);

        var zero   = cb == true && result == 0;
        var carry  = value >> 7 == 1;

        SetFlags(CPUFlags.Zero,      zero);
        SetFlags(CPUFlags.Subtract,  false);
        SetFlags(CPUFlags.HalfCarry, false);
        SetFlags(CPUFlags.Carry,     carry);

        return (byte)result;
    }

    private byte Rr(byte value, bool cb)
    {
        var curCF  = HasFlags(CPUFlags.Carry) ? 1 : 0;
        var bit0   = value & 0b_00000001;
        var result = value >> 1 | (curCF << 7);

        var zero   = cb == true && result == 0;
        var carry  = bit0 == 1;

        SetFlags(CPUFlags.Zero,      zero);
        SetFlags(CPUFlags.Subtract,  false);
        SetFlags(CPUFlags.HalfCarry, false);
        SetFlags(CPUFlags.Carry,     carry);

        return (byte)result;
    }

    private byte Sla(byte value)
    {
        var bit7   = (byte)(value >> 7);
        var result = (byte)(value << 1);

        var zero   = result == 0;
        var carry  = bit7   == 1;

        SetFlags(CPUFlags.Zero,      zero);
        SetFlags(CPUFlags.Subtract,  false);
        SetFlags(CPUFlags.HalfCarry, false);
        SetFlags(CPUFlags.Carry,     carry);

        return result;
    }

    private byte Sra(byte value)
    {
        var bit0   = value & 0b_00000001;
        var result = (byte)(value >> 1 | value & 0b_10000000); // Didn't expect this, but bit 7 stays the same

        var zero   = result == 0;
        var carry  = bit0   == 1;

        SetFlags(CPUFlags.Zero,      zero);
        SetFlags(CPUFlags.Subtract,  false);
        SetFlags(CPUFlags.HalfCarry, false);
        SetFlags(CPUFlags.Carry,     carry);

        return result;
    }

    private byte Swap(byte value)
    {
        var low    = value & 0b_00001111;
        var high   = value & 0b_11110000;
        var result = low << 4 | high >> 4;

        var zero   = result == 0;

        SetFlags(CPUFlags.Zero,      zero);
        SetFlags(CPUFlags.Subtract,  false);
        SetFlags(CPUFlags.HalfCarry, false);
        SetFlags(CPUFlags.Carry,     false);

        return (byte)result;
    }

    private byte Srl(byte value)
    {
        var bit0   = value & 0b_00000001;
        var result = (byte)(value >> 1 & 0b_01111111);

        var zero   = result == 0;
        var carry  = bit0 == 1;

        SetFlags(CPUFlags.Zero,      zero);
        SetFlags(CPUFlags.Subtract,  false);
        SetFlags(CPUFlags.HalfCarry, false);
        SetFlags(CPUFlags.Carry,     carry);

        return result;
    }

    private void And(byte value)
    {
        var result = (byte)(processor.registers.A & value);

        var zero   = result == 0;

        SetFlags(CPUFlags.Zero,      zero);
        SetFlags(CPUFlags.Subtract,  false);
        SetFlags(CPUFlags.HalfCarry, true);
        SetFlags(CPUFlags.Carry,     false);

        processor.registers.A = result;
    }

    private void Xor(byte value)
    {
        var result = (byte)(processor.registers.A ^ value);

        var zero   = result == 0;

        SetFlags(CPUFlags.Zero,      zero);
        SetFlags(CPUFlags.Subtract,  false);
        SetFlags(CPUFlags.HalfCarry, false);
        SetFlags(CPUFlags.Carry,     false);

        processor.registers.A = result;
    }

    private void Or(byte value)
    {
        var result = (byte)(processor.registers.A | value);

        var zero   = result == 0;

        SetFlags(CPUFlags.Zero,      zero);
        SetFlags(CPUFlags.Subtract,  false);
        SetFlags(CPUFlags.HalfCarry, false);
        SetFlags(CPUFlags.Carry,     false);

        processor.registers.A = result;
    }

    private void Cpl()
    {
        var result = ~processor.registers.A;
        SetFlags(CPUFlags.Subtract,  true);
        SetFlags(CPUFlags.HalfCarry, true);

        processor.registers.A = (byte)result;
    }
}
