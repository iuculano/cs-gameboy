namespace Enulator.Core.Cartridge;

public sealed class CartridgeMBC3 : Cartridge
{
    // https://gbdev.io/pandocs/MBC1.html
    private bool ramEnable;
    private bool rtcEnable;

    private byte romBankNumber = 1;
    private byte ramBankNumber;
    private byte bankingMode;

    private byte[] ram;

    public CartridgeMBC3() : base()
    {
        ram = new byte[0x8000]; // 4x 8k banks
    }

    public override byte ReadByte(ushort address)
    {
        // Need to map the absolute address to the relative location in memory
        // For example, 0xA000 is the start of ERAM, but is 0x0000 in the array
        // Since it's one array, we can just subtract the start of its absolute
        // location in the memory map to get the relative address, then add the
        // the bank's offset, which is the size of the bank * the number
        byte data = 0;
        switch (address)
        {
            case var addr when address <= 0x3FFF:
                data = rom[addr];
                break;

            case var addr when address <= 0x7FFF:
                data = rom[addr - 0x4000 + 0x4000 * romBankNumber];
                break;

            case var addr when address >= 0xA000 && address <= 0xBFFF:
                if (ramEnable)
                {
                    data = ram[addr - 0xA000 + 0x2000 * ramBankNumber];
                    break;
                }

                else
                {
                    //What gets returned if RAM is not enabled?
                    data = 0x00;
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

            case var addr when address <= 0x3FFF:
                data = (byte)(value & 0b_01111111);
                romBankNumber = (byte)(data == 0 ? 1 : data);
                break;

            case var addr when address <= 0x5FFF:
                if (value == 0x00 && value <= 0x03 ||
                    value == 0x08 && value <= 0x0C)
                {
                    ramBankNumber = value;
                }
                ramBankNumber = (byte)(value & 0b_00001100); // 0x00 - 0x03
                break;

            case var addr when address <= 0x7FFF:
                // RTC stub
                break;

            case var addr when address >= 0xA000 && address <= 0xBFFF:
                ram[addr - 0xA000 + 0x2000 + ramBankNumber] = value;
                break;
        }
    }
}
