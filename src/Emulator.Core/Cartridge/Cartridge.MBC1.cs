namespace Enulator.Core.Cartridge;

public sealed class CartridgeMBC1 : Cartridge
{
    // https://gbdev.io/pandocs/MBC1.html

    // MBC memory mapped registers
    private bool ramEnable     = false;
    private byte romBankNumber = 1; // Reads as 1 if set to 0
    private byte ramBankNumber = 0;
    private byte bankingMode   = 0;

    // Additional RAM, potentially provided by the cartridge
    private byte[] ram;

    public CartridgeMBC1() : base()
    {
        ram = new byte[0x8000]; // 4x 8k banks
    }

    public override byte ReadByte(ushort address)
    {
        byte data = 0;
        switch (address)
        {
            // address is absolute when it comes in and the ROM and RAM are
            // their own arrays, not part of a contiguous block of memory.
            // We can just subtract by the offset to make it relative.

            // ROM Bank 00
            case var addr when address <= 0x3FFF:
                data = rom[addr];
                break;

            // ROM Bank 01-7F
            case var addr when address <= 0x7FFF:
                data = rom[addr - 0x4000 + 0x4000 * romBankNumber];
                break;

            // RAM Bank 00–03
            case var addr when address <= 0xBFFF:
                if (ramEnable)
                {
                    data = ram[addr - 0xA000 + 0x2000 * ramBankNumber];
                    break;
                }

                else
                {
                    // Pandoc says 0xFF is typical but no guaranteed - in
                    // practice, other values (like 0x00) seem to cause issues.
                    data = 0xFF;
                    break;
                }
        }

        return data;
    }

    public override void WriteByte(ushort address, byte value)
    {
        byte data;
        switch (address)
        {
            // RAM Enable register
            case var addr when address <= 0x1FFF:
                data      = (byte)(value & 0b_00001111);
                ramEnable = data == 0x0A ? true : false;
                break;

            // ROM Bank number register
            case var addr when address <= 0x3FFF:
                data = (byte)(value & 0b_00011111);

                // None of these values are valid, but it seems like we can just
                // carve out a bit of a special case here and increment by 1?
                if (data == 0x00 || data == 0x20 || data == 0x40 || data == 0x60)
                {
                    data += 1;
                }

                romBankNumber = data;
                break;

            // RAM Bank number register
            case var addr when address <= 0x5FFF:
                if (bankingMode == 0)
                {
                    // Really not sure if this is correct
                    data = (byte)(value & 0b_00011111);
                    if (data == 0x00 || data == 0x20 || data == 0x40 || data == 0x60)
                    {
                        data += 1;
                    }

                    romBankNumber = data;
                }

                else
                {
                    ramBankNumber = (byte)(value & 0b_00000011);
                }
                break;

            // Banking mode register
            case var addr when address <= 0x7FFF:
                // Either 0 or 1
                bankingMode = (byte)(value & 0b_00000001);
                break;
        }
    }
}
