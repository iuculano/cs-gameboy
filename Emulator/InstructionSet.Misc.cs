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


            var opcode = processor.memory.ReadByte(processor.registers.PC);
            var length = Opcodes[opcode].OperandLength + 1;
            Push((ushort)(processor.registers.PC + length));
        }
    }
}
