using axGB.CPU;
using System;

namespace axGB.System
{
    public class Timer
    {
        private MemoryBus memory;
        private int       divAccumulator;
        private int       timaAccumulator;
        private int[]     tacFrequencyTable = { 1024, 16, 64, 256 };

        public Timer(MemoryBus memory)
        {
            this.memory = memory;
        }


        public void Update(int cycles)
        {
            // https://gbdev.io/pandocs/Timer_and_Divider_Registers.html

            divAccumulator  += cycles;
            timaAccumulator += cycles;
            
            // Update DIV - note that it always ticks
            // 4194304 cycles / 16384 = 256, Divider updates every 256 cycles
            if (divAccumulator >= 255)
            {
                // Increment this outside of the memory bus because writes to this register
                // through it will reset this register to 0
                memory.DIV++;
                divAccumulator -= 256;
            }

            // Check if the timer control is actually enabled
            if ((memory.TAC & 0b_00000100) > 0)
            {
                // Update TIMA register
                // The frequency of which it's updated depends on what's in the TAC register
                var frequency = tacFrequencyTable[memory.TAC & 0b_00000011];

                if (timaAccumulator >= frequency - 1)
                {
                    var pendingValue = memory.TIMA + 1; // Check for overflow
                    if (pendingValue > 0xFF)
                    {
                        memory.TIMA  = memory.TMA;
                        memory.IF   |= 0b_00000100;
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
}
