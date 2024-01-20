using Emulator.Core.CPU;

namespace Enulator.Core.CPU;

public partial class InstructionSet
{
    private void Scf()
    {
        SetFlags(CPUFlags.Carry,     true);
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
        processor.IME          = true;
        processor.NeedsEIDelay = true;
    }

    private void Di()
    {
        processor.IME = false;
    }

    private void Halt()
    {
        processor.isHalted = true;
    }
}
