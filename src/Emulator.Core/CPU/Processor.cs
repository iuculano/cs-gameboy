using System;
using System.Diagnostics;
using Emulator.Core.Bus;
using Enulator.Core.Bus;

namespace Enulator.Core.CPU;

/// <summary>
/// The GameBoy processor.
/// </summary>
public class Processor
{
    // https://en.wikipedia.org/wiki/Game_Boy
    public const int ClockSpeed     = 4194304;
    public const int CyclesPerFrame = (int)(ClockSpeed / 59.7275f);

    internal int  cycles    = 0;      // Current number of executed cycles
    internal bool isHalted  = false;  
    internal bool isHaltBug = false;

    internal MemoryBus        memory;
    internal Registers        registers;
    internal InterruptHandler interruptHandler;
    internal InstructionSet   instructionSet;

    public bool IME        { get; internal set; }
    public bool IMEPending { get; internal set; }

    public Processor(MemoryBus memory, InterruptHandler interruptHandler)
    {
        this.memory           = memory;
        this.interruptHandler = interruptHandler;
        registers             = new Registers();        
        instructionSet        = new InstructionSet(this);
    }

    /// <summary>
    /// Executes a frame's worth of instructions, optionally processing an
    /// interrupt.
    /// </summary>
    /// <returns>The number of cycles taken to step the processor.</returns>
    public int Update()
    {
        ProcessInterrupt();
        return StepInstruction();
    }

    private void CallInterruptVector(ushort vector, InterruptType requestFlag)
    {
        IME      = false; // Will be re-enabled by RETI        
        isHalted = false; // Exit HALT since we're processing an interrupt

        // Like the Call instruction
        var address = registers.SP -= 2;
        memory.WriteWord(address, registers.PC);
        registers.PC = vector;

        // Clear the interrupt flag that we handled
        interruptHandler.Clear(requestFlag);
    }

    /// <summary>
    /// Process pending Interrupts.
    /// </summary>
    private void ProcessInterrupt()
    {
        // https://gbdev.io/pandocs/Interrupts.html

        // Get interrupts that are both enabled and signaled
        // Need to exit the HALT state even if IME is false
        var pending = interruptHandler.PendingInterrupts;
        if (pending != 0)
        {
            isHalted = false;
        }

        // If this is set, EI was called and IME is trying to enable
        // It should be delayed by one instruction, so we set it here rather
        // than in EI
        if (IMEPending)
        {
            IMEPending = false;
            IME        = true;
            return;
        }

        // Don't service interrupts if IME is false - we should immediately
        // return to the CPU trying to execute the next instruction
        if (IME == false)
        {
            NeedsEIDelay = false;
            return;
        }

        // Bit      : 7 | 6 | 5 | 4	     | 3      | 2     | 1   | 0
        // Interrupt: X | X | X | Joypad | Serial | Timer | LCD | VBlank
        // Only process one at a time - if we process an interrupt, just return
        if (pending.HasFlag(InterruptType.VBlank))
        {
            CallInterruptVector(0x0040, InterruptType.VBlank);
            return;
        }

        if (pending.HasFlag(InterruptType.LCD))
        {
            CallInterruptVector(0x0048, InterruptType.LCD);
            return;
        }

        if (pending.HasFlag(InterruptType.Timer))
        {
            CallInterruptVector(0x0050, InterruptType.Timer);
            return;
        }

        if (pending.HasFlag(InterruptType.Serial))
        {
            CallInterruptVector(0x0058, InterruptType.Serial);
            return;
        }

        if (pending.HasFlag(InterruptType.Joypad))
        {
            CallInterruptVector(0x0060, InterruptType.Joypad);
            return;
        }
    }

    /// <summary>
    /// Steps the CPU by executing the next instruction in the stream.
    /// </summary>
    /// <returns>The number of cycles consumed.</returns>
    private int StepInstruction()
    {
        if (isHalted)
        {
            return 0;
        }


        // Simply skip stepping the CPU when we're halted
        if (isHalted)
        {
            return 0;
        }

        int opcode = memory.ReadByte(registers.PC);

        // 0xCB isn't really an opcode, but rather a prefix saying we need to
        // use the 2nd instruction set
        if (opcode == 0xCB)
        {
            // All the prefixed instructions have a operand length of 1 - need
            // to grab the next byte after PC to get to the appropriate
            // instruction.
            //
            // Don't actually advance the PC register here, let the operand
            // length drive it further down the line when the instruction itself
            // executes.
            //
            // Internally, it's all one large lookup table. The 2nd instruction
            // set can be accessed just by adding 256 to the opcode.
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
                //disassembly   = String.Format(instruction.Disassembly, operand);
                registers.PC += 1;

                func.Invoke(operand);
                break;
            }

            case 2:
            {
                var func      = (Action<ushort>)instruction.Function;
                var operand   = memory.ReadWord(++registers.PC);
                //disassembly   = String.Format(instruction.Disassembly, operand);
                registers.PC += 2;

                func.Invoke(operand);
                break;
            }

            default:
            {
                var func      = (Action)instruction.Function;
                //disassembly   = instruction.Disassembly;
                registers.PC += 1;

                func.Invoke();
                break;
            }
        }

        //Console.WriteLine(String.Format($"{disassembly}".PadRight(24) + $"{registers.ToString()}"));

        // Conditional cycles are handled within the instruction helpers and add
        // to this
        var executedCycles = cycles + instruction.Cycles;
        cycles             = 0;

        return executedCycles;
    }
}
