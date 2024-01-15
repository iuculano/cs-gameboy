using System;
using Emulator.Core.Bus;

namespace Enulator.Core.CPU;

/// <summary>
///     The GameBoy processor.
/// </summary>
public class Processor
{
    // https://en.wikipedia.org/wiki/Game_Boy
    public const int ClockSpeed     = 4194304;
    public const int CyclesPerFrame = (int)(ClockSpeed / 59.7275f);

    internal int  cycles    = 0;
    internal bool isHalted  = false;
    internal bool isHaltBug = false;

    internal MemoryBus        memory;
    internal Registers        registers;
    internal InterruptHandler interruptHandler;
    internal InstructionSet   instructionSet;
    
    public Processor(MemoryBus memory)
    {
        this.memory      = memory;
        registers        = new Registers();
        interruptHandler = new InterruptHandler(this);
        instructionSet   = new InstructionSet(this);
    }

    /// <summary>
    /// Executes a frame's worth of instructions.
    /// </summary>
    /// <returns></returns>
    public int Update()
    {
        interruptHandler.ProcessInterrupts();
        return StepInstruction();
    }

    /// <summary>
    /// Steps the CPU by executing the next instruction in the stream.
    /// </summary>
    /// <returns>The last opcode that was executed.</returns>
    public int StepInstruction()
    {
        if (isHalted)
        {
            return 0;
        }


        if (isHaltBug)
        {
            // PC shouldn't move if we've triggered this, so just move it back
            registers.PC -= 1;
        }

        int opcode = memory.ReadByte(registers.PC);
        if (opcode == 0xCB)
        {
            /*
                All the prefixed instructions have a operand length of 1 - need to
                grab the next byte after PC to get to the appropriate instruction.
                Don't actually advance the PC register here, let the operand length 
                drive it further down the line when the instruction itself executes.

                Internally, it's all one large lookup table. The 2nd instruction set
                can be accessed just by adding 256 to the opcode.
            */

            opcode = 256 + memory.ReadByte((ushort)(registers.PC + 1));
        }

        var instruction = instructionSet.Opcodes[opcode];
        var disassembly = "";
        switch (instruction.OperandLength)
        {
            case 1:
            {
                var func      = (Action<byte>)instruction.Function;
                var operand   = memory.ReadByte(++registers.PC);
                disassembly   = String.Format(instruction.Disassembly, operand);
                registers.PC += 1;

                func.Invoke(operand);
                break;
            }

            case 2:
            {
                var func      = (Action<ushort>)instruction.Function;
                var operand   = memory.ReadWord(++registers.PC);
                disassembly   = String.Format(instruction.Disassembly, operand);
                registers.PC += 2;

                func.Invoke(operand);
                break;
            }

            default:
            {
                var func      = (Action)instruction.Function;
                disassembly   = instruction.Disassembly;
                registers.PC += 1;

                func.Invoke();
                break;
            }
        }

        //Console.WriteLine(String.Format($"{disassembly}".PadRight(24) + $"{registers.ToString()}"));

        // Conditional cycles are handled within the instruction helpers and add to this
        var executedCycles = cycles + instruction.Cycles;
        cycles             = 0;

        return executedCycles;
    }
}
