using System;
namespace Emulator.Core.CPU;

[Flags]
public enum CPUFlags : byte
{
    None      = 0b_00000000,
    Zero      = 0b_10000000,
    Subtract  = 0b_01000000,
    HalfCarry = 0b_00100000,
    Carry     = 0b_00010000,
    All       = 0b_11110000
}
