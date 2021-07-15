namespace axGB.System
{
    public partial class MemoryBus
    {
        // Memory mapped registers - this is mostly for convenience

        // Whether we're still executing the on-chip boot rom
        public byte BOOT
        {
            get => MMIO.Span[0x50];
            set => MMIO.Span[0x50] = value;
        }


        // Controller
        public byte JOYP
        {
            get => MMIO.Span[0x00];
            set => MMIO.Span[0x00] = value;
        }


        // Serial bus
        public byte SB
        {
            get => MMIO.Span[0x01];
            set => MMIO.Span[0x01] = value;
        }

        public byte SC
        {
            get => MMIO.Span[0x02];
            set => MMIO.Span[0x02] = value;
        }


        // Timer and Divider
        public byte DIV
        {
            get => MMIO.Span[0x04];
            set => MMIO.Span[0x04] = value;
        }

        public byte TIMA
        {
            get => MMIO.Span[0x05];
            set => MMIO.Span[0x05] = value;
        }

        public byte TMA
        {
            get => MMIO.Span[0x06];
            set => MMIO.Span[0x06] = value;
        }

        public byte TAC
        {
            get => MMIO.Span[0x04];
            set => MMIO.Span[0x04] = value;
        }


        // LCD
        public byte LCDC
        {
            get => MMIO.Span[0x40];
            set => MMIO.Span[0x40] = value;
        }

        public byte STAT
        {
            get => MMIO.Span[0x41];
            set => MMIO.Span[0x41] = value;
        }

        public byte SCY
        {
            get => MMIO.Span[0x42];
            set => MMIO.Span[0x42] = value;
        }

        public byte SCX
        {
            get => MMIO.Span[0x43];
            set => MMIO.Span[0x43] = value;
        }

        public byte LY
        {
            get => MMIO.Span[0x44];
            set => MMIO.Span[0x44] = value;
        }

        public byte LYC
        {
            get => MMIO.Span[0x45];
            set => MMIO.Span[0x45] = value;
        }

        public byte WY
        {
            get => MMIO.Span[0x4A];
            set => MMIO.Span[0x4A] = value;
        }

        public byte WX
        {
            get => MMIO.Span[0x4B];
            set => MMIO.Span[0x4B] = value;
        }

        public byte BGP
        {
            get => MMIO.Span[0x4B];
            set => MMIO.Span[0x4B] = value;
        }


        // Interupts
        public byte IE
        {
            get => ZRAM.Span[0x7F];
            set => ZRAM.Span[0x7F] = value;
        }
        
        public byte IF
        {
            get => MMIO.Span[0x0F];
            set => MMIO.Span[0x0F] = value;
        }
    }
}
