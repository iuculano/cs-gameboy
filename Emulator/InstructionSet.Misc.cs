using System;


namespace axGB.CPU
{
    public partial class InstructionSet
    {
        private void Scf()
        {
            SetFlags(Flags.Carry,     true);
            SetFlags(Flags.Subtract,  false);
            SetFlags(Flags.HalfCarry, false);
        }

        private void Ccf()
        {
            SetFlags(Flags.Carry,     !HasFlags(Flags.Carry));
            SetFlags(Flags.Subtract,  false);
            SetFlags(Flags.HalfCarry, false);
        }

        private void Ei()
        {
            processor.interruptHandler.IME          = true;
            processor.interruptHandler.NeedsEIDelay = true;
        }

        private void Di()
        {
            processor.interruptHandler.IME = false;
        }

        private void Halt()
        {
            processor.isHalted      = true;
            processor.registers.PC += 1;
        }
    }
}
