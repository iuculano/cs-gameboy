using Enulator.Core.Bus;

namespace Emulator.Core.Bus;

public class Timer
{
    // https://gbdev.io/pandocs/Timer_and_Divider_Registers.html

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

    private bool IsEnabled
    {
        get => (memory.TAC & 0b_00000100) == 0b_00000100;
    }

    private int Frequency
    {
        get => tacFrequencyTable[memory.TAC & 0b_00000011];
    }

    public void Update(int cycles)
    {
        divAccumulator  += cycles;
        timaAccumulator += cycles;

        // DIV always ticks regardless of TAC's state
        // 4194304 cycles / 16384 = 256, Divider updates every 256 cycles
        if (divAccumulator >= 256)
        {
            memory.DIV++;

            // There's no guarantee we have churned exactly 256 cycles - simply
            // setting this to 0 can be wrong timing wise
            divAccumulator -= 256;
        }

        // Check if the timer control is actually enabled
        if (IsEnabled)
        {
            // Update TIMA register
            if (timaAccumulator >= Frequency)
            {
                // Check for overflow
                var pendingValue = memory.TIMA + 1;
                if (pendingValue > 0xFF)
                {
                    memory.TIMA = memory.TMA;
                    interruptHandler.Request(InterruptType.Timer);
                }

                else
                {
                    memory.TIMA++;
                }

                timaAccumulator -= Frequency;
            }
        }
    }
}
