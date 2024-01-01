namespace axGB.System
{
    public sealed class CartridgeMBC1 : Cartridge
    {
        // https://gbdev.io/pandocs/MBC1.html
        private byte ramEnable;     // 4 bits
        private byte romBankNumber = 1; // 5 bits
        private byte ramBankNumber; // 2 bits
        private byte bankingMode;   // 1 bit

        private byte[] ram;

        public CartridgeMBC1() : base()
        {
            ram = new byte[0x8000]; // 4x 8k banks
        }

        public override byte ReadByte(ushort address)
        {
            // I think that ROM is stored in a physically linear way in the rom itself
            // and that the banking mechanism is really just used to map different parts
            // into the same address space

            // This seems like it really just boils down to an offset if I'm understanding
            // this right

            byte data = 0;
            switch (address)
            {
                case var addr when (address >= 0x0000 && address <= 0x3FFF):
                    data = rom[addr];
                    break;

                case var addr when (address >= 0x4000 && address <= 0x7FFF):
                    data = rom[addr - 0x4000 + (0x4000 * romBankNumber)];
                    break;

                case var addr when (address <= 0xBFFF):
                    if (ramEnable)
                    {
                    data = ram[(addr - 0xA000) + (0x2000 * ramBankNumber)];
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
                case var addr when (address >= 0x0000 && address <= 0x1FFF):
                    data = (byte)(value & 0b_00001111);
                    if (data == 0x0A)
                    {
                        ramEnable = data;
                    }
                    
                    break;

                case var addr when (address >= 0x2000 && address <= 0x3FFF):
                    data          = (byte)(value & 0b_00011111);
                    romBankNumber = (byte)(data == 0 ? 1 : data);
                    break;

                case var addr when (address >= 0x4000 && address < 0x5FFF):
                    ramBankNumber = (byte)(value & 0b_00000011);
                    break;

                case var addr when (address >= 0x6000 && address < 0x7FFF):
                    bankingMode = (byte)(value & 0b_00000001);
                    break;
            }
        }
    }
}
