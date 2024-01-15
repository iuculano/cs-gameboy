using System;

namespace Emulator.Core.CPU;

[Flags]
public enum InterruptType : byte
{
    Joypad = 0b_00010000,
    Serial = 0b_00001000,
    Timer  = 0b_00000100,
    LCD    = 0b_00000010,
    VBlank = 0b_00000001
}
