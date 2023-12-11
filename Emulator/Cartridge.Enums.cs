namespace axGB.System
{
    // Pretty much all of this came from here:
    // https://gbdev.gg8.se/wiki/articles/The_Cartridge_Header
    public enum ROMSize
    {
        // This reads dumb, but can't start with a number :|
        Kilobytes32  = 0x00,
        Kilobytes64  = 0x01,
        Kilobytes128 = 0x02,
        Kilobytes256 = 0x03,
        Kilobytes512 = 0x04,
        Megabytes1   = 0x05,
        Megabytes2   = 0x06,
        Megabytes4   = 0x07,
        Megabytes8   = 0x08
    }

    public enum RAMSize
    {
        None         = 0x00,
        Kilobytes2   = 0x01,
        Kilobytes8   = 0x02,
        Kilobytes32  = 0x03,
        Kilobytes64  = 0x05, // Yes, 5 - apparently
        Kilobytes128 = 0x04
    }

    public enum CartridgeType
    {
        RomOnly                    = 0x00,
        MBC1                       = 0x01,
        MBC1Ram                    = 0x02,
        MBC1RamBattery             = 0x03,
        MBC2                       = 0x05,
        MBC2Battery                = 0x06,
        RomRam                     = 0x08,
        RomRamBattery              = 0x09,
        MMM01                      = 0x0B,
        MMM01Ram                   = 0x0C,
        MMM01RamBattery            = 0x0D,
        MBC3TimerBattery           = 0x0F,
        MBC3TimerRamBattery        = 0x10,
        MBC3                       = 0x11,
        MBC3Ram                    = 0x12,
        MBC3RamBattery             = 0x13,
        MBC5                       = 0x19,
        MBC5Ram                    = 0x1A,
        MBC5RamBattery             = 0x1B,
        MBC5Rumble                 = 0x1C,
        MBC5RumbleRam              = 0x1D,
        MBC5RumbleRamBattery       = 0x1E,
        MBC6                       = 0x20,
        MBC7SensorRumbleRamBattery = 0x22
    }

    public enum DestinationCode
    {
        Japanese    = 0x00,
        NonJapanese = 0x01
    }
}
