namespace axGB.CPU
{
    public class InterruptHandler
    {

        public InterruptHandler(Processor processor)
        {
            this.processor = processor;
        }

        private Processor processor
        {
            get;
        }

        public bool IME          { get; set; }
        public bool NeedsEIDelay { get; set; }

        private void JumpToInterruptVector(ushort vector, byte requestFlag)
        {
            // https://gbdev.io/pandocs/Interrupts.html
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
        ///     Process pending Interrupts.
        /// </summary>
        public void ProcessInterrupts()
        {
            // https://gbdev.io/pandocs/Interrupts.html

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

            processor.isHalted = false;

            // Bit      : 7 | 6 | 5 | 4	     | 3      | 2     | 1   | 0
            // Interrupt: X | X | X | Joypad | Serial | Timer | LCD | VBlank

            // Get interrupts are both enabled and signaled
            var pendingInterrupts = (byte)(processor.memory.IF & processor.memory.IE);

            // VBlank
            if ((pendingInterrupts & 0b_00000001) > 0)
            {
                JumpToInterruptVector(0x0040, 0b_00000001);
                return;
            }

            // LCD STAT
            if ((pendingInterrupts & 0b_00000010) > 0)
            {
                JumpToInterruptVector(0x0048, 0b_00000010);
                return;
            }

            // Timer
            if ((pendingInterrupts & 0b_00000100) > 0)
            {
                JumpToInterruptVector(0x0050, 0b_00000100);
                return;
            }

            // Serial
            if ((pendingInterrupts & 0b_00001000) > 0)
            {
                JumpToInterruptVector(0x0058, 0b_00001000);
                return;
            }

            // Joypad
            if ((pendingInterrupts & 0b_00010000) > 0)
            {
                JumpToInterruptVector(0x0060, 0b_00010000);
            }
        }
    }
}
