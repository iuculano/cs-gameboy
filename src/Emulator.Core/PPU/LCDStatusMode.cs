namespace Emulator.Core.PPU;

public enum LCDStatusMode : byte
{
    HBlank   = 0b_00000000,
    VBlank   = 0b_00000001,
    OAM      = 0b_00000010,
    Transfer = 0b_00000011
}
