using System;
using System.IO;
using System.Diagnostics;

namespace axGB.System
{
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
}
