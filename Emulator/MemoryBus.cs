using System;
using System.IO;
using System.Text;
using System.Diagnostics;


namespace axGB.System
{
    public partial class MemoryBus
    {
        // The basic memory map
        public byte[]       Bootstrap { get; init; }
        public byte[]       Memory    { get; init; }
        public Memory<byte> ROM0      { get; init; }
        public Memory<byte> ROM1      { get; init; }
        public Memory<byte> VRAM      { get; init; }
        public Memory<byte> ERAM      { get; init; }
        public Memory<byte> WRAM      { get; init; }
        public Memory<byte> MMIO      { get; init; }
        public Memory<byte> ZRAM      { get; init; }
        
        // Components
        private Cartridge cartridge { get; set; }


        public MemoryBus()
        {
            // The bootstrap rom sort of shadows the first 256 bytes of ROM0
            // Once the system powers on, it should read that set of data
            // instead until the BOOT register (at 0xFF50) is set to 1
            Bootstrap = File.ReadAllBytes("bootstrap.gb");


            Memory = new byte[0x10000];
            ROM0   = Memory.AsMemory(0x0000..0x4000);
            ROM1   = Memory.AsMemory(0x4000..0x8000);
            VRAM   = Memory.AsMemory(0x8000..0xA000);
            ERAM   = Memory.AsMemory(0xA000..0xC000);
            WRAM   = Memory.AsMemory(0xC000..0xFF00);
            MMIO   = Memory.AsMemory(0xFF00..0xFF80);
            ZRAM   = Memory.AsMemory(0xFF80..0x10000);
        }

        public void Connect(Cartridge cartridge)
        {
            // This is super naive and does not accomodate for ROM size
            // and will become an issue with cartridges with a MBC
            this.cartridge = cartridge;
            this.cartridge.ROM.CopyTo(Memory, 0);
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
                    data = VRAM.Span[addr - 0x8000];
                    break;

                case var addr when (address >= 0xA000 && address < 0xC000):
                    data = ERAM.Span[addr - 0xA000];
                    break;

                case var addr when (address >= 0xC000 && address < 0xFF00):
                    data = WRAM.Span[addr - 0xC000];
                    break;

                case var addr when (address >= 0xFF00 && address < 0xFF80):
                    data = MMIO.Span[addr - 0xFF00];
                    break;

                case var addr when (address >= 0xFF80 && address <= 0xFFFF):
                    return ZRAM.Span[addr - 0xFF80];

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
                    Console.WriteLine("ROM0: Illegal memory write - ignoring.");
                    break;

                case var addr when (address >= 0x4000 && address <= 0x7FFF):
                    Console.WriteLine("ROM1: Illegal memory write - ignoring.");
                    break;

                case var addr when (address >= 0x8000 && address <= 0x9FFF):
                    VRAM.Span[addr - 0x8000] = value;
                    break;

                case var addr when (address >= 0xA000 && address <= 0xBFFF):
                    ERAM.Span[addr - 0xA000] = value;
                    break;

                case var addr when (address >= 0xC000 && address <= 0xDFFF):
                    WRAM.Span[addr - 0xC000] = value;
                    break;

                case var addr when (address >= 0xFF00 && address <= 0xFF7F):
                    switch (addr)
                    {
                        // Joypad
                        case 0xFF00:
                            break;

                        // Serial
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
                            DIV = 0; // Reset on any write
                            break;

                        // Sound Controller
                        case var _ when (address >= 0xFF10 && address <= 0xFF3F):
                            // DebugPrint("Unimplemented MMIO write", "Sound Controller", address, value);
                            MMIO.Span[addr - 0xFF00] = value;
                            break;

                        case 0xFF40:
                            LCDC = value;
                            break;

                        case 0xFF41:
                            STAT = value;
                            break;

                        case 0xFF42:
                            SCY = value;
                            break;

                        case 0xFF44:
                            LY = value;
                            break;

                        case 0xFF45:
                            LYC = value;
                            break;

                        // DMG Boot register
                        case 0xFF50:
                            BOOT = value;
                            break;
 
                        default:
                            // Pass through
                            MMIO.Span[addr - 0xFF00] = value;
                            break;
                    }

                    break;

                case var addr when (address >= 0xFF80 && address <= 0xFFFF):
                    ZRAM.Span[addr - 0xFF80] = value;
                    break;

                default:
                    break;
                    //throw new Exception($"Illegal memory access: {address:X2}");
            }
        }

        public void WriteWord(ushort address, ushort value)
        {
            WriteByte(address,               (byte) (value & 0x00FF));
            WriteByte((ushort)(address + 1), (byte)((value & 0xFF00) >> 8));
        }

        public void InitializeMBC(Cartridge cartridge)
        {

        }
    }
}
