using Enulator.Core.Bus;

namespace Emulator.Core.Bus;

public class Timer
{
    private readonly MemoryBus        memory;
    private readonly InterruptHandler interruptHandler;
    private readonly int[]            tacFrequencyTable = { 1024, 16, 64, 256 };

    private int divAccumulator;
    private int timaAccumulator;

    public Timer(MemoryBus memory, InterruptHandler interruptHnadler)
    {
        this.memory           = memory;
        this.interruptHandler = interruptHnadler;
    }

    public void Update(int cycles)
    {
        // https://gbdev.io/pandocs/Timer_and_Divider_Registers.html
        divAccumulator  += cycles;
        timaAccumulator += cycles;

        // Update DIV - note that it always ticks regardless of TAC's state
        // 4194304 cycles / 16384 = 256, Divider updates every 256 cycles
        if (divAccumulator >= 256)
        {
            // Directly increment this register without writing through the
            // memory bus anything else trying to write to that register should
            // simply set it to 0
            memory.DIV++;

            // Don't blindly set divAccumulator to zero, there's no guarantee we
            // have moved exactly 256 cycles - divAccumulator may be something
            // like 260 for example and DIV would wind up incrementing at the
            // wrong pace
            divAccumulator -= 256;
        }

        // Check if the timer control is actually enabled
        if ((memory.TAC & 0b_00000100) == 0b_00000100)
        {
            // Update TIMA register
            // The frequency of which it's updated depends on what's in the TAC
            // register
            var frequency = tacFrequencyTable[memory.TAC & 0b_00000011];

            // If we've consumed enough cycles to tick
            if (timaAccumulator >= frequency)
            {
                var pendingValue = memory.TIMA + 1; // Check for overflow
                if (pendingValue > 0xFF)
                {
                    memory.TIMA = memory.TMA;
                    interruptHandler.Request(InterruptType.Timer);
                }

                else
                {
                    memory.TIMA++;
                }

                timaAccumulator -= frequency;
            }
        }
    }
}
