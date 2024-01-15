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
        Console.WriteLine("Illegal memory write.");
    }
}
