using System;

namespace Emulator.Core.PPU;

[Flags]
public enum LCDStatusInterrupt : byte
{
    // https://gbdev.io/pandocs/STAT.html#ff41--stat-lcd-status
    LYC    = 0b_01000000, // LYC == LY coincidence
    OAM    = 0b_00100000, // Mode 2
    VBlank = 0b_00010000, // Mode 1
    HBlank = 0b_00001000  // Mode 0
}
