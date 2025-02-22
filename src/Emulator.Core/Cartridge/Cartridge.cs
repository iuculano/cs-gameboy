using System;
using System.IO;
using System.Text;
using Emulator.Core.Bus;

namespace Enulator.Core.Cartridge;

public abstract class Cartridge
{
    public    CartridgeHeader Header { get; init; }
    protected byte[]          rom    { get; init; }
    protected MemoryBus       memory { get; init; }

    public static Cartridge Load(string filename, MemoryBus memory)
    {
        // https://gbdev.io/pandocs/The_Cartridge_Header.html
        var buffer = File.ReadAllBytes(filename);
        var header = new CartridgeHeader
        {
            NintendoLogo     = buffer[0x0104..0x0134],
            Title            = Encoding.ASCII.GetString(buffer[0x0134..0x0144]).TrimEnd("\0".ToCharArray()),
            ManufacturerCode = buffer[0x013F..0x0143],
            CGBFlag          = buffer[0x0143],
            NewLicenseeCode  = Encoding.ASCII.GetString(buffer[0x0144..0x0146]).TrimEnd("\0".ToCharArray()),
            CartridgeType    = (CartridgeType)buffer[0x0147],
            ROMSize          = (ROMSize)buffer[0x0148],
            RAMSize          = (RAMSize)buffer[0x0149],
            DestinationCode  = (DestinationCode)buffer[0x014A],
            OldLicenseeCode  = buffer[0x014B],
            ROMVersion       = buffer[0x014C]
        };

        switch (header.CartridgeType)
        {
            case CartridgeType.RomOnly:
                return new CartridgeNoMBC
                {
                    rom    = buffer,
                    Header = header,
                    memory = memory
                };

            case CartridgeType.MBC1:
            case CartridgeType.MBC1Ram:
            case CartridgeType.MBC1RamBattery:
                return new CartridgeMBC1
                {
                    rom    = buffer,
                    Header = header,
                    memory = memory
                };

            case CartridgeType.MBC3:
            case CartridgeType.MBC3Ram:
            case CartridgeType.MBC3RamBattery:
                return new CartridgeMBC3
                {
                    rom    = buffer,
                    Header = header,
                    memory = memory
                };

            default:
                throw new Exception("Unknown or unsupported cartridge type.");
        }
    }

    public abstract byte ReadByte (ushort address);
    public abstract void WriteByte(ushort address, byte value);
}
