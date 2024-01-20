using System;

namespace Enulator.Core.Cartridge;

public sealed class CartridgeNoMBC : Cartridge
{
    public CartridgeNoMBC() : base()
    {

    }

    public override byte ReadByte(ushort address)
    {
        return rom[address];
    }

    public override void WriteByte(ushort address, byte value)
    {
        switch (address)
        {
            // RAM Enable register
            case var addr when (address <= 0x3FFF):
                Console.WriteLine($"ROM0: Illegal write to ${addr.ToString("X4")}");
                break;

            case var addr when (address <= 0x7FFF):
                Console.WriteLine($"ROM1: Illegal write to ${addr.ToString("X4")}");
                break;
        }
    }
}
