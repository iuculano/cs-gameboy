namespace Enulator.Core.CPU;

public partial class InstructionSet
{
    private void Push(ushort register)
    {
        var address = processor.registers.SP -= 2;
        processor.memory.WriteWord(address, register);
    }

    private ushort Pop()
    {
        var data                = processor.memory.ReadWord(processor.registers.SP);
        processor.registers.SP += 2;

        return data;
    }
}
