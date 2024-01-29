using Emulator.Core.CPU;

namespace Enulator.Core.CPU;

public partial class InstructionSet
{
    private bool HasFlags(CPUFlags flags)
    {
        return (processor.registers.F & (byte)flags) == (byte)flags;
    }

    private void SetFlags(CPUFlags flags, bool enabled)
    {
        if (enabled)
        {
            processor.registers.F |= (byte)flags;
        }

        else
        {
            unchecked
            {
                processor.registers.F &= (byte)~(flags);
            }
        }
    }

    public void ClearFlags(CPUFlags flags)
    {
        unchecked
        {
            processor.registers.F &= (byte)~(flags);
        }
    }
}
