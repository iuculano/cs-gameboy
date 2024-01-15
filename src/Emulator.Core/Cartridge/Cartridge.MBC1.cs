namespace Enulator.Core.Cartridge;

public sealed class CartridgeMBC1 : Cartridge
{
    // https://gbdev.io/pandocs/MBC1.html
    private bool ramEnable;
    private byte romBankNumber = 1;
    private byte ramBankNumber;
    private byte bankingMode;

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
            case var addr when address <= 0x3FFF:
                data = rom[addr];
                break;

            case var addr when address <= 0x7FFF:
                data = rom[addr - 0x4000 + 0x4000 * romBankNumber];
                break;

            case var addr when address <= 0xBFFF:
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
        // Really just all writes to memory mapped registers?

        byte data;
        switch (address)
        {
            case var addr when address <= 0x1FFF:
                data      = (byte)(value & 0b_00001111);
                ramEnable = data == 0x0A ? true : false;
                break;

            case var addr when address <= 0x3FFF:
                data = (byte)(value & 0b_00011111);

                // Not sure if this is a particuarlly good solution, but it seems like
                // I can just carve out a bit of a special case and increment these
                // invalid cases
                if (data == 0x00 || data == 0x20 || data == 0x40 || data == 0x60)
                {
                    data += 1;
                }

                romBankNumber = data;
                break;

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

            case var addr when address <= 0x7FFF:
                // Either 0 or 1
                bankingMode = (byte)(value & 0b_00000001);
                break;
        }
    }
}
