using System;

namespace Emulator.Core.Bus;

[Flags]
public enum TimerControl
{
    Enabled              = 0b_00000100,
    ClockRateDivider16   = 0b_00000001, // 01
    ClockRateDivider64   = 0b_00000010, // 10
    ClockRateDivider256  = 0b_00000011, // 11
    ClockRateDivider1024 = 0b_00000000  // 00
}
