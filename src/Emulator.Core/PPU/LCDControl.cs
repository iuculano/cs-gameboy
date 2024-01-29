namespace Emulator.Core.PPU;

public enum LCDControl : byte
{
    // https://gbdev.io/pandocs/LCDC.html#ff40--lcdc-lcd-control
    LCDEnabled             = 0b_10000000,
    WindowArea             = 0b_01000000,
    WindowEnabled          = 0b_00100000,
    BackgroundDataArea     = 0b_00010000,
    BackgroundTileMapArea  = 0b_00001000,
    ObjectSize             = 0b_00000100,
    ObjectEnabled          = 0b_00000010,
    BackgroundWindowEnable = 0b_00000001
}
