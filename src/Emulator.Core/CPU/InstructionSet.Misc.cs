using Emulator.Core.CPU;

namespace Enulator.Core.CPU;

public partial class InstructionSet
{
    private void Scf()
    {
        SetFlags(CPUFlags.Carry,      true);
        SetFlags(CPUFlags.Subtract,  false);
        SetFlags(CPUFlags.HalfCarry, false);
    }

    private void Ccf()
    {
        SetFlags(CPUFlags.Carry,     !HasFlags(CPUFlags.Carry));
        SetFlags(CPUFlags.Subtract,  false);
        SetFlags(CPUFlags.HalfCarry, false);
    }

    private void Ei()
    {
        processor.IMEPending = true;
    }

    private void Di()
    {
        processor.IME = false;
    }

    private void Halt()
    {
        // https://gbdev.io/pandocs/halt.html

        if (processor.IME)
        {
            processor.isHalted = true;
        }

        else
        {
            if (processor.interruptHandler.PendingInterrupts != 0)
            {
                // Halt bug
                processor.registers.PC -= 1;
                return;
            }

            processor.isHalted = true;
        }
    }
}
