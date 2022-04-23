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
            processor.interuptHandler.IME          = true;
            processor.interuptHandler.NeedsEIDelay = true;
        }

        private void Di()
        {
            processor.interuptHandler.IME = false;
        }

        private void Halt()
        {
            processor.isHalted      = true;
            processor.registers.PC += 1;
        }
    }
}
