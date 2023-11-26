namespace axGB.System
{
    public sealed class CartridgeMBC1 : Cartridge
    {
        // https://gbdev.io/pandocs/MBC1.html
        private byte ramEnable;     // 4 bits
        private byte romBankNumber; // 5 bits
        private byte ramBankNumber; // 2 bits
        private byte bankingMode;   // 1 bit

        public CartridgeMBC1() : base()
        {

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
                    //data = (BOOT == 0 && addr <= 0xFF) ? Bootstrap[addr] : cartridge.ReadByte(addr);
                    break;

                case var addr when (address >= 0x4000 && address <= 0x7FFF):
                    //data = cartridge.ReadByte(addr);
                    break;

                case var addr when (address >= 0xA000 && address <= 0xBFFF):
                    //ERAM.Span[addr - 0xA000] = value;
                    break;
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
