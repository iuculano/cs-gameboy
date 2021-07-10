using axGB.CPU;
using axGB.System;

namespace axGB
{
    class Program
    {
        static void Main(string[] args)
        {
            var memory    = new MemoryBus();
            var processor = new Processor(memory);
            var graphics  = new GraphicsProcessor(memory);
            var cartridge = Cartridge.Load(@"09-op r,r.gb", memory);  // 4, 5, 6

            memory.Connect(cartridge);

            while(true)
            {
                var cycles          = 0;
                var cyclesThisFrame = 0;

                while (cyclesThisFrame < Processor.CyclesPerFrame)
                {                    
                    cycles = processor.Update();
                    graphics.Update(cycles);

                    cyclesThisFrame += cycles;
                }

                // Thread.Sleep(16);
            }
        }
    }
}
