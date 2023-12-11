namespace axGB.CPU
{
    /// <summary>
    /// GameBoy register file.
    /// </summary>
    public class Registers
    {
        public byte   A;
        public byte   F;
        public byte   B;
        public byte   C;
        public byte   D;
        public byte   E;
        public byte   H;
        public byte   L;
        public ushort PC;
        public ushort SP;

        public ushort AF
        {
            get => (ushort)((A << 8) | F);
            set
            {
                A = (byte)(value >> 8);
                F = (byte)value;
            }
        }

        public ushort BC
        {
            get => (ushort)((B << 8) | C);
            set
            {
                B = (byte)(value >> 8);
                C = (byte)value;
            }
        }

        public ushort DE
        {
            get => (ushort)((D << 8) | E);
            set
            {
                D = (byte)(value >> 8);
                E = (byte)value;
            }
        }

        public ushort HL
        {
            get => (ushort)((H << 8) | L);
            set
            {
                H = (byte)(value >> 8);
                L = (byte)value;
            }
        }


        public override string ToString()
        {
            return $"A:{A:X2} F:{F:X2} B:{B:X2} C:{C:X2} "         +
                   $"D:{D:X2} E:{E:X2} H:{H:X2} L:{L:X2} "         + "| " +
                   $"AF:{AF:X4} BC:{BC:X4} DE:{DE:X4} HL:{HL:X4} " + "| " +
                   $"PC:{PC:X4} SP:{SP:X4}";
        }
    }
}
