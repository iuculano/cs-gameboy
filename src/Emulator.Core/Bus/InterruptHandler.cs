using Emulator.Core.Bus;

namespace Enulator.Core.Bus;

// Feel a little odd about this belonging to the bus
public class InterruptHandler
{
    private readonly MemoryBus memory;

    public InterruptHandler(MemoryBus memory)
    {
        this.memory = memory;
    }

    public InterruptType PendingInterrupts
    {
        get => (InterruptType)(memory.IE & memory.IF);
    }

    public bool IsServiceable(InterruptType interruptType)
    {
        return PendingInterrupts.HasFlag(interruptType);
    }

    public void Request(InterruptType type)
    {
        memory.IF |= (byte)type;
    }

    public void Clear(InterruptType type)
    {
        memory.IF &= (byte)~type;
    }
}
