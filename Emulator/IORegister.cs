using System;

namespace axGB.System
{
    public enum IORegister : int
    {
        JOYP = 0x00,
        SB   = 0x01,
        SC   = 0x02,
        DIV  = 0x04,
        TIMA = 0x05,
        TMA  = 0x06,
        TAC  = 0x07,
        NR10 = 0x10,
        NR11 = 0x11,
        NR12 = 0x12,
        NR13 = 0x13,
        NR14 = 0x14,
        NR21 = 0x16,
        NR22 = 0x17,
        NR23 = 0x18,
        NR24 = 0x19,
        NR30 = 0x1A,
        NR31 = 0x1B,
        NR32 = 0x1C,
        NR33 = 0x1D,
        NR34 = 0x1E,
        NR41 = 0x20,
        NR42 = 0x21,
        NR43 = 0x22,
        NR44 = 0x23,
        NR50 = 0x24,
        NR51 = 0x25,
        NR52 = 0x26,
        IE   = 0xFF,
        IF   = 0x0F
    }
}
