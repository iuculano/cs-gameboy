using System;

namespace axGB.CPU
{
    public partial class InstructionSet
    {
        private bool HasFlags(Flags flags)
        {
            return (processor.registers.F & (byte)flags) == (byte)flags;
        }

        private void SetFlags(Flags flags, bool enabled)
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

        public void ClearFlags(Flags flags)
        {
            unchecked
            {
                processor.registers.F &= (byte)~(flags);
            }
        }
    }
}

