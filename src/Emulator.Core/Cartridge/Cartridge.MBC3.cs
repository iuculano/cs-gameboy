namespace Enulator.Core.Cartridge;

public sealed class CartridgeMBC3 : Cartridge
{
    // https://gbdev.io/pandocs/MBC1.html

    // MBC memory mapped registers
    private bool ramEnable     = false;
    private bool rtcEnable     = false;
    private byte romBankNumber = 1; // Reads as 1 if set to 0
    private byte ramBankNumber = 0;
    private byte bankingMode   = 0;

    // Additional RAM, potentially provided by the cartridge
    private byte[] ram;

    public CartridgeMBC3() : base()
    {
        ram = new byte[0x8000]; // 4x 8k banks
    }

    public override byte ReadByte(ushort address)
    {
        // address is absolute when it comes in and the ROM and RAM are
        // their own arrays, not part of a contiguous block of memory.
        // We can just subtract by the offset to make it relative.

        byte data = 0;
        switch (address)
        {
            // ROM Bank 00
            case var addr when address <= 0x3FFF:
                data = rom[addr];
                break;

            // ROM Bank 01-7F
            case var addr when address <= 0x7FFF:
                data = rom[addr - 0x4000 + 0x4000 * romBankNumber];
                break;

            // RAM bank 00-03
            case var addr when address >= 0xA000 && address <= 0xBFFF:
                if (ramEnable)
                {
                    data = ram[addr - 0xA000 + 0x2000 * ramBankNumber];
                    break;
                }

                else
                {
                    //What gets returned if RAM is not enabled?
                    data = 0xFF;
                    break;
                }
        }

        return data;
    }

    public override void WriteByte(ushort address, byte value)
    {
        // https://gbdev.io/pandocs/MBC3.html

        byte data;
        switch (address)
        {
            // RAM Enable register
            case var addr when address <= 0x1FFF:
                // Look for 0x0A in the lower 4 bits
                // Why would it be 0x0A? Oddly arbitrary
                data = (byte)(value & 0b_00001111);
                if (data == 0x0A)
                {
                    ramEnable = true;
                    rtcEnable = true;
                }

                else if (data == 0x00)
                {
                    ramEnable = false;
                    rtcEnable = false;
                }

                break;

            // ROM Bank number register
            case var addr when address <= 0x3FFF:
                data          = (byte)(value & 0b_01111111);
                romBankNumber = (byte)(data == 0 ? 1 : data);
                break;

            // RAM Bank number register
            case var addr when address <= 0x5FFF:
                if (value == 0x00 && value <= 0x03 ||
                    value == 0x08 && value <= 0x0C)
                {
                    ramBankNumber = value;
                }

                ramBankNumber = (byte)(value & 0b_00001100); // 0x00 - 0x03
                break;

            // Latch Clock register
            case var addr when address <= 0x7FFF:
                // RTC stub, just a no-op for now
                rtcEnable = false;
                break;

            // RAM Bank 00-03
            case var addr when address >= 0xA000 && address <= 0xBFFF:
                ram[addr - 0xA000 + 0x2000 + ramBankNumber] = value;
                break;
        }
    }
}
