using System;

namespace Emulator.Core.Bus;

[Flags]
public enum InterruptType : byte
{
    // https://gbdev.io/pandocs/Interrupts.html#ffff--ie-interrupt-enable
    Joypad = 0b_00010000,
    Serial = 0b_00001000,
    Timer  = 0b_00000100,
    LCD    = 0b_00000010,
    VBlank = 0b_00000001
}
