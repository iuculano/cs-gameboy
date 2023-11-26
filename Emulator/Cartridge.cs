using System;
using System.IO;
using System.Text;
using System.Runtime.CompilerServices;

namespace axGB.System
{
    // https://gbdev.io/pandocs/The_Cartridge_Header.html
    public struct CartridgeHeader
    {
        public byte[]          NintendoLogo     { get; init; }
        public string          Title            { get; init; }
        public byte[]          ManufacturerCode { get; init; }
        public byte            CGBFlag          { get; init; }
        public string          NewLicenseeCode  { get; init; }
        public byte            SGBFlag          { get; init; }
        public CartridgeType   CartridgeType    { get; init; }
        public ROMSize         ROMSize          { get; init; }
        public RAMSize         RAMSize          { get; init; }
        public DestinationCode DestinationCode  { get; init; }
        public byte            OldLicenseeCode  { get; init; }
        public byte            ROMVersion       { get; init; }
    }

    public abstract class Cartridge
    {
        // Not sure I like how I handle this - ROM here is really just temporary storage
        // It's really copied over to the memory that the system bus owns, and the
        // Read/Write functions below interact with that
        public  byte[]          ROM    { get; init; }
        private MemoryBus       memory { get; init; }
        public  CartridgeHeader Header { get; init; }


        public static Cartridge Load(string filename, MemoryBus memory)
        {
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
                    return new CartridgeNoMBC()
                    {
                        ROM    = buffer,
                        Header = header,
                        memory = memory
                    };

                case CartridgeType.MBC1:
                    return new CartridgeMBC1()
                    {
                        ROM    = buffer,
                        Header = header,
                        memory = memory
                    };

                default:
                    throw new Exception("Unknown cartridge type.");
            }
        }

        // Just a very simple passthrough for read and writes to ROM.
        // Not sure I like having such hot & light functions be virtual...
        public virtual byte ReadByte(ushort address)
        {
            return memory.Memory[address];
        }

        public virtual void WriteByte(ushort address, byte value)
        {
            memory.Memory[address] = value;
        }
    }
}
