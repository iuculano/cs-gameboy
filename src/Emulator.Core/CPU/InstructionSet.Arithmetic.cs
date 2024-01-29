using Emulator.Core.CPU;
using System;

namespace Enulator.Core.CPU;

public partial class InstructionSet
{
    private byte Inc(byte register)
    {
        var result = register + 1;

        var zero = (byte)result == 0;
        var half = (register & 0x0F) + 1 > 0x0F;

        SetFlags(CPUFlags.Zero,      zero);
        SetFlags(CPUFlags.Subtract,  false);
        SetFlags(CPUFlags.HalfCarry, half);

        return (byte)result;
    }

    private byte Dec(byte register)
    {
        var result = register - 1;

        var zero = (byte)result == 0;         // Half carry is set if it's less than 0
        var half = (register & 0x0F) - 1 < 0; // https://gbdev.io/pandocs/CPU_Registers_and_Flags.html

        SetFlags(CPUFlags.Zero,      zero);
        SetFlags(CPUFlags.Subtract,  true);
        SetFlags(CPUFlags.HalfCarry, half);

        return (byte)result;
    }

    private byte Add(byte register, byte value)
    {
        var result = register + value;

        var zero  = (byte)result == 0;
        var half  = (register & 0x0F) + (value & 0x0F) > 0x0F;
        var carry = result > 0xFF;

        SetFlags(CPUFlags.Zero,      zero);
        SetFlags(CPUFlags.Subtract,  false);
        SetFlags(CPUFlags.HalfCarry, half);
        SetFlags(CPUFlags.Carry,     carry);

        return (byte)result;
    }

    private ushort Add(ushort register, ushort value)
    {
        var result = register + value;

        var zero  = (byte)result == 0;
        var half  = (register & 0x0FFF) + (value & 0x0FFF) > 0x0FFF;
        var carry = result > 0xFFFF;

        SetFlags(CPUFlags.Subtract,  false);
        SetFlags(CPUFlags.HalfCarry, half);
        SetFlags(CPUFlags.Carry,     carry);

        return (ushort)result;
    }

    private ushort AddSPRelative(byte value)
    {
        var result = processor.registers.SP + (sbyte)value;

        var half  = (processor.registers.SP & 0x0F) + (value & 0x0F) > 0x0F;
        var carry = (processor.registers.SP & 0xFF) + value > 0xFF; // I'm not entirely sure why this works :|
        // I'd think it'd be (sbyte)value, but that fails
        SetFlags(CPUFlags.Zero,      false);
        SetFlags(CPUFlags.Subtract,  false);
        SetFlags(CPUFlags.HalfCarry, half);
        SetFlags(CPUFlags.Carry,     carry);

        return (ushort)result;
    }

    private byte Adc(byte register, byte value)
    {
        var hasCarry = Convert.ToInt32(HasFlags(CPUFlags.Carry));
        var result   = register + value + hasCarry;

        var zero  = (byte)result == 0;
        var half  = (register & 0x0F) + (value & 0x0F) + hasCarry > 0x0F;
        var carry = result > 0xFF;

        SetFlags(CPUFlags.Zero,      zero);
        SetFlags(CPUFlags.Subtract,  false);
        SetFlags(CPUFlags.HalfCarry, half);
        SetFlags(CPUFlags.Carry,     carry);

        return (byte)result;
    }

    private byte Sub(byte register, byte value)
    {
        var result = register - value;

        var zero  = (byte)result == 0;
        var half  = (register & 0x0F) - (value & 0x0F) < 0;
        var carry = result < 0;

        SetFlags(CPUFlags.Zero,      zero);
        SetFlags(CPUFlags.Subtract,  true);
        SetFlags(CPUFlags.HalfCarry, half);
        SetFlags(CPUFlags.Carry,     carry);

        return (byte)result;
    }

    private byte Sbc(byte register, byte value)
    {
        var hasCarry = Convert.ToInt32(HasFlags(CPUFlags.Carry));
        var result   = register - value - hasCarry;

        var zero  = (byte)result == 0;
        var half  = (register & 0x0F) - (value & 0x0F) - hasCarry < 0;
        var carry = result < 0;

        SetFlags(CPUFlags.Zero,      zero);
        SetFlags(CPUFlags.Subtract,  true);
        SetFlags(CPUFlags.HalfCarry, half);
        SetFlags(CPUFlags.Carry,     carry);

        return (byte)result;
    }

    private void Daa()
    {
        // https://forums.nesdev.org/viewtopic.php?t=15944

        var result = processor.registers.A;
        if (!HasFlags(CPUFlags.Subtract))
        {
            if (HasFlags(CPUFlags.Carry) || processor.registers.A > 0x99)
            {
                result += 0x60;
                SetFlags(CPUFlags.Carry, true);
            }

            if (HasFlags(CPUFlags.HalfCarry) || (processor.registers.A & 0x0F) > 0x09)
            {
                result += 0x06;
            }
        }

        else
        {
            if (HasFlags(CPUFlags.Carry))
            {
                result -= 0x60;
            }

            if (HasFlags(CPUFlags.HalfCarry))
            {
                result -= 0x06;
            }
        }

        processor.registers.A = result;

        var zero = result == 0;
        SetFlags(CPUFlags.Zero,      zero);
        SetFlags(CPUFlags.HalfCarry, false);
    }
}
