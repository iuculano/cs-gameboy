namespace Emulator.Core.Bus;

public partial class MemoryBus
{
    // Memory mapped registers - this is mostly for convenience

    // Whether we're still executing the on-chip boot rom
    public byte BOOT
    {
        get => MMIO[0x50];
        set => MMIO[0x50] = value;
    }

    // Controller
    public byte JOYP
    {
        get => MMIO[0x00];
        set => MMIO[0x00] = value;
    }

    // Serial bus
    public byte SB
    {
        get => MMIO[0x01];
        set => MMIO[0x01] = value;
    }

    public byte SC
    {
        get => MMIO[0x02];
        set => MMIO[0x02] = value;
    }

    // Timer and Divider
    public byte DIV
    {
        get => MMIO[0x04];
        set => MMIO[0x04] = value;
    }

    public byte TIMA
    {
        get => MMIO[0x05];
        set => MMIO[0x05] = value;
    }

    public byte TMA
    {
        get => MMIO[0x06];
        set => MMIO[0x06] = value;
    }

    public byte TAC
    {
        get => MMIO[0x07];
        set => MMIO[0x07] = value;
    }


    // LCD
    public byte LCDC
    {
        get => MMIO[0x40];
        set => MMIO[0x40] = value;
    }

    public byte STAT
    {
        get => MMIO[0x41];
        set => MMIO[0x41] = value;
    }

    public byte SCY
    {
        get => MMIO[0x42];
        set => MMIO[0x42] = value;
    }

    public byte SCX
    {
        get => MMIO[0x43];
        set => MMIO[0x43] = value;
    }

    public byte LY
    {
        get => MMIO[0x44];
        set => MMIO[0x44] = value;
    }

    public byte LYC
    {
        get => MMIO[0x45];
        set => MMIO[0x45] = value;
    }

    public byte BGP
    {
        get => MMIO[0x47];
        set => MMIO[0x47] = value;
    }

    public byte WY
    {
        get => MMIO[0x4A];
        set => MMIO[0x4A] = value;
    }

    public byte WX
    {
        get => MMIO[0x4B];
        set => MMIO[0x4B] = value;
    }

    // Interupts
    public byte IE
    {
        get => HRAM[0x7F];
        set => HRAM[0x7F] = (byte)(0b_11100000 | value); // 3 MSB are always set
    }
    
    public byte IF
    {
        get => MMIO[0x0F];
        set => MMIO[0x0F] = (byte)(0b_11100000 | value); // 3 MSB are always set
    }
}
