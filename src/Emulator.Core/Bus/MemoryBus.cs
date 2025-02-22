using System;
using System.IO;
using System.Text;
using Enulator.Core.Cartridge;

namespace Emulator.Core.Bus;

public partial class MemoryBus
{
    // The basic memory map
    // ROM0, ROM1, and ERAM live on the catridge
    // https://gbdev.io/pandocs/Memory_Map.html
    public byte[] Bootstrap { get; init; }
    public byte[] VRAM      { get; init; }
    public byte[] WRAM      { get; init; }
    public byte[] OAM       { get; init; }
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
        OAM  = new byte[0xA0];   // 0xFE00 - 0xFE9F
        MMIO = new byte[0x80];   // 0xFF00 - 0xFF7F
        HRAM = new byte[0x80];   // 0xFF80 - 0xFFFE

        // Set a few default values on bootup based on BGB
        JOYP = 0xCF;
        SC   = 0x7E;
        IF   = 0xE1;
        TAC  = 0xF8;
        STAT = 0x84;
    }

    public void Connect(Cartridge cartridge)
    {
        this.cartridge = cartridge;
    }


    public byte ReadByte(ushort address)
    {
        // No idea why storing the byte first insead of just return seems
        // to get slightly better code gen
        byte data;
        switch (address) 
        {
            case var addr when (address <= 0x3FFF):
                data = (BOOT == 0 && addr <= 0xFF) ? Bootstrap[addr] : cartridge.ReadByte(addr);
                break;

            case var addr when (address <= 0x7FFF):
                data = cartridge.ReadByte(addr);
                break;

            case var addr when (address <= 0x9FFF):
                data = VRAM[addr - 0x8000];
                break;

            case var addr when (address <= 0xBFFF):
                data = cartridge.ReadByte(addr);
                break;

            case var addr when (address <= 0xDFFF):
                data = WRAM[addr - 0xC000];
                break;

            case var addr when (address <= 0xFDFF): // ECHO
                data = WRAM[addr - 0xE000];
                break;

            case var addr when (address <= 0xFE9F):
                data = OAM[addr - 0xFE00];
                break;

            case var addr when (address <= 0xFEFF):
                // I guess a read from here is just completely undefined?
                data = 0xFF;
                break;

            case var addr when (address <= 0xFF7F):
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

            case var addr when (address <= 0xFFFF):
                return HRAM[addr - 0xFF80];

            default:
                throw new Exception($"Illegal memory read: {address}");
        }

        return data;
    }

    public ushort ReadWord(ushort address)
    {
        Span<byte> bytes = stackalloc byte[2];
        bytes[0] = ReadByte(address);
        bytes[1] = ReadByte((ushort)(address + 1));

        return BitConverter.ToUInt16(bytes);
    }

    public void WriteByte(ushort address, byte value)
    {
        switch (address) 
        {
            case var addr when (address <= 0x7FFF):
                cartridge.WriteByte(addr, value);
                break;

            case var addr when (address <= 0x9FFF):
                VRAM[addr - 0x8000] = value;
                break;

            case var addr when (address <= 0xBFFF):
                cartridge.WriteByte(addr, value);
                break;

            case var addr when (address <= 0xDFFF): // Looks like this plays a part in CGB
                WRAM[addr - 0xC000] = value;        // Just ignore that for now...
                break;

            case var addr when (address <= 0xFDFF): // This seemingly shouldn't happen, but it
                WRAM[addr - 0xE000] = value;        // seems writes do succeed?
                break;

            case var addr when (address <= 0xFE9F):
                OAM[addr - 0xFE00] = value;
                break;

            case var addr when (address <= 0xFEFF): // Presumably a no-op
                break;

            case var addr when (address <= 0xFF7F):
                switch (addr)
                {
                    // Serial - intercept this for debug purposes
                    case 0xFF02:
                    {
                        // Deviation from this value seems to be a sign the
                        // transfer is complete
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

            case var addr when (address <= 0xFFFF):
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
