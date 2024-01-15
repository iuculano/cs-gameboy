using Emulator.Core.CPU;

namespace Enulator.Core.CPU;


public class InterruptHandler
{

    public InterruptHandler(Processor processor)
    {
        this.processor = processor;

        processor.memory.IF = 0b_11100001;
    }

    private Processor processor
    {
        get;
    }

    public bool IME          { get; set; }
    public bool NeedsEIDelay { get; set; }

    private void CallInterruptVector(ushort vector, InterruptType requestFlag)
    {
        // Will be re-enabled by RETI
        IME = false;

        // Clear HALT status since we're processing an interrupt
        processor.isHalted = false;

        // Like the Call instruction
        var address = processor.registers.SP -= 2;
        processor.memory.WriteWord(address, processor.registers.PC);
        processor.registers.PC = vector;

        // Clear the appropriate interrupt flag
        unchecked
        {
            processor.memory.IF &= (byte)~requestFlag;
        }
    }

    /// <summary>
    /// Process pending Interrupts.
    /// </summary>
    public void ProcessInterrupts()
    {
        // https://gbdev.io/pandocs/Interrupts.html

        // Get interrupts that are both enabled and signaled
        var pendingInterrupts = (InterruptType)(processor.memory.IE & processor.memory.IF);

        // Need to exit the HALT state even if IME is false
        if (pendingInterrupts != 0)
        {
            processor.isHalted = false;
        }

        // Don't service interrupts if IME is false
        if (IME == false)
        {
            // Handle the HALT bug - IME is disabled and there's a pending
            // interrupt. PC should fail to increment in the CPU
            if (processor.isHalted && pendingInterrupts != 0)
            {
                processor.isHaltBug = true;
            }

            return;
        }

        // Delay for one instruction after EI, IME is guaranteed to be true
        // if this is set
        if (NeedsEIDelay)
        {
            NeedsEIDelay = false;
            return;
        }

        // Bit      : 7 | 6 | 5 | 4	     | 3      | 2     | 1   | 0
        // Interrupt: X | X | X | Joypad | Serial | Timer | LCD | VBlank
        // VBlank
        if (pendingInterrupts.HasFlag(InterruptType.VBlank))
        {
            CallInterruptVector(0x0040, InterruptType.VBlank);
            return;
        }

        // LCD STAT
        if (pendingInterrupts.HasFlag(InterruptType.LCD))
        {
            CallInterruptVector(0x0048, InterruptType.LCD);
            return;
        }

        // Timer
        if (pendingInterrupts.HasFlag(InterruptType.Timer))
        {
            CallInterruptVector(0x0050, InterruptType.Timer);
            return;
        }

        // Serial
        if (pendingInterrupts.HasFlag(InterruptType.Serial))
        {
            CallInterruptVector(0x0058, InterruptType.Serial);
            return;
        }

        // Joypad
        if (pendingInterrupts.HasFlag(InterruptType.Joypad))
        {
            CallInterruptVector(0x0060, InterruptType.Joypad);
            return;
        }
    }
}
