namespace axGB.CPU
{
    public partial class InteruptHandler
    {
        private Processor processor
        {
            get; init;
        }

        public InteruptHandler(Processor processor)
        {
            this.processor = processor;
        }

        public bool IME          { get; set; }
        public bool NeedsEIDelay { get; set; }
        public bool IsInterruptRequested
        {
            get
            {
                return processor.memory.IE != 0;
            }
        }

        

        /// <summary>
        /// Process pending interupts.
        /// </summary>
        public void ProcessInterupts()
        {
            if (IME == false || processor.memory.IE == 0)
            {
                return;
            }

            // Delay for one instruction after EI
            if (NeedsEIDelay)
            {
                NeedsEIDelay = false;
                return;
            }

            // VBlank
            if ((processor.memory.IF & 0b_00000001) > 0) 
            {
                // https://emudev.de/gameboy-emulator/interrupts-and-timers/
                var address = processor.registers.SP -= 2;
                processor.memory.WriteWord(address, processor.registers.PC);

                // VBlank interrupt vector
                processor.registers.PC = 0x0040;

                unchecked
                {
                    processor.memory.IF &= (byte)~(0b_00000001);
                }
            }

            // LCD STAT
            if ((processor.memory.IF & 0b_00000010) > 0)
            {

            }

            // Timer
            if ((processor.memory.IF & 0b_00000100) > 0)
            {

            }

            // Serial
            if ((processor.memory.IF & 0b_00001000) > 0)
            {

            }

            // Joypad
            if ((processor.memory.IF & 0b_00010000) > 0)
            {

            }
        }
    }
}