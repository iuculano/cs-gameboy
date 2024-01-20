using Emulator.Core.Bus;

namespace Enulator.Core.Bus;

// Feel a little odd about this belonging to the bus
public class InterruptHandler
{
    private readonly MemoryBus memory;

    public InterruptType PendingInterrupts
    { 
        get => (InterruptType)(memory.IE & memory.IF);
    }

    public InterruptHandler(MemoryBus memory)
    {
        this.memory = memory;
    }

    public void Request(InterruptType type)
    {
        memory.IF |= (byte)~type;
    }

    public void Clear(InterruptType type)
    {
        memory.IF &= (byte)~type;
    }
}
