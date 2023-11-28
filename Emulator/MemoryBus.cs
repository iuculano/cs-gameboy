using System;
using System.IO;
using System.Text;
using System.Diagnostics;


namespace axGB.System
{
    public partial class MemoryBus
    {
        // The basic memory map
        // ROM0, ROM0, and ERAM live on the catridge
        // https://gbdev.io/pandocs/Memory_Map.html
        public byte[] Bootstrap { get; init; }
        public byte[] VRAM      { get; init; }
        public byte[] WRAM      { get; init; }
        public byte[] MMIO      { get; init; }
        public byte[] HRAM      { get; init; }
        
        // Components
        private Cartridge cartridge { get; set; }


        public MemoryBus()
        {
            // The bootstrap rom sort of shadows the first 256 bytes of ROM0
            // Once the system powers on, it should read that set of data
            // instead until the BOOT register (at 0xFF50) is set to 1
            Bootstrap = File.ReadAllBytes("bootstrap.gb");

            VRAM = new byte[0x2000]; // 0x8000 - 0x9FFF
            WRAM = new byte[0x2000]; // 0xC000 - 0xDFFF
            MMIO = new byte[0x80];   // 0xFF00 - 0xFF7F
            HRAM = new byte[0x80];   // 0xFF80 - 0xFFFE
        }

        public void Connect(Cartridge cartridge)
        {
            this.cartridge = cartridge;
        }


        public byte ReadByte(ushort address)
        {
            byte data;
            switch (address) 
            {
                case var addr when (address >= 0x0000 && address <= 0x3FFF):
                    data = (BOOT == 0 && addr <= 0xFF) ? Bootstrap[addr] : cartridge.ReadByte(addr);
                    break;

                case var addr when (address >= 0x4000 && address <= 0x7FFF):
                    data = cartridge.ReadByte(addr);
                    break;

                case var addr when (address >= 0x8000 && address < 0xA000):
                    data = VRAM[addr - 0x8000];
                    break;

                case var addr when (address >= 0xA000 && address < 0xC000):
                    data = cartridge.ReadByte(addr);
                    break;

                case var addr when (address >= 0xC000 && address < 0xFF00):
                    data = WRAM[addr - 0xC000];
                    break;

                case var addr when (address >= 0xFF00 && address < 0xFF80):
                    // https://www.reddit.com/r/EmuDev/comments/ipap0w/blarggs_cpu_tests_and_the_stop_instruction/
                    if (addr == 0xFF4D)
                    {
                        data = 0xFF;                        
                    }

                    else
                    {
                        data = MMIO[addr - 0xFF00];
                    }
                    break;

                case var addr when (address >= 0xFF80 && address <= 0xFFFF):
                    return HRAM[addr - 0xFF80];

                default:
                    throw new Exception($"Illegal memory read: {address}");
            }

            return data;
        }

        public ushort ReadWord(ushort address)
        {
            var bytes = (Span<byte>)stackalloc byte[2];
            bytes[0]  = ReadByte(address);
            bytes[1]  = ReadByte((ushort)(address + 1));

            return BitConverter.ToUInt16(bytes);
        }

        public void WriteByte(ushort address, byte value)
        {
            switch (address) 
            {
                case var addr when (address >= 0x0000 && address <= 0x3FFF):
                    //cartridge.WriteROM0(addr, value);
                    break;

                case var addr when (address >= 0x4000 && address <= 0x7FFF):
                    //cartridge.WriteROM1(addr, value);
                    break;

                case var addr when (address >= 0x8000 && address <= 0x9FFF):
                    VRAM[addr - 0x8000] = value;
                    break;

                case var addr when (address >= 0xA000 && address <= 0xBFFF):
                    cartridge.WriteByte(addr, value);
                    break;

                case var addr when (address >= 0xC000 && address <= 0xDFFF):
                    WRAM[addr - 0xC000] = value;
                    break;

                case var addr when (address >= 0xFF00 && address <= 0xFF7F):
                    switch (addr)
                    {
                        // Joypad
                        case 0xFF00:
                            break;

                        // Serial - intercept this for debug purposes
                        case 0xFF02:
                        {
                            // Deviation from this value seems to be a sign the transfer is complete
                            if (value == 0b_10000001)
                            {
                                var text = Encoding.ASCII.GetString(new[] { SB });
                                Console.Write(text);
                            }

                            break;
                        }

                        // Timer and Divider Registers
                        case 0xFF04:
                            DIV = 0; // Writes to this register set it to 0
                            break;

                        default:
                            // Pass through
                            MMIO[addr - 0xFF00] = value;
                            break;
                    }

                    break;

                case var addr when (address >= 0xFF80 && address <= 0xFFFF):
                    HRAM[addr - 0xFF80] = value;
                    break;

                default:
                    break;
            }
        }

        public void WriteWord(ushort address, ushort value)
        {
            WriteByte(address,               (byte) (value & 0x00FF));
            WriteByte((ushort)(address + 1), (byte)((value & 0xFF00) >> 8));
        }
    }
}
