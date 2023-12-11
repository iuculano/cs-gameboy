using System;

namespace axGB.CPU
{
    public class Instruction
    {
        public Instruction(string disassembly, int operandLength, int cycles, Delegate function)
        {
            Disassembly   = disassembly;
            OperandLength = operandLength;
            Cycles        = cycles;
            Function      = function;
        }


        public string Disassembly
        {
            get;
            init;
        }

        public int OperandLength
        {
            get;
            init;
        }

        public int Cycles
        {
            get;
            init;
        }

        public Delegate Function
        {
            get;
            init;
        }
    }
}
