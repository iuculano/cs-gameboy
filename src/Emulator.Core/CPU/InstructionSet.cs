using Emulator.Core.CPU;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Enulator.Core.CPU;

public partial class InstructionSet
{
    private Processor processor
    {
        get; init;
    }

    public List<Instruction> Opcodes = new List<Instruction>();

    public InstructionSet(Processor processor)
    {
        this.processor = processor;
        Initialize();
    }

    private void Initialize()
    {
        Opcodes = new List<Instruction>
        {
            new Instruction("NOP",                     0, 4,  OP_0x00),     // 0x00
            new Instruction("LD BC, ${0:x4}",          2, 12, OP_0x01),     // 0x01
            new Instruction("LD (BC), A",              0, 8,  OP_0x02),     // 0x02
            new Instruction("INC BC",                  0, 8,  OP_0x03),     // 0x03
            new Instruction("INC B",                   0, 4,  OP_0x04),     // 0x04
            new Instruction("DEC B",                   0, 4,  OP_0x05),     // 0x05
            new Instruction("LD B, ${0:x2}",           1, 8,  OP_0x06),     // 0x06
            new Instruction("RLCA",                    0, 4,  OP_0x07),     // 0x07
            new Instruction("LD (${0:x4}), SP",        2, 20, OP_0x08),     // 0x08
            new Instruction("ADD HL, BC",              0, 8,  OP_0x09),     // 0x09
            new Instruction("LD A, (BC)",              0, 8,  OP_0x0A),     // 0x0A
            new Instruction("DEC BC",                  0, 8,  OP_0x0B),     // 0x0B
            new Instruction("INC C",                   0, 4,  OP_0x0C),     // 0x0C
            new Instruction("DEC C",                   0, 4,  OP_0x0D),     // 0x0D
            new Instruction("LD C, ${0:x2}",           1, 8,  OP_0x0E),     // 0x0E
            new Instruction("RRCA",                    0, 4,  OP_0x0F),     // 0x0F
                                                                        
            new Instruction("STOP 0",                  1, 4,  OP_0xFF),     // 0x10
            new Instruction("LD DE, ${0:x4}",          2, 12, OP_0x11),     // 0x11
            new Instruction("LD (DE), A",              0, 8,  OP_0x12),     // 0x12
            new Instruction("INC DE",                  0, 8,  OP_0x13),     // 0x13
            new Instruction("INC D",                   0, 4,  OP_0x14),     // 0x14
            new Instruction("DEC D",                   0, 4,  OP_0x15),     // 0x15
            new Instruction("LD D, ${0:x2}",           1, 8,  OP_0x16),     // 0x16
            new Instruction("RLA",                     0, 4,  OP_0x17),     // 0x17
            new Instruction("JR {0:x2}",               1, 12, OP_0x18),     // 0x18
            new Instruction("ADD HL, DE",              0, 8,  OP_0x19),     // 0x19
            new Instruction("LD A, (DE)",              0, 8,  OP_0x1A),     // 0x1A
            new Instruction("DEC DE",                  0, 8,  OP_0x1B),     // 0x1B
            new Instruction("INC E",                   0, 4,  OP_0x1C),     // 0x1C
            new Instruction("DEC E",                   0, 4,  OP_0x1D),     // 0x1D
            new Instruction("LD E, ${0:x2}",           1, 8,  OP_0x1E),     // 0x1E
            new Instruction("RRA",                     0, 4,  OP_0x1F),     // 0x1F
                                                                        
            new Instruction("JR NZ, ${0:x2}",          1, 8,  OP_0x20),     // 0x20 - Conditional
            new Instruction("LD HL, ${0:x4}",          2, 12, OP_0x21),     // 0x21
            new Instruction("LD (HL+), A",             0, 8,  OP_0x22),     // 0x22
            new Instruction("INC HL",                  0, 8,  OP_0x23),     // 0x23
            new Instruction("INC H",                   0, 4,  OP_0x24),     // 0x24
            new Instruction("DEC H",                   0, 4,  OP_0x25),     // 0x25
            new Instruction("LD H, ${0:x2}",           1, 8,  OP_0x26),     // 0x26
            new Instruction("DAA",                     0, 4,  OP_0x27),     // 0x27
            new Instruction("JR Z, ${0:x2}",           1, 12, OP_0x28),     // 0x28
            new Instruction("ADD HL, HL",              0, 8,  OP_0x29),     // 0x29
            new Instruction("LD A, (HL+)",             0, 8,  OP_0x2A),     // 0x2A
            new Instruction("DEC HL",                  0, 8,  OP_0x2B),     // 0x2B
            new Instruction("INC L",                   0, 4,  OP_0x2C),     // 0x2C
            new Instruction("DEC L",                   0, 4,  OP_0x2D),     // 0x2D
            new Instruction("LD L, ${0:x2}",           1, 8,  OP_0x2E),     // 0x2E
            new Instruction("CPL",                     0, 4,  OP_0x2F),     // 0x2F
                                                                        
            new Instruction("JR NC, ${0:x2}",          1, 8,  OP_0x30),     // 0x30 - Conditional
            new Instruction("LD SP, ${0:x4}",          2, 12, OP_0x31),     // 0x31
            new Instruction("LD (HL-), A",             0, 8,  OP_0x32),     // 0x32
            new Instruction("INC SP",                  0, 8,  OP_0x33),     // 0x33
            new Instruction("INC (HL)",                0, 12, OP_0x34),     // 0x34
            new Instruction("DEC (HL)",                0, 12, OP_0x35),     // 0x35
            new Instruction("LD (HL), ${0:x2}",        1, 12, OP_0x36),     // 0x36
            new Instruction("SCF",                     0, 4,  OP_0x37),     // 0x37
            new Instruction("JR C, ${0:x2}",           1, 8,  OP_0x38),     // 0x38 - Conditional
            new Instruction("ADD HL, SP",              0, 8,  OP_0x39),     // 0x39
            new Instruction("LD A, (HL-)",             0, 8,  OP_0x3A),     // 0x3A
            new Instruction("DEC SP",                  0, 8,  OP_0x3B),     // 0x3B
            new Instruction("INC A",                   0, 4,  OP_0x3C),     // 0x3C
            new Instruction("DEC A",                   0, 4,  OP_0x3D),     // 0x3D
            new Instruction("LD A, ${0:x2}",           1, 8,  OP_0x3E),     // 0x3E
            new Instruction("CCF",                     0, 4,  OP_0x3F),     // 0x3F
                                                                        
            new Instruction("LD B, B",                 0, 4,  OP_0x40),     // 0x40
            new Instruction("LD B, C",                 0, 4,  OP_0x41),     // 0x41
            new Instruction("LD B, D",                 0, 4,  OP_0x42),     // 0x42
            new Instruction("LD B, E",                 0, 4,  OP_0x43),     // 0x43
            new Instruction("LD B, H",                 0, 4,  OP_0x44),     // 0x44
            new Instruction("LD B, L",                 0, 4,  OP_0x45),     // 0x45
            new Instruction("LD B, (HL)",              0, 8,  OP_0x46),     // 0x46
            new Instruction("LD B, A",                 0, 4,  OP_0x47),     // 0x47
            new Instruction("LD C, B",                 0, 4,  OP_0x48),     // 0x48
            new Instruction("LD C, C",                 0, 4,  OP_0x49),     // 0x49
            new Instruction("LD C, D",                 0, 4,  OP_0x4A),     // 0x4A
            new Instruction("LD C, E",                 0, 4,  OP_0x4B),     // 0x4B
            new Instruction("LD C, H",                 0, 4,  OP_0x4C),     // 0x4C
            new Instruction("LD C, L",                 0, 4,  OP_0x4D),     // 0x4D
            new Instruction("LD C, (HL)",              0, 8,  OP_0x4E),     // 0x4E
            new Instruction("LD C, A",                 0, 4,  OP_0x4F),     // 0x4F
                                                          
            new Instruction("LD D, B",                 0, 4,  OP_0x50),     // 0x50
            new Instruction("LD D, C",                 0, 4,  OP_0x51),     // 0x51
            new Instruction("LD D, D",                 0, 4,  OP_0x52),     // 0x52
            new Instruction("LD D, E",                 0, 4,  OP_0x53),     // 0x53
            new Instruction("LD D, H",                 0, 4,  OP_0x54),     // 0x54
            new Instruction("LD D, L",                 0, 4,  OP_0x55),     // 0x55
            new Instruction("LD D, (HL)",              0, 8,  OP_0x56),     // 0x56
            new Instruction("LD D, A",                 0, 4,  OP_0x57),     // 0x57
            new Instruction("LD E, B",                 0, 4,  OP_0x58),     // 0x58
            new Instruction("LD E, C",                 0, 4,  OP_0x59),     // 0x59
            new Instruction("LD E, D",                 0, 4,  OP_0x5A),     // 0x5A
            new Instruction("LD E, E",                 0, 4,  OP_0x5B),     // 0x5B
            new Instruction("LD E, H",                 0, 4,  OP_0x5C),     // 0x5C
            new Instruction("LD E, L",                 0, 4,  OP_0x5D),     // 0x5D
            new Instruction("LD E, (HL)",              0, 8,  OP_0x5E),     // 0x5E
            new Instruction("LD E, A",                 0, 4,  OP_0x5F),     // 0x5F
                                                                        
            new Instruction("LD H, B",                 0, 4,  OP_0x60),     // 0x60
            new Instruction("LD H, C",                 0, 4,  OP_0x61),     // 0x61
            new Instruction("LD H, D",                 0, 4,  OP_0x62),     // 0x62
            new Instruction("LD H, E",                 0, 4,  OP_0x63),     // 0x63
            new Instruction("LD H, H",                 0, 4,  OP_0x64),     // 0x64
            new Instruction("LD H, L",                 0, 4,  OP_0x65),     // 0x65
            new Instruction("LD H, (HL)",              0, 8,  OP_0x66),     // 0x66
            new Instruction("LD H, A",                 0, 4,  OP_0x67),     // 0x67
            new Instruction("LD L, B",                 0, 4,  OP_0x68),     // 0x68
            new Instruction("LD L, C",                 0, 4,  OP_0x69),     // 0x69
            new Instruction("LD L, D",                 0, 4,  OP_0x6A),     // 0x6A
            new Instruction("LD L, E",                 0, 4,  OP_0x6B),     // 0x6B
            new Instruction("LD L, H",                 0, 4,  OP_0x6C),     // 0x6C
            new Instruction("LD L, L",                 0, 4,  OP_0x6D),     // 0x6D
            new Instruction("LD L, (HL)",              0, 8,  OP_0x6E),     // 0x6E
            new Instruction("LD L, A",                 0, 4,  OP_0x6F),     // 0x6F
                                                                        
            new Instruction("LD (HL), B",              0, 8,  OP_0x70),     // 0x70
            new Instruction("LD (HL), C",              0, 8,  OP_0x71),     // 0x71
            new Instruction("LD (HL), D",              0, 8,  OP_0x72),     // 0x72
            new Instruction("LD (HL), E",              0, 8,  OP_0x73),     // 0x73
            new Instruction("LD (HL), H",              0, 8,  OP_0x74),     // 0x74
            new Instruction("LD (HL), L",              0, 8,  OP_0x75),     // 0x75
            new Instruction("HALT",                    0, 4,  OP_0x76),     // 0x76
            new Instruction("LD (HL), A",              0, 8,  OP_0x77),     // 0x77
            new Instruction("LD A, B",                 0, 4,  OP_0x78),     // 0x78
            new Instruction("LD A, C",                 0, 4,  OP_0x79),     // 0x79
            new Instruction("LD A, D",                 0, 4,  OP_0x7A),     // 0x7A
            new Instruction("LD A, E",                 0, 4,  OP_0x7B),     // 0x7B
            new Instruction("LD A, H",                 0, 4,  OP_0x7C),     // 0x7C
            new Instruction("LD A, L",                 0, 4,  OP_0x7D),     // 0x7D
            new Instruction("LD A, (HL)",              0, 8,  OP_0x7E),     // 0x7E
            new Instruction("LD A, A",                 0, 4,  OP_0x7F),     // 0x7F
                                                                        
            new Instruction("ADD A, B",                0, 4,  OP_0x80),     // 0x80
            new Instruction("ADD A, C",                0, 4,  OP_0x81),     // 0x81
            new Instruction("ADD A, D",                0, 4,  OP_0x82),     // 0x82
            new Instruction("ADD A, E",                0, 4,  OP_0x83),     // 0x83
            new Instruction("ADD A, H",                0, 4,  OP_0x84),     // 0x84
            new Instruction("ADD A, L",                0, 4,  OP_0x85),     // 0x85
            new Instruction("ADD A, (HL)",             0, 8,  OP_0x86),     // 0x86
            new Instruction("ADD A, A",                0, 4,  OP_0x87),     // 0x87
            new Instruction("ADC A, B",                0, 4,  OP_0x88),     // 0x88
            new Instruction("ADC A, C",                0, 4,  OP_0x89),     // 0x89
            new Instruction("ADC A, D",                0, 4,  OP_0x8A),     // 0x8A
            new Instruction("ADC A, E",                0, 4,  OP_0x8B),     // 0x8B
            new Instruction("ADC A, H",                0, 4,  OP_0x8C),     // 0x8C
            new Instruction("ADC A, L",                0, 4,  OP_0x8D),     // 0x8D
            new Instruction("ADC A, (HL)",             0, 8,  OP_0x8E),     // 0x8E
            new Instruction("ADC A, A",                0, 4,  OP_0x8F),     // 0x8F
                                                                        
            new Instruction("SUB B",                   0, 4,  OP_0x90),     // 0x90
            new Instruction("SUB C",                   0, 4,  OP_0x91),     // 0x91
            new Instruction("SUB D",                   0, 4,  OP_0x92),     // 0x92
            new Instruction("SUB E",                   0, 4,  OP_0x93),     // 0x93
            new Instruction("SUB H",                   0, 4,  OP_0x94),     // 0x94
            new Instruction("SUB L",                   0, 4,  OP_0x95),     // 0x95
            new Instruction("SUB (HL)",                0, 8,  OP_0x96),     // 0x96
            new Instruction("SUB A",                   0, 4,  OP_0x97),     // 0x97
            new Instruction("SBC A, B",                0, 4,  OP_0x98),     // 0x98
            new Instruction("SBC A, C",                0, 4,  OP_0x99),     // 0x99
            new Instruction("SBC A, D",                0, 4,  OP_0x9A),     // 0x9A
            new Instruction("SBC A, E",                0, 4,  OP_0x9B),     // 0x9B
            new Instruction("SBC A, H",                0, 4,  OP_0x9C),     // 0x9C
            new Instruction("SBC A, L",                0, 4,  OP_0x9D),     // 0x9D
            new Instruction("SBC A, (HL)",             0, 8,  OP_0x9E),     // 0x9E
            new Instruction("SBC A, A",                0, 4,  OP_0x9F),     // 0x9F
                                                                        
            new Instruction("AND B",                   0, 4,  OP_0xA0),     // 0xA0
            new Instruction("AND C",                   0, 4,  OP_0xA1),     // 0xA1
            new Instruction("AND D",                   0, 4,  OP_0xA2),     // 0xA2
            new Instruction("AND E",                   0, 4,  OP_0xA3),     // 0xA3
            new Instruction("AND H",                   0, 4,  OP_0xA4),     // 0xA4
            new Instruction("AND L",                   0, 4,  OP_0xA5),     // 0xA5
            new Instruction("AND (HL)",                0, 8,  OP_0xA6),     // 0xA6
            new Instruction("AND A",                   0, 4,  OP_0xA7),     // 0xA7
            new Instruction("XOR B",                   0, 4,  OP_0xA8),     // 0xA8
            new Instruction("XOR C",                   0, 4,  OP_0xA9),     // 0xA9
            new Instruction("XOR D",                   0, 4,  OP_0xAA),     // 0xAA
            new Instruction("XOR E",                   0, 4,  OP_0xAB),     // 0xAB
            new Instruction("XOR H",                   0, 4,  OP_0xAC),     // 0xAC
            new Instruction("XOR L",                   0, 4,  OP_0xAD),     // 0xAD
            new Instruction("XOR (HL)",                0, 8,  OP_0xAE),     // 0xAE
            new Instruction("XOR A",                   0, 4,  OP_0xAF),     // 0xAF
                                                                        
            new Instruction("OR B",                    0, 4,  OP_0xB0),     // 0xB0
            new Instruction("OR C",                    0, 4,  OP_0xB1),     // 0xB1
            new Instruction("OR D",                    0, 4,  OP_0xB2),     // 0xB2
            new Instruction("OR E",                    0, 4,  OP_0xB3),     // 0xB3
            new Instruction("OR H",                    0, 4,  OP_0xB4),     // 0xB4
            new Instruction("OR L",                    0, 4,  OP_0xB5),     // 0xB5
            new Instruction("OR (HL)",                 0, 8,  OP_0xB6),     // 0xB6
            new Instruction("OR A",                    0, 4,  OP_0xB7),     // 0xB7
            new Instruction("CP B",                    0, 4,  OP_0xB8),     // 0xB8
            new Instruction("CP C",                    0, 4,  OP_0xB9),     // 0xB9
            new Instruction("CP D",                    0, 4,  OP_0xBA),     // 0xBA
            new Instruction("CP E",                    0, 4,  OP_0xBB),     // 0xBB
            new Instruction("CP H",                    0, 4,  OP_0xBC),     // 0xBC
            new Instruction("CP L",                    0, 4,  OP_0xBD),     // 0xBD
            new Instruction("CP (HL)",                 0, 8,  OP_0xBE),     // 0xBE
            new Instruction("CP A",                    0, 4,  OP_0xBF),     // 0xBF
                                                          
            new Instruction("RET NZ",                  0, 8,  OP_0xC0),     // 0xC0 - Conditional
            new Instruction("POP BC",                  0, 12, OP_0xC1),     // 0xC1
            new Instruction("JP NZ, ${0:x4}",          2, 12, OP_0xC2),     // 0xC2 - Conditional
            new Instruction("JP {0:x4}",               2, 16, OP_0xC3),     // 0xC3
            new Instruction("CALL NZ, ${0:x4}",        2, 12, OP_0xC4),     // 0xC4 - Conditional
            new Instruction("PUSH BC",                 0, 16, OP_0xC5),     // 0xC5
            new Instruction("ADD A, ${0:x2}",          1, 8,  OP_0xC6),     // 0xC6
            new Instruction("RST 00H",                 0, 16, OP_0xC7),     // 0xC7
            new Instruction("RET Z",                   0, 8,  OP_0xC8),     // 0xC8
            new Instruction("RET",                     0, 4,  OP_0xC9),     // 0xC9
            new Instruction("JP Z, ${0:x4}",           2, 12, OP_0xCA),     // 0xCA
            new Instruction("PREFIX",                  0, 4,  OP_0xFF),     // 0xCB
            new Instruction("CALL Z, ${0:x4}",         2, 12, OP_0xCC),     // 0xCC - Conditional
            new Instruction("CALL ${0:x4}",            2, 24, OP_0xCD),     // 0xCD
            new Instruction("ADC A, ${0:x2}",          1, 8,  OP_0xCE),     // 0xCE
            new Instruction("RST 08H",                 0, 16, OP_0xCF),     // 0xCF
                                                          
            new Instruction("RET NC",                  0, 8,  OP_0xD0),     // 0xD0 - Conditional
            new Instruction("POP DE",                  0, 12, OP_0xD1),     // 0xD1
            new Instruction("JP NC, ${0:x4}",          2, 12, OP_0xD2),     // 0xD2 - Conditional
            new Instruction("INVALID_OPCODE",          0, 0,  OP_INVALID),  // 0xD3
            new Instruction("CALL NC, ${0:x4}",        2, 12, OP_0xD4),     // 0xD4 - Conditional
            new Instruction("PUSH DE",                 0, 16, OP_0xD5),     // 0xD5
            new Instruction("SUB {0:x2}",              1, 8,  OP_0xD6),     // 0xD6
            new Instruction("RST 10H",                 0, 16, OP_0xD7),     // 0xD7
            new Instruction("RET C",                   0, 8,  OP_0xD8),     // 0xD8
            new Instruction("RETI",                    0, 16, OP_0xD9),     // 0xD9
            new Instruction("JP C, ${0:x4}",           2, 12, OP_0xDA),     // 0xDA - Conditional
            new Instruction("INVALID_OPCODE",          0, 0,  OP_INVALID),  // 0xDB
            new Instruction("CALL C, ${0:x4}",         2, 12, OP_0xDC),     // 0xDC - Conditional
            new Instruction("INVALID_OPCODE",          0, 0,  OP_INVALID),  // 0xDD
            new Instruction("SBC A, ${0:x2}",          1, 8,  OP_0xDE),     // 0xDE
            new Instruction("RST 18H",                 0, 16, OP_0xDF),     // 0xDF
                                                          
            new Instruction("LD ($FF00 + ${0:x2}), A", 1, 12, OP_0xE0),     // 0xE0
            new Instruction("POP HL",                  0, 12, OP_0xE1),     // 0xE1
            new Instruction("LD ($FF00 + C), A",       0, 8,  OP_0xE2),     // 0xE2
            new Instruction("INVALID_OPCODE",          0, 0,  OP_INVALID),  // 0xE3
            new Instruction("INVALID_OPCODE",          0, 0,  OP_INVALID),  // 0xE4
            new Instruction("PUSH HL",                 0, 16, OP_0xE5),     // 0xE5
            new Instruction("AND {0:x2}",              1, 8,  OP_0xE6),     // 0xE6
            new Instruction("RST 20H",                 0, 16, OP_0xE7),     // 0xE7
            new Instruction("ADD SP, ${0:x2}",         1, 16, OP_0xE8),     // 0xE8
            new Instruction("JP HL",                   0, 4,  OP_0xE9),     // 0xE9
            new Instruction("LD (${0:x4}), A",         2, 16, OP_0xEA),     // 0xEA
            new Instruction("INVALID_OPCODE",          0, 4,  OP_INVALID),  // 0xEB
            new Instruction("INVALID_OPCODE",          0, 0,  OP_INVALID),  // 0xEC
            new Instruction("INVALID_OPCODE",          0, 0,  OP_INVALID),  // 0xED
            new Instruction("XOR ${0:x2}",             1, 8,  OP_0xEE),     // 0xEE
            new Instruction("RST 28H",                 0, 16, OP_0xEF),     // 0xEF
                                                          
            new Instruction("LD A, ($FF00 + ${0:x2})", 1, 12, OP_0xF0),     // 0xF0
            new Instruction("POP AF",                  0, 12, OP_0xF1),     // 0xF1
            new Instruction("LD A, ($FF00 + C)",       0, 8,  OP_0xF2),     // 0xF2
            new Instruction("DI",                      0, 4,  OP_0xF3),     // 0xF3
            new Instruction("INVALID_OPCODE",          0, 0,  OP_NOT_IMPL), // 0xF4
            new Instruction("PUSH AF",                 0, 16, OP_0xF5),     // 0xF5
            new Instruction("OR ${0:x2}",              1, 8,  OP_0xF6),     // 0xF6
            new Instruction("RST 30H",                 0, 16, OP_0xF7),     // 0xF7
            new Instruction("LD HL, SP + ${0:x2}",     1, 12, OP_0xF8),     // 0xF8
            new Instruction("LD SP, HL",               0, 8,  OP_0xF9),     // 0xF9
            new Instruction("LD A, (${0:x4})",         2, 16, OP_0xFA),     // 0xFA
            new Instruction("EI",                      0, 4,  OP_0xFB),     // 0xFB
            new Instruction("INVALID_OPCODE",          0, 0,  OP_INVALID),  // 0xFC
            new Instruction("INVALID_OPCODE",          0, 0,  OP_INVALID),  // 0xFD
            new Instruction("CP ${0:x2}",              1, 8,  OP_0xFE),     // 0xFE
            new Instruction("RST 38H",                 0, 16, OP_0xFF),     // 0xFF

            // Prefixed    
            new Instruction("RLC B",                   1, 8,  OP_CB_0x00),  // 0x00
            new Instruction("RLC C",                   1, 8,  OP_CB_0x01),  // 0x01
            new Instruction("RLC D",                   1, 8,  OP_CB_0x02),  // 0x02
            new Instruction("RLC E",                   1, 8,  OP_CB_0x03),  // 0x03
            new Instruction("RLC H",                   1, 8,  OP_CB_0x04),  // 0x04
            new Instruction("RLC L",                   1, 8,  OP_CB_0x05),  // 0x05
            new Instruction("RLC (HL)",                1, 16, OP_CB_0x06),  // 0x06
            new Instruction("RLC A",                   1, 8,  OP_CB_0x07),  // 0x07
            new Instruction("RRC B",                   1, 8,  OP_CB_0x08),  // 0x08
            new Instruction("RRC C",                   1, 8,  OP_CB_0x09),  // 0x09
            new Instruction("RRC D",                   1, 8,  OP_CB_0x0A),  // 0x0A
            new Instruction("RRC E",                   1, 8,  OP_CB_0x0B),  // 0x0B
            new Instruction("RRC H",                   1, 8,  OP_CB_0x0C),  // 0x0C
            new Instruction("RRC L",                   1, 8,  OP_CB_0x0D),  // 0x0D
            new Instruction("RRC (HL)",                1, 16, OP_CB_0x0E),  // 0x0E
            new Instruction("RRC A",                   1, 8,  OP_CB_0x0F),  // 0x0F
 
            new Instruction("RL B",                    1, 8,  OP_CB_0x10),  // 0x10
            new Instruction("RL C",                    1, 8,  OP_CB_0x11),  // 0x11
            new Instruction("RL D",                    1, 8,  OP_CB_0x12),  // 0x12
            new Instruction("RL E",                    1, 8,  OP_CB_0x13),  // 0x13
            new Instruction("RL H",                    1, 8,  OP_CB_0x14),  // 0x14
            new Instruction("RL L",                    1, 8,  OP_CB_0x15),  // 0x15
            new Instruction("RL (HL)",                 1, 16, OP_CB_0x16),  // 0x16
            new Instruction("RL A",                    1, 8,  OP_CB_0x17),  // 0x17
            new Instruction("RR B",                    1, 8,  OP_CB_0x18),  // 0x18
            new Instruction("RR C",                    1, 8,  OP_CB_0x19),  // 0x19
            new Instruction("RR D",                    1, 8,  OP_CB_0x1A),  // 0x1A
            new Instruction("RR E",                    1, 8,  OP_CB_0x1B),  // 0x1B
            new Instruction("RR H",                    1, 8,  OP_CB_0x1C),  // 0x1C
            new Instruction("RR L",                    1, 8,  OP_CB_0x1D),  // 0x1D
            new Instruction("RR (HL)",                 1, 16, OP_CB_0x1E),  // 0x1E
            new Instruction("RR A",                    1, 8,  OP_CB_0x1F),  // 0x1F
 
            new Instruction("SLA B",                   1, 8,  OP_CB_0x20),  // 0x20
            new Instruction("SLA C",                   1, 8,  OP_CB_0x21),  // 0x21
            new Instruction("SLA D",                   1, 8,  OP_CB_0x22),  // 0x22
            new Instruction("SLA E",                   1, 8,  OP_CB_0x23),  // 0x23
            new Instruction("SLA H",                   1, 8,  OP_CB_0x24),  // 0x24
            new Instruction("SLA L",                   1, 8,  OP_CB_0x25),  // 0x25
            new Instruction("SLA (HL)",                1, 16, OP_CB_0x26),  // 0x26
            new Instruction("SLA A",                   1, 8,  OP_CB_0x27),  // 0x27
            new Instruction("SRA B",                   1, 8,  OP_CB_0x28),  // 0x28
            new Instruction("SRA C",                   1, 8,  OP_CB_0x29),  // 0x29
            new Instruction("SRA D",                   1, 8,  OP_CB_0x2A),  // 0x2A
            new Instruction("SRA E",                   1, 8,  OP_CB_0x2B),  // 0x2B
            new Instruction("SRA H",                   1, 8,  OP_CB_0x2C),  // 0x2C
            new Instruction("SRA L",                   1, 8,  OP_CB_0x2D),  // 0x2D
            new Instruction("SRA (HL)",                1, 16, OP_CB_0x2E),  // 0x2E
            new Instruction("SRA A",                   1, 8,  OP_CB_0x2F),  // 0x2F

            new Instruction("SWAP B",                  1, 8,  OP_CB_0x30),  // 0x30
            new Instruction("SWAP C",                  1, 8,  OP_CB_0x31),  // 0x31
            new Instruction("SWAP D",                  1, 8,  OP_CB_0x32),  // 0x32
            new Instruction("SWAP E",                  1, 8,  OP_CB_0x33),  // 0x33
            new Instruction("SWAP H",                  1, 8,  OP_CB_0x34),  // 0x34
            new Instruction("SWAP L",                  1, 8,  OP_CB_0x35),  // 0x35
            new Instruction("SWAP (HL)",               1, 16, OP_CB_0x36),  // 0x36
            new Instruction("SWAP A",                  1, 8,  OP_CB_0x37),  // 0x37
            new Instruction("SRL B",                   1, 8,  OP_CB_0x38),  // 0x38
            new Instruction("SRL C",                   1, 8,  OP_CB_0x39),  // 0x39
            new Instruction("SRL D",                   1, 8,  OP_CB_0x3A),  // 0x3A
            new Instruction("SRL E",                   1, 8,  OP_CB_0x3B),  // 0x3B
            new Instruction("SRL H",                   1, 8,  OP_CB_0x3C),  // 0x3C
            new Instruction("SRL L",                   1, 8,  OP_CB_0x3D),  // 0x3D
            new Instruction("SRL (HL)",                1, 16, OP_CB_0x3E),  // 0x3E
            new Instruction("SRL A",                   1, 8,  OP_CB_0x3F),  // 0x3F
 
            new Instruction("BIT 0, B",                1, 8,  OP_CB_0x40),  // 0x40
            new Instruction("BIT 0, C",                1, 8,  OP_CB_0x41),  // 0x41
            new Instruction("BIT 0, D",                1, 8,  OP_CB_0x42),  // 0x42
            new Instruction("BIT 0, E",                1, 8,  OP_CB_0x43),  // 0x43
            new Instruction("BIT 0, H",                1, 8,  OP_CB_0x44),  // 0x44
            new Instruction("BIT 0, L",                1, 8,  OP_CB_0x45),  // 0x45
            new Instruction("BIT 0, (HL)",             1, 12, OP_CB_0x46),  // 0x46
            new Instruction("BIT 0, A",                1, 8,  OP_CB_0x47),  // 0x47
            new Instruction("BIT 1, B",                1, 8,  OP_CB_0x48),  // 0x48
            new Instruction("BIT 1, C",                1, 8,  OP_CB_0x49),  // 0x49
            new Instruction("BIT 1, D",                1, 8,  OP_CB_0x4A),  // 0x4A
            new Instruction("BIT 1, E",                1, 8,  OP_CB_0x4B),  // 0x4B
            new Instruction("BIT 1, H",                1, 8,  OP_CB_0x4C),  // 0x4C
            new Instruction("BIT 1, L",                1, 8,  OP_CB_0x4D),  // 0x4D
            new Instruction("BIT 1, (HL)",             1, 12, OP_CB_0x4E),  // 0x4E
            new Instruction("BIT 1, A",                1, 8,  OP_CB_0x4F),  // 0x4F
 
            new Instruction("BIT 2, B",                1, 8,  OP_CB_0x50),  // 0x50
            new Instruction("BIT 2, C",                1, 8,  OP_CB_0x51),  // 0x51
            new Instruction("BIT 2, D",                1, 8,  OP_CB_0x52),  // 0x52
            new Instruction("BIT 2, E",                1, 8,  OP_CB_0x53),  // 0x53
            new Instruction("BIT 2, H",                1, 8,  OP_CB_0x54),  // 0x54
            new Instruction("BIT 2, L",                1, 8,  OP_CB_0x55),  // 0x55
            new Instruction("BIT 2, (HL)",             1, 12, OP_CB_0x56),  // 0x56
            new Instruction("BIT 2, A",                1, 8,  OP_CB_0x57),  // 0x57
            new Instruction("BIT 3, B",                1, 8,  OP_CB_0x58),  // 0x58
            new Instruction("BIT 3, C",                1, 8,  OP_CB_0x59),  // 0x59
            new Instruction("BIT 3, D",                1, 8,  OP_CB_0x5A),  // 0x5A
            new Instruction("BIT 3, E",                1, 8,  OP_CB_0x5B),  // 0x5B
            new Instruction("BIT 3, H",                1, 8,  OP_CB_0x5C),  // 0x5C
            new Instruction("BIT 3, L",                1, 8,  OP_CB_0x5D),  // 0x5D
            new Instruction("BIT 3, (HL)",             1, 12, OP_CB_0x5E),  // 0x5E
            new Instruction("BIT 3, A",                1, 8,  OP_CB_0x5F),  // 0x5F
 
            new Instruction("BIT 4, B",                1, 8,  OP_CB_0x60),  // 0x60
            new Instruction("BIT 4, C",                1, 8,  OP_CB_0x61),  // 0x61
            new Instruction("BIT 4, D",                1, 8,  OP_CB_0x62),  // 0x62
            new Instruction("BIT 4, E",                1, 8,  OP_CB_0x63),  // 0x63
            new Instruction("BIT 4, H",                1, 8,  OP_CB_0x64),  // 0x64
            new Instruction("BIT 4, L",                1, 8,  OP_CB_0x65),  // 0x65
            new Instruction("BIT 4, (HL)",             1, 12, OP_CB_0x66),  // 0x66
            new Instruction("BIT 4, A",                1, 8,  OP_CB_0x67),  // 0x67
            new Instruction("BIT 5, B",                1, 8,  OP_CB_0x68),  // 0x68
            new Instruction("BIT 5, C",                1, 8,  OP_CB_0x69),  // 0x69
            new Instruction("BIT 5, D",                1, 8,  OP_CB_0x6A),  // 0x6A
            new Instruction("BIT 5, E",                1, 8,  OP_CB_0x6B),  // 0x6B
            new Instruction("BIT 5, H",                1, 8,  OP_CB_0x6C),  // 0x6C
            new Instruction("BIT 5, L",                1, 8,  OP_CB_0x6D),  // 0x6D
            new Instruction("BIT 5, (HL)",             1, 12, OP_CB_0x6E),  // 0x6E
            new Instruction("BIT 5, A",                1, 8,  OP_CB_0x6F),  // 0x6F
 
            new Instruction("BIT 6, B",                1, 8,  OP_CB_0x70),  // 0x70
            new Instruction("BIT 6, C",                1, 8,  OP_CB_0x71),  // 0x71
            new Instruction("BIT 6, D",                1, 8,  OP_CB_0x72),  // 0x72
            new Instruction("BIT 6, E",                1, 8,  OP_CB_0x73),  // 0x73
            new Instruction("BIT 6, H",                1, 8,  OP_CB_0x74),  // 0x74
            new Instruction("BIT 6, L",                1, 8,  OP_CB_0x75),  // 0x75
            new Instruction("BIT 6, (HL)",             1, 12, OP_CB_0x76),  // 0x76
            new Instruction("BIT 6, A",                1, 8,  OP_CB_0x77),  // 0x77
            new Instruction("BIT 7, B",                1, 8,  OP_CB_0x78),  // 0x78
            new Instruction("BIT 7, C",                1, 8,  OP_CB_0x79),  // 0x79
            new Instruction("BIT 7, D",                1, 8,  OP_CB_0x7A),  // 0x7A
            new Instruction("BIT 7, E",                1, 8,  OP_CB_0x7B),  // 0x7B
            new Instruction("BIT 7, H",                1, 8,  OP_CB_0x7C),  // 0x7C
            new Instruction("BIT 7, L",                1, 8,  OP_CB_0x7D),  // 0x7D
            new Instruction("BIT 7, (HL)",             1, 12, OP_CB_0x7E),  // 0x7E
            new Instruction("BIT 7, A",                1, 8,  OP_CB_0x7F),  // 0x7F
 
            new Instruction("RES 0, B",                1, 8,  OP_CB_0x80),  // 0x80
            new Instruction("RES 0, C",                1, 8,  OP_CB_0x81),  // 0x81
            new Instruction("RES 0, D",                1, 8,  OP_CB_0x82),  // 0x82
            new Instruction("RES 0, E",                1, 8,  OP_CB_0x83),  // 0x83
            new Instruction("RES 0, H",                1, 8,  OP_CB_0x84),  // 0x84
            new Instruction("RES 0, L",                1, 8,  OP_CB_0x85),  // 0x85
            new Instruction("RES 0, (HL)",             1, 16, OP_CB_0x86),  // 0x86
            new Instruction("RES 0, A",                1, 8,  OP_CB_0x87),  // 0x87
            new Instruction("RES 1, B",                1, 8,  OP_CB_0x88),  // 0x88
            new Instruction("RES 1, C",                1, 8,  OP_CB_0x89),  // 0x89
            new Instruction("RES 1, D",                1, 8,  OP_CB_0x8A),  // 0x8A
            new Instruction("RES 1, E",                1, 8,  OP_CB_0x8B),  // 0x8B
            new Instruction("RES 1, H",                1, 8,  OP_CB_0x8C),  // 0x8C
            new Instruction("RES 1, L",                1, 8,  OP_CB_0x8D),  // 0x8D
            new Instruction("RES 1, (HL)",             1, 16, OP_CB_0x8E),  // 0x8E
            new Instruction("RES 1, A",                1, 8,  OP_CB_0x8F),  // 0x8F
 
            new Instruction("RES 2, B",                1, 8,  OP_CB_0x90),  // 0x90
            new Instruction("RES 2, C",                1, 8,  OP_CB_0x91),  // 0x91
            new Instruction("RES 2, D",                1, 8,  OP_CB_0x92),  // 0x92
            new Instruction("RES 2, E",                1, 8,  OP_CB_0x93),  // 0x93
            new Instruction("RES 2, H",                1, 8,  OP_CB_0x94),  // 0x94
            new Instruction("RES 2, L",                1, 8,  OP_CB_0x95),  // 0x95
            new Instruction("RES 2, (HL)",             1, 16, OP_CB_0x96),  // 0x96
            new Instruction("RES 2, A",                1, 8,  OP_CB_0x97),  // 0x97
            new Instruction("RES 3, B",                1, 8,  OP_CB_0x98),  // 0x98
            new Instruction("RES 3, C",                1, 8,  OP_CB_0x99),  // 0x99
            new Instruction("RES 3, D",                1, 8,  OP_CB_0x9A),  // 0x9A
            new Instruction("RES 3, E",                1, 8,  OP_CB_0x9B),  // 0x9B
            new Instruction("RES 3, H",                1, 8,  OP_CB_0x9C),  // 0x9C
            new Instruction("RES 3, L",                1, 8,  OP_CB_0x9D),  // 0x9D
            new Instruction("RES 3, (HL)",             1, 16, OP_CB_0x9E),  // 0x9E
            new Instruction("RES 3, A",                1, 8,  OP_CB_0x9F),  // 0x9F
 
            new Instruction("RES 4, B",                1, 8,  OP_CB_0xA0),  // 0xA0
            new Instruction("RES 4, C",                1, 8,  OP_CB_0xA1),  // 0xA1
            new Instruction("RES 4, D",                1, 8,  OP_CB_0xA2),  // 0xA2
            new Instruction("RES 4, E",                1, 8,  OP_CB_0xA3),  // 0xA3
            new Instruction("RES 4, H",                1, 8,  OP_CB_0xA4),  // 0xA4
            new Instruction("RES 4, L",                1, 8,  OP_CB_0xA5),  // 0xA5
            new Instruction("RES 4, (HL)",             1, 16, OP_CB_0xA6),  // 0xA6
            new Instruction("RES 4, A",                1, 8,  OP_CB_0xA7),  // 0xA7
            new Instruction("RES 5, B",                1, 8,  OP_CB_0xA8),  // 0xA8
            new Instruction("RES 5, C",                1, 8,  OP_CB_0xA9),  // 0xA9
            new Instruction("RES 5, D",                1, 8,  OP_CB_0xAA),  // 0xAA
            new Instruction("RES 5, E",                1, 8,  OP_CB_0xAB),  // 0xAB
            new Instruction("RES 5, H",                1, 8,  OP_CB_0xAC),  // 0xAC
            new Instruction("RES 5, L",                1, 8,  OP_CB_0xAD),  // 0xAD
            new Instruction("RES 5, (HL)",             1, 16, OP_CB_0xAE),  // 0xAE
            new Instruction("RES 5, A",                1, 8,  OP_CB_0xAF),  // 0xAF
 
            new Instruction("RES 6, B",                1, 8,  OP_CB_0xB0),  // 0xB0
            new Instruction("RES 6, C",                1, 8,  OP_CB_0xB1),  // 0xB1
            new Instruction("RES 6, D",                1, 8,  OP_CB_0xB2),  // 0xB2
            new Instruction("RES 6, E",                1, 8,  OP_CB_0xB3),  // 0xB3
            new Instruction("RES 6, H",                1, 8,  OP_CB_0xB4),  // 0xB4
            new Instruction("RES 6, L",                1, 8,  OP_CB_0xB5),  // 0xB5
            new Instruction("RES 6, (HL)",             1, 16, OP_CB_0xB6),  // 0xB6
            new Instruction("RES 6, A",                1, 8,  OP_CB_0xB7),  // 0xB7
            new Instruction("RES 7, B",                1, 8,  OP_CB_0xB8),  // 0xB8
            new Instruction("RES 7, C",                1, 8,  OP_CB_0xB9),  // 0xB9
            new Instruction("RES 7, D",                1, 8,  OP_CB_0xBA),  // 0xBA
            new Instruction("RES 7, E",                1, 8,  OP_CB_0xBB),  // 0xBB
            new Instruction("RES 7, H",                1, 8,  OP_CB_0xBC),  // 0xBC
            new Instruction("RES 7, L",                1, 8,  OP_CB_0xBD),  // 0xBD
            new Instruction("RES 7, (HL)",             1, 16, OP_CB_0xBE),  // 0xBE
            new Instruction("RES 7, A",                1, 8,  OP_CB_0xBF),  // 0xBF
 
            new Instruction("SET 0, B",                1, 8,  OP_CB_0xC0), // 0xC0
            new Instruction("SET 0, C",                1, 8,  OP_CB_0xC1), // 0xC1
            new Instruction("SET 0, D",                1, 8,  OP_CB_0xC2), // 0xC2
            new Instruction("SET 0, E",                1, 8,  OP_CB_0xC3), // 0xC3
            new Instruction("SET 0, H",                1, 8,  OP_CB_0xC4), // 0xC4
            new Instruction("SET 0, L",                1, 8,  OP_CB_0xC5), // 0xC5
            new Instruction("SET 0, (HL)",             1, 16, OP_CB_0xC6), // 0xC6
            new Instruction("SET 0, A",                1, 8,  OP_CB_0xC7), // 0xC7
            new Instruction("SET 1, B",                1, 8,  OP_CB_0xC8), // 0xC8
            new Instruction("SET 1, C",                1, 8,  OP_CB_0xC9), // 0xC9
            new Instruction("SET 1, D",                1, 8,  OP_CB_0xCA), // 0xCA
            new Instruction("SET 1, E",                1, 8,  OP_CB_0xCB), // 0xCB
            new Instruction("SET 1, H",                1, 8,  OP_CB_0xCC), // 0xCC
            new Instruction("SET 1, L",                1, 8,  OP_CB_0xCD), // 0xCD
            new Instruction("SET 1, (HL)",             1, 16, OP_CB_0xCE), // 0xCE
            new Instruction("SET 1, A",                1, 8,  OP_CB_0xCF), // 0xCF
 
            new Instruction("SET 2, B",                1, 8,  OP_CB_0xD0), // 0xD0
            new Instruction("SET 2, C",                1, 8,  OP_CB_0xD1), // 0xD1
            new Instruction("SET 2, D",                1, 8,  OP_CB_0xD2), // 0xD2
            new Instruction("SET 2, E",                1, 8,  OP_CB_0xD3), // 0xD3
            new Instruction("SET 2, H",                1, 8,  OP_CB_0xD4), // 0xD4
            new Instruction("SET 2, L",                1, 8,  OP_CB_0xD5), // 0xD5
            new Instruction("SET 2, (HL)",             1, 16, OP_CB_0xD6), // 0xD6
            new Instruction("SET 2, A",                1, 8,  OP_CB_0xD7), // 0xD7
            new Instruction("SET 3, B",                1, 8,  OP_CB_0xD8), // 0xD8
            new Instruction("SET 3, C",                1, 8,  OP_CB_0xD9), // 0xD9
            new Instruction("SET 3, D",                1, 8,  OP_CB_0xDA), // 0xDA
            new Instruction("SET 3, E",                1, 8,  OP_CB_0xDB), // 0xDB
            new Instruction("SET 3, H",                1, 8,  OP_CB_0xDC), // 0xDC
            new Instruction("SET 3, L",                1, 8,  OP_CB_0xDD), // 0xDD
            new Instruction("SET 3, (HL)",             1, 16, OP_CB_0xDE), // 0xDE
            new Instruction("SET 3, A",                1, 8,  OP_CB_0xDF), // 0xDF
 
            new Instruction("SET 4, B",                1, 8,  OP_CB_0xE0), // 0xE0
            new Instruction("SET 4, C",                1, 8,  OP_CB_0xE1), // 0xE1
            new Instruction("SET 4, D",                1, 8,  OP_CB_0xE2), // 0xE2
            new Instruction("SET 4, E",                1, 8,  OP_CB_0xE3), // 0xE3
            new Instruction("SET 4, H",                1, 8,  OP_CB_0xE4), // 0xE4
            new Instruction("SET 4, L",                1, 8,  OP_CB_0xE5), // 0xE5
            new Instruction("SET 4, (HL)",             1, 16, OP_CB_0xE6), // 0xE6
            new Instruction("SET 4, A",                1, 8,  OP_CB_0xE7), // 0xE7
            new Instruction("SET 5, B",                1, 8,  OP_CB_0xE8), // 0xE8
            new Instruction("SET 5, C",                1, 8,  OP_CB_0xE9), // 0xE9
            new Instruction("SET 5, D",                1, 8,  OP_CB_0xEA), // 0xEA
            new Instruction("SET 5, E",                1, 8,  OP_CB_0xEB), // 0xEB
            new Instruction("SET 5, H",                1, 8,  OP_CB_0xEC), // 0xEC
            new Instruction("SET 5, L",                1, 8,  OP_CB_0xED), // 0xED
            new Instruction("SET 5, (HL)",             1, 16, OP_CB_0xEE), // 0xEE
            new Instruction("SET 5, A",                1, 8,  OP_CB_0xEF), // 0xEF
 
            new Instruction("SET 6, B",                1, 8,  OP_CB_0xF0), // 0xF0
            new Instruction("SET 6, C",                1, 8,  OP_CB_0xF1), // 0xF1
            new Instruction("SET 6, D",                1, 8,  OP_CB_0xF2), // 0xF2
            new Instruction("SET 6, E",                1, 8,  OP_CB_0xF3), // 0xF3
            new Instruction("SET 6, H",                1, 8,  OP_CB_0xF4), // 0xF4
            new Instruction("SET 6, L",                1, 8,  OP_CB_0xF5), // 0xF5
            new Instruction("SET 6, (HL)",             1, 16, OP_CB_0xF6), // 0xF6
            new Instruction("SET 6, A",                1, 8,  OP_CB_0xF7), // 0xF7
            new Instruction("SET 7, B",                1, 8,  OP_CB_0xF8), // 0xF8
            new Instruction("SET 7, C",                1, 8,  OP_CB_0xF9), // 0xF9
            new Instruction("SET 7, D",                1, 8,  OP_CB_0xFA), // 0xFA
            new Instruction("SET 7, E",                1, 8,  OP_CB_0xFB), // 0xFB
            new Instruction("SET 7, H",                1, 8,  OP_CB_0xFC), // 0xFC
            new Instruction("SET 7, L",                1, 8,  OP_CB_0xFD), // 0xFD
            new Instruction("SET 7, (HL)",             1, 16, OP_CB_0xFE), // 0xFE
            new Instruction("SET 7, A",                1, 8,  OP_CB_0xFF), // 0xFF
        };
    }
    

    private static void OP_NOT_IMPL()
    {
        string str = string.Concat(
            "!! CPU FAULT - HERE BE DRAGONS !!\n",
            "This is likely:\n",
            "- An emulation bug\n",
            "- An unimplmeneted opcode\n\n",

            "Executuion can continue, but probably shouldn't, as processor state is likely corrupt."
        );

        Console.WriteLine(str);
        Debugger.Break();
    }

    private static void OP_INVALID()
    {
        throw new Exception("Invalid or illegal opcode.");
    }


    private void OP_0x00()               => processor.registers.A  = processor.registers.A;
    private void OP_0x01(ushort operand) => processor.registers.BC = operand;
    private void OP_0x02()               => processor.memory.WriteByte(processor.registers.BC, processor.registers.A);
    private void OP_0x03()               => processor.registers.BC++;
    private void OP_0x04()               => processor.registers.B = Inc(processor.registers.B);
    private void OP_0x05()               => processor.registers.B = Dec(processor.registers.B);
    private void OP_0x06(byte operand)   => processor.registers.B = operand;
    private void OP_0x07()               => processor.registers.A = Rlc(processor.registers.A, false);
    private void OP_0x08(ushort operand) => processor.memory.WriteWord(operand, processor.registers.SP);
    private void OP_0x09()               => processor.registers.HL = Add(processor.registers.HL, processor.registers.BC);
    private void OP_0x0A()               => processor.registers.A = processor.memory.ReadByte(processor.registers.BC);
    private void OP_0x0B()               => processor.registers.BC--;
    private void OP_0x0C()               => processor.registers.C = Inc(processor.registers.C);
    private void OP_0x0D()               => processor.registers.C = Dec(processor.registers.C);
    private void OP_0x0E(byte   operand) => processor.registers.C = operand;
    private void OP_0x0F()               => processor.registers.A = Rrc(processor.registers.A, false);

    private void OP_0x10()               => Console.WriteLine("STOP stub");
    private void OP_0x11(ushort operand) => processor.registers.DE = operand;
    private void OP_0x12()               => processor.memory.WriteByte(processor.registers.DE, processor.registers.A);
    private void OP_0x13()               => processor.registers.DE++;
    private void OP_0x14()               => processor.registers.D = Inc(processor.registers.D);
    private void OP_0x15()               => processor.registers.D = Dec(processor.registers.D);
    private void OP_0x16(byte   operand) => processor.registers.D = operand;
    private void OP_0x17()               => processor.registers.A = Rl(processor.registers.A, false);
    private void OP_0x18(byte   operand) => JumpRelative(operand);
    private void OP_0x19()               => processor.registers.HL = Add(processor.registers.HL, processor.registers.DE);
    private void OP_0x1A()               => processor.registers.A = processor.memory.ReadByte(processor.registers.DE);
    private void OP_0x1B()               => processor.registers.DE--;
    private void OP_0x1C()               => processor.registers.E = Inc(processor.registers.E);
    private void OP_0x1D()               => processor.registers.E = Dec(processor.registers.E);
    private void OP_0x1E(byte   operand) => processor.registers.E = operand;
    private void OP_0x1F()               => processor.registers.A = Rr(processor.registers.A, false);

    private void OP_0x20(byte   operand) => JumpRelative(operand, CPUFlags.Zero, false);
    private void OP_0x21(ushort operand) => processor.registers.HL = operand;
    private void OP_0x22()               => processor.memory.WriteByte(processor.registers.HL++, processor.registers.A);
    private void OP_0x23()               => processor.registers.HL++;
    private void OP_0x24()               => processor.registers.H = Inc(processor.registers.H);
    private void OP_0x25()               => processor.registers.H = Dec(processor.registers.H);
    private void OP_0x26(byte   operand) => processor.registers.H = operand;
    private void OP_0x27()               => Daa();
    private void OP_0x28(byte   operand) => JumpRelative(operand, CPUFlags.Zero, true);
    private void OP_0x29()               => processor.registers.HL = Add(processor.registers.HL, processor.registers.HL);
    private void OP_0x2A()               => processor.registers.A = processor.memory.ReadByte(processor.registers.HL++);
    private void OP_0x2B()               => processor.registers.HL--;
    private void OP_0x2C()               => processor.registers.L = Inc(processor.registers.L);
    private void OP_0x2D()               => processor.registers.L = Dec(processor.registers.L);
    private void OP_0x2E(byte  operand)  => processor.registers.L = operand;
    private void OP_0x2F()               => Cpl();

    private void OP_0x30(byte   operand) => JumpRelative(operand, CPUFlags.Carry, false);
    private void OP_0x31(ushort operand) => processor.registers.SP = operand;
    private void OP_0x32()               => processor.memory.WriteByte(processor.registers.HL--, processor.registers.A);
    private void OP_0x33()               => processor.registers.SP++;
    private void OP_0x34()               => processor.memory.WriteByte(processor.registers.HL, Inc(processor.memory.ReadByte(processor.registers.HL)));
    private void OP_0x35()               => processor.memory.WriteByte(processor.registers.HL, Dec(processor.memory.ReadByte(processor.registers.HL)));
    private void OP_0x36(byte   operand) => processor.memory.WriteByte(processor.registers.HL, operand);
    private void OP_0x37()               => Scf();
    private void OP_0x38(byte   operand) => JumpRelative(operand, CPUFlags.Carry, true);
    private void OP_0x39()               => processor.registers.HL = Add(processor.registers.HL, processor.registers.SP);
    private void OP_0x3A()               => processor.registers.A  = processor.memory.ReadByte(processor.registers.HL--);
    private void OP_0x3B()               => processor.registers.SP--;
    private void OP_0x3C()               => processor.registers.A = Inc(processor.registers.A);
    private void OP_0x3D()               => processor.registers.A = Dec(processor.registers.A);
    private void OP_0x3E(byte   operand) => processor.registers.A = operand;
    private void OP_0x3F()               => Ccf();

    private void OP_0x40()               => processor.registers.B = processor.registers.B;
    private void OP_0x41()               => processor.registers.B = processor.registers.C;
    private void OP_0x42()               => processor.registers.B = processor.registers.D;
    private void OP_0x43()               => processor.registers.B = processor.registers.E;
    private void OP_0x44()               => processor.registers.B = processor.registers.H;
    private void OP_0x45()               => processor.registers.B = processor.registers.L;
    private void OP_0x46()               => processor.registers.B = processor.memory.ReadByte(processor.registers.HL);
    private void OP_0x47()               => processor.registers.B = processor.registers.A;
    private void OP_0x48()               => processor.registers.C = processor.registers.B;
    private void OP_0x49()               => processor.registers.C = processor.registers.C;
    private void OP_0x4A()               => processor.registers.C = processor.registers.D;
    private void OP_0x4B()               => processor.registers.C = processor.registers.E;
    private void OP_0x4C()               => processor.registers.C = processor.registers.H;
    private void OP_0x4D()               => processor.registers.C = processor.registers.L;
    private void OP_0x4E()               => processor.registers.C = processor.memory.ReadByte(processor.registers.HL);
    private void OP_0x4F()               => processor.registers.C = processor.registers.A;

    private void OP_0x50()               => processor.registers.D = processor.registers.B;
    private void OP_0x51()               => processor.registers.D = processor.registers.C;
    private void OP_0x52()               => processor.registers.D = processor.registers.D;
    private void OP_0x53()               => processor.registers.D = processor.registers.E;
    private void OP_0x54()               => processor.registers.D = processor.registers.H;
    private void OP_0x55()               => processor.registers.D = processor.registers.L;
    private void OP_0x56()               => processor.registers.D = processor.memory.ReadByte(processor.registers.HL);
    private void OP_0x57()               => processor.registers.D = processor.registers.A;
    private void OP_0x58()               => processor.registers.E = processor.registers.B;
    private void OP_0x59()               => processor.registers.E = processor.registers.C;
    private void OP_0x5A()               => processor.registers.E = processor.registers.D;
    private void OP_0x5B()               => processor.registers.E = processor.registers.E;
    private void OP_0x5C()               => processor.registers.E = processor.registers.H;
    private void OP_0x5D()               => processor.registers.E = processor.registers.L;
    private void OP_0x5E()               => processor.registers.E = processor.memory.ReadByte(processor.registers.HL);
    private void OP_0x5F()               => processor.registers.E = processor.registers.A;

    private void OP_0x60()               => processor.registers.H = processor.registers.B;
    private void OP_0x61()               => processor.registers.H = processor.registers.C;
    private void OP_0x62()               => processor.registers.H = processor.registers.D;
    private void OP_0x63()               => processor.registers.H = processor.registers.E;
    private void OP_0x64()               => processor.registers.H = processor.registers.H;
    private void OP_0x65()               => processor.registers.H = processor.registers.L;
    private void OP_0x66()               => processor.registers.H = processor.memory.ReadByte(processor.registers.HL);
    private void OP_0x67()               => processor.registers.H = processor.registers.A;
    private void OP_0x68()               => processor.registers.L = processor.registers.B;
    private void OP_0x69()               => processor.registers.L = processor.registers.C;
    private void OP_0x6A()               => processor.registers.L = processor.registers.D;
    private void OP_0x6B()               => processor.registers.L = processor.registers.E;
    private void OP_0x6C()               => processor.registers.L = processor.registers.H;
    private void OP_0x6D()               => processor.registers.L = processor.registers.L;
    private void OP_0x6E()               => processor.registers.L = processor.memory.ReadByte(processor.registers.HL);
    private void OP_0x6F()               => processor.registers.L = processor.registers.A;

    private void OP_0x70()               => processor.memory.WriteByte(processor.registers.HL, processor.registers.B);
    private void OP_0x71()               => processor.memory.WriteByte(processor.registers.HL, processor.registers.C);
    private void OP_0x72()               => processor.memory.WriteByte(processor.registers.HL, processor.registers.D);
    private void OP_0x73()               => processor.memory.WriteByte(processor.registers.HL, processor.registers.E);
    private void OP_0x74()               => processor.memory.WriteByte(processor.registers.HL, processor.registers.H);
    private void OP_0x75()               => processor.memory.WriteByte(processor.registers.HL, processor.registers.L);
    private void OP_0x76()               => Halt();
    private void OP_0x77()               => processor.memory.WriteByte(processor.registers.HL, processor.registers.A);
    private void OP_0x78()               => processor.registers.A = processor.registers.B;
    private void OP_0x79()               => processor.registers.A = processor.registers.C;
    private void OP_0x7A()               => processor.registers.A = processor.registers.D;
    private void OP_0x7B()               => processor.registers.A = processor.registers.E;
    private void OP_0x7C()               => processor.registers.A = processor.registers.H;
    private void OP_0x7D()               => processor.registers.A = processor.registers.L;
    private void OP_0x7E()               => processor.registers.A = processor.memory.ReadByte(processor.registers.HL);
    private void OP_0x7F()               => processor.registers.A = processor.registers.A;

    private void OP_0x80()               => processor.registers.A = Add(processor.registers.A, processor.registers.B);
    private void OP_0x81()               => processor.registers.A = Add(processor.registers.A, processor.registers.C);
    private void OP_0x82()               => processor.registers.A = Add(processor.registers.A, processor.registers.D);
    private void OP_0x83()               => processor.registers.A = Add(processor.registers.A, processor.registers.E);
    private void OP_0x84()               => processor.registers.A = Add(processor.registers.A, processor.registers.H);
    private void OP_0x85()               => processor.registers.A = Add(processor.registers.A, processor.registers.L);
    private void OP_0x86()               => processor.registers.A = Add(processor.registers.A, processor.memory.ReadByte(processor.registers.HL));
    private void OP_0x87()               => processor.registers.A = Add(processor.registers.A, processor.registers.A);        
    private void OP_0x88()               => processor.registers.A = Adc(processor.registers.A, processor.registers.B);
    private void OP_0x89()               => processor.registers.A = Adc(processor.registers.A, processor.registers.C);
    private void OP_0x8A()               => processor.registers.A = Adc(processor.registers.A, processor.registers.D);
    private void OP_0x8B()               => processor.registers.A = Adc(processor.registers.A, processor.registers.E);
    private void OP_0x8C()               => processor.registers.A = Adc(processor.registers.A, processor.registers.H);
    private void OP_0x8D()               => processor.registers.A = Adc(processor.registers.A, processor.registers.L);
    private void OP_0x8E()               => processor.registers.A = Adc(processor.registers.A, processor.memory.ReadByte(processor.registers.HL));
    private void OP_0x8F()               => processor.registers.A = Adc(processor.registers.A, processor.registers.A);

    private void OP_0x90()               => processor.registers.A = Sub(processor.registers.A, processor.registers.B);
    private void OP_0x91()               => processor.registers.A = Sub(processor.registers.A, processor.registers.C);
    private void OP_0x92()               => processor.registers.A = Sub(processor.registers.A, processor.registers.D);
    private void OP_0x93()               => processor.registers.A = Sub(processor.registers.A, processor.registers.E);
    private void OP_0x94()               => processor.registers.A = Sub(processor.registers.A, processor.registers.H);
    private void OP_0x95()               => processor.registers.A = Sub(processor.registers.A, processor.registers.L);
    private void OP_0x96()               => processor.registers.A = Sub(processor.registers.A, processor.memory.ReadByte(processor.registers.HL));
    private void OP_0x97()               => processor.registers.A = Sub(processor.registers.A, processor.registers.A);
    private void OP_0x98()               => processor.registers.A = Sbc(processor.registers.A, processor.registers.B);
    private void OP_0x99()               => processor.registers.A = Sbc(processor.registers.A, processor.registers.C);
    private void OP_0x9A()               => processor.registers.A = Sbc(processor.registers.A, processor.registers.D);
    private void OP_0x9B()               => processor.registers.A = Sbc(processor.registers.A, processor.registers.E);
    private void OP_0x9C()               => processor.registers.A = Sbc(processor.registers.A, processor.registers.H);
    private void OP_0x9D()               => processor.registers.A = Sbc(processor.registers.A, processor.registers.L);
    private void OP_0x9E()               => processor.registers.A = Sbc(processor.registers.A, processor.memory.ReadByte(processor.registers.HL));
    private void OP_0x9F()               => processor.registers.A = Sbc(processor.registers.A, processor.registers.A);


    private void OP_0xA0()               => And(processor.registers.B);
    private void OP_0xA1()               => And(processor.registers.C);
    private void OP_0xA2()               => And(processor.registers.D);
    private void OP_0xA3()               => And(processor.registers.E);
    private void OP_0xA4()               => And(processor.registers.H);
    private void OP_0xA5()               => And(processor.registers.L);
    private void OP_0xA6()               => And(processor.memory.ReadByte(processor.registers.HL));
    private void OP_0xA7()               => And(processor.registers.A);
    private void OP_0xA8()               => Xor(processor.registers.B);
    private void OP_0xA9()               => Xor(processor.registers.C);
    private void OP_0xAA()               => Xor(processor.registers.D);
    private void OP_0xAB()               => Xor(processor.registers.E);
    private void OP_0xAC()               => Xor(processor.registers.H);
    private void OP_0xAD()               => Xor(processor.registers.L);
    private void OP_0xAE()               => Xor(processor.memory.ReadByte(processor.registers.HL));
    private void OP_0xAF()               => Xor(processor.registers.A);

    private void OP_0xB0()               => Or(processor.registers.B);
    private void OP_0xB1()               => Or(processor.registers.C);
    private void OP_0xB2()               => Or(processor.registers.D);
    private void OP_0xB3()               => Or(processor.registers.E);
    private void OP_0xB4()               => Or(processor.registers.H);
    private void OP_0xB5()               => Or(processor.registers.L);
    private void OP_0xB6()               => Or(processor.memory.ReadByte(processor.registers.HL));
    private void OP_0xB7()               => Or(processor.registers.A);
    private void OP_0xB8()               => Cp(processor.registers.B);
    private void OP_0xB9()               => Cp(processor.registers.C);
    private void OP_0xBA()               => Cp(processor.registers.D);
    private void OP_0xBB()               => Cp(processor.registers.E);
    private void OP_0xBC()               => Cp(processor.registers.H);
    private void OP_0xBD()               => Cp(processor.registers.L);
    private void OP_0xBE()               => Cp(processor.memory.ReadByte(processor.registers.HL));
    private void OP_0xBF()               => Cp(processor.registers.A);

    private void OP_0xC0()               => Ret(CPUFlags.Zero, false);
    private void OP_0xC1()               => processor.registers.BC = Pop();
    private void OP_0xC2(ushort operand) => Jump(operand, CPUFlags.Zero, false);
    private void OP_0xC3(ushort operand) => Jump(operand);
    private void OP_0xC4(ushort operand) => Call(operand, CPUFlags.Zero, false);
    private void OP_0xC5()               => Push(processor.registers.BC);
    private void OP_0xC6(byte   operand) => processor.registers.A = Add(processor.registers.A, operand);
    private void OP_0xC7()               => Rst(0x0000);
    private void OP_0xC8()               => Ret(CPUFlags.Zero, true);
    private void OP_0xC9()               => processor.registers.PC = Pop();
    private void OP_0xCA(ushort operand) => Jump(operand, CPUFlags.Zero, true);
    private void OP_0xCC(ushort operand) => Call(operand, CPUFlags.Zero, true);
    private void OP_0xCD(ushort operand) => Call(operand);
    private void OP_0xCE(byte   operand) => processor.registers.A = Adc(processor.registers.A, operand);
    private void OP_0xCF()               => Rst(0x0008);

    private void OP_0xD0()               => Ret(CPUFlags.Carry, false);
    private void OP_0xD1()               => processor.registers.DE = Pop();
    private void OP_0xD2(ushort operand) => Jump(operand, CPUFlags.Carry, false);
    private void OP_0xD4(ushort operand) => Call(operand, CPUFlags.Carry, false);
    private void OP_0xD5()               => Push(processor.registers.DE);
    private void OP_0xD6(byte   operand) => processor.registers.A = Sub(processor.registers.A, operand);
    private void OP_0xD7()               => Rst(0x0010);
    private void OP_0xD8()               => Ret(CPUFlags.Carry, true);
    private void OP_0xD9()               => Reti();
    private void OP_0xDA(ushort operand) => Jump(operand, CPUFlags.Carry, true);
    private void OP_0xDC(ushort operand) => Call(operand, CPUFlags.Carry, true);
    private void OP_0xDE(byte   operand) => processor.registers.A = Sbc(processor.registers.A, operand);
    private void OP_0xDF()               => Rst(0x0018);

    private void OP_0xE0(byte operand)   => processor.memory.WriteByte((ushort)(0xFF00 + operand), processor.registers.A);
    private void OP_0xE1()               => processor.registers.HL = Pop();
    private void OP_0xE2()               => processor.memory.WriteByte((ushort)(0xFF00 + processor.registers.C), processor.registers.A);
    private void OP_0xE5()               => Push(processor.registers.HL);
    private void OP_0xE6(byte   operand) => And(operand);
    private void OP_0xE7()               => Rst(0x0020);
    private void OP_0xE8(byte   operand) => processor.registers.SP = AddSPRelative(operand);
    private void OP_0xE9()               => Jump(processor.registers.HL);
    private void OP_0xEA(ushort operand) => processor.memory.WriteByte(operand, processor.registers.A);
    private void OP_0xEE(byte operand)   => Xor(operand);
    private void OP_0xEF()               => Rst(0x0028);

    private void OP_0xF0(byte operand)   => processor.registers.A = processor.memory.ReadByte((ushort)(0xFF00 + operand));
    private void OP_0xF1()               => processor.registers.AF = (ushort)(Pop() & 0b_11111111_11110000);
    private void OP_0xF2()               => processor.registers.A = processor.memory.ReadByte((ushort)(0xFF00 + processor.registers.C));
    private void OP_0xF3()               => processor.interruptHandler.IME = false;
    private void OP_0xF5()               => Push(processor.registers.AF);
    private void OP_0xF6(byte   operand) => Or(operand);
    private void OP_0xF7()               => Rst(0x0030);
    private void OP_0xF8(byte   operand) => processor.registers.HL = AddSPRelative(operand);
    private void OP_0xF9()               => processor.registers.SP = processor.registers.HL;
    private void OP_0xFA(ushort operand) => processor.registers.A = processor.memory.ReadByte(operand);
    private void OP_0xFB()               => Ei();
    private void OP_0xFE(byte operand)   => Cp(operand);
    private void OP_0xFF()               => Rst(0x0038);

    private void OP_CB_0x00(byte fake)   => processor.registers.B = Rlc(processor.registers.B, true);
    private void OP_CB_0x01(byte fake)   => processor.registers.C = Rlc(processor.registers.C, true);
    private void OP_CB_0x02(byte fake)   => processor.registers.D = Rlc(processor.registers.D, true);
    private void OP_CB_0x03(byte fake)   => processor.registers.E = Rlc(processor.registers.E, true);
    private void OP_CB_0x04(byte fake)   => processor.registers.H = Rlc(processor.registers.H, true);
    private void OP_CB_0x05(byte fake)   => processor.registers.L = Rlc(processor.registers.L, true);
    private void OP_CB_0x06(byte fake)   => processor.memory.WriteByte(processor.registers.HL, Rlc(processor.memory.ReadByte(processor.registers.HL), true));
    private void OP_CB_0x07(byte fake)   => processor.registers.A = Rlc(processor.registers.A, true);
    private void OP_CB_0x08(byte fake)   => processor.registers.B = Rrc(processor.registers.B, true);
    private void OP_CB_0x09(byte fake)   => processor.registers.C = Rrc(processor.registers.C, true);
    private void OP_CB_0x0A(byte fake)   => processor.registers.D = Rrc(processor.registers.D, true);
    private void OP_CB_0x0B(byte fake)   => processor.registers.E = Rrc(processor.registers.E, true);
    private void OP_CB_0x0C(byte fake)   => processor.registers.H = Rrc(processor.registers.H, true);
    private void OP_CB_0x0D(byte fake)   => processor.registers.L = Rrc(processor.registers.L, true);
    private void OP_CB_0x0E(byte fake)   => processor.memory.WriteByte(processor.registers.HL, Rrc(processor.memory.ReadByte(processor.registers.HL), true));
    private void OP_CB_0x0F(byte fake)   => processor.registers.A = Rrc(processor.registers.A, true);

    private void OP_CB_0x10(byte fake)   => processor.registers.B = Rl(processor.registers.B, true);
    private void OP_CB_0x11(byte fake)   => processor.registers.C = Rl(processor.registers.C, true);
    private void OP_CB_0x12(byte fake)   => processor.registers.D = Rl(processor.registers.D, true);
    private void OP_CB_0x13(byte fake)   => processor.registers.E = Rl(processor.registers.E, true);
    private void OP_CB_0x14(byte fake)   => processor.registers.H = Rl(processor.registers.H, true);
    private void OP_CB_0x15(byte fake)   => processor.registers.L = Rl(processor.registers.L, true);
    private void OP_CB_0x16(byte fake)   => processor.memory.WriteByte(processor.registers.HL, Rl(processor.memory.ReadByte(processor.registers.HL), true));
    private void OP_CB_0x17(byte fake)   => processor.registers.A = Rl(processor.registers.A, true);
    private void OP_CB_0x18(byte fake)   => processor.registers.B = Rr(processor.registers.B, true);
    private void OP_CB_0x19(byte fake)   => processor.registers.C = Rr(processor.registers.C, true);
    private void OP_CB_0x1A(byte fake)   => processor.registers.D = Rr(processor.registers.D, true);
    private void OP_CB_0x1B(byte fake)   => processor.registers.E = Rr(processor.registers.E, true);
    private void OP_CB_0x1C(byte fake)   => processor.registers.H = Rr(processor.registers.H, true);
    private void OP_CB_0x1D(byte fake)   => processor.registers.L = Rr(processor.registers.L, true);
    private void OP_CB_0x1E(byte fake)   => processor.memory.WriteByte(processor.registers.HL, Rr(processor.memory.ReadByte(processor.registers.HL), true));
    private void OP_CB_0x1F(byte fake)   => processor.registers.A = Rr(processor.registers.A, true);

    private void OP_CB_0x20(byte fake)   => processor.registers.B = Sla(processor.registers.B);
    private void OP_CB_0x21(byte fake)   => processor.registers.C = Sla(processor.registers.C);
    private void OP_CB_0x22(byte fake)   => processor.registers.D = Sla(processor.registers.D);
    private void OP_CB_0x23(byte fake)   => processor.registers.E = Sla(processor.registers.E);
    private void OP_CB_0x24(byte fake)   => processor.registers.H = Sla(processor.registers.H);
    private void OP_CB_0x25(byte fake)   => processor.registers.L = Sla(processor.registers.L);
    private void OP_CB_0x26(byte fake)   => processor.memory.WriteByte(processor.registers.HL, Sla(processor.memory.ReadByte(processor.registers.HL)));
    private void OP_CB_0x27(byte fake)   => processor.registers.A = Sla(processor.registers.A);
    private void OP_CB_0x28(byte fake)   => processor.registers.B = Sra(processor.registers.B);
    private void OP_CB_0x29(byte fake)   => processor.registers.C = Sra(processor.registers.C);
    private void OP_CB_0x2A(byte fake)   => processor.registers.D = Sra(processor.registers.D);
    private void OP_CB_0x2B(byte fake)   => processor.registers.E = Sra(processor.registers.E);
    private void OP_CB_0x2C(byte fake)   => processor.registers.H = Sra(processor.registers.H);
    private void OP_CB_0x2D(byte fake)   => processor.registers.L = Sra(processor.registers.L);
    private void OP_CB_0x2E(byte fake)   => processor.memory.WriteByte(processor.registers.HL, Sra(processor.memory.ReadByte(processor.registers.HL)));
    private void OP_CB_0x2F(byte fake)   => processor.registers.A = Sra(processor.registers.A);

    private void OP_CB_0x30(byte fake)   => processor.registers.B = Swap(processor.registers.B);
    private void OP_CB_0x31(byte fake)   => processor.registers.C = Swap(processor.registers.C);
    private void OP_CB_0x32(byte fake)   => processor.registers.D = Swap(processor.registers.D);
    private void OP_CB_0x33(byte fake)   => processor.registers.E = Swap(processor.registers.E);
    private void OP_CB_0x34(byte fake)   => processor.registers.H = Swap(processor.registers.H);
    private void OP_CB_0x35(byte fake)   => processor.registers.L = Swap(processor.registers.L);
    private void OP_CB_0x36(byte fake)   => processor.memory.WriteByte(processor.registers.HL, Swap(processor.memory.ReadByte(processor.registers.HL)));
    private void OP_CB_0x37(byte fake)   => processor.registers.A = Swap(processor.registers.A);

    private void OP_CB_0x38(byte fake)   => processor.registers.B = Srl(processor.registers.B);
    private void OP_CB_0x39(byte fake)   => processor.registers.C = Srl(processor.registers.C);
    private void OP_CB_0x3A(byte fake)   => processor.registers.D = Srl(processor.registers.D);
    private void OP_CB_0x3B(byte fake)   => processor.registers.E = Srl(processor.registers.E);
    private void OP_CB_0x3C(byte fake)   => processor.registers.H = Srl(processor.registers.H);
    private void OP_CB_0x3D(byte fake)   => processor.registers.L = Srl(processor.registers.L);
    private void OP_CB_0x3E(byte fake)   => processor.memory.WriteByte(processor.registers.HL, Srl(processor.memory.ReadByte(processor.registers.HL)));
    private void OP_CB_0x3F(byte fake)   => processor.registers.A = Srl(processor.registers.A);

    private void OP_CB_0x40(byte fake)   => Bit(0, processor.registers.B);
    private void OP_CB_0x41(byte fake)   => Bit(0, processor.registers.C);
    private void OP_CB_0x42(byte fake)   => Bit(0, processor.registers.D);
    private void OP_CB_0x43(byte fake)   => Bit(0, processor.registers.E);
    private void OP_CB_0x44(byte fake)   => Bit(0, processor.registers.H);
    private void OP_CB_0x45(byte fake)   => Bit(0, processor.registers.L);
    private void OP_CB_0x46(byte fake)   => Bit(0, processor.memory.ReadByte(processor.registers.HL));
    private void OP_CB_0x47(byte fake)   => Bit(0, processor.registers.A);
    private void OP_CB_0x48(byte fake)   => Bit(1, processor.registers.B);
    private void OP_CB_0x49(byte fake)   => Bit(1, processor.registers.C);
    private void OP_CB_0x4A(byte fake)   => Bit(1, processor.registers.D);
    private void OP_CB_0x4B(byte fake)   => Bit(1, processor.registers.E);
    private void OP_CB_0x4C(byte fake)   => Bit(1, processor.registers.H);
    private void OP_CB_0x4D(byte fake)   => Bit(1, processor.registers.L);
    private void OP_CB_0x4E(byte fake)   => Bit(1, processor.memory.ReadByte(processor.registers.HL));
    private void OP_CB_0x4F(byte fake)   => Bit(1, processor.registers.A);

    private void OP_CB_0x50(byte fake)   => Bit(2, processor.registers.B);
    private void OP_CB_0x51(byte fake)   => Bit(2, processor.registers.C);
    private void OP_CB_0x52(byte fake)   => Bit(2, processor.registers.D);
    private void OP_CB_0x53(byte fake)   => Bit(2, processor.registers.E);
    private void OP_CB_0x54(byte fake)   => Bit(2, processor.registers.H);
    private void OP_CB_0x55(byte fake)   => Bit(2, processor.registers.L);
    private void OP_CB_0x56(byte fake)   => Bit(2, processor.memory.ReadByte(processor.registers.HL));
    private void OP_CB_0x57(byte fake)   => Bit(2, processor.registers.A);
    private void OP_CB_0x58(byte fake)   => Bit(3, processor.registers.B);
    private void OP_CB_0x59(byte fake)   => Bit(3, processor.registers.C);
    private void OP_CB_0x5A(byte fake)   => Bit(3, processor.registers.D);
    private void OP_CB_0x5B(byte fake)   => Bit(3, processor.registers.E);
    private void OP_CB_0x5C(byte fake)   => Bit(3, processor.registers.H);
    private void OP_CB_0x5D(byte fake)   => Bit(3, processor.registers.L);
    private void OP_CB_0x5E(byte fake)   => Bit(3, processor.memory.ReadByte(processor.registers.HL));
    private void OP_CB_0x5F(byte fake)   => Bit(3, processor.registers.A);

    private void OP_CB_0x60(byte fake)   => Bit(4, processor.registers.B);
    private void OP_CB_0x61(byte fake)   => Bit(4, processor.registers.C);
    private void OP_CB_0x62(byte fake)   => Bit(4, processor.registers.D);
    private void OP_CB_0x63(byte fake)   => Bit(4, processor.registers.E);
    private void OP_CB_0x64(byte fake)   => Bit(4, processor.registers.H);
    private void OP_CB_0x65(byte fake)   => Bit(4, processor.registers.L);
    private void OP_CB_0x66(byte fake)   => Bit(4, processor.memory.ReadByte(processor.registers.HL));
    private void OP_CB_0x67(byte fake)   => Bit(4, processor.registers.A);
    private void OP_CB_0x68(byte fake)   => Bit(5, processor.registers.B);
    private void OP_CB_0x69(byte fake)   => Bit(5, processor.registers.C);
    private void OP_CB_0x6A(byte fake)   => Bit(5, processor.registers.D);
    private void OP_CB_0x6B(byte fake)   => Bit(5, processor.registers.E);
    private void OP_CB_0x6C(byte fake)   => Bit(5, processor.registers.H);
    private void OP_CB_0x6D(byte fake)   => Bit(5, processor.registers.L);
    private void OP_CB_0x6E(byte fake)   => Bit(5, processor.memory.ReadByte(processor.registers.HL));
    private void OP_CB_0x6F(byte fake)   => Bit(5, processor.registers.A);

    private void OP_CB_0x70(byte fake)   => Bit(6, processor.registers.B);
    private void OP_CB_0x71(byte fake)   => Bit(6, processor.registers.C);
    private void OP_CB_0x72(byte fake)   => Bit(6, processor.registers.D);
    private void OP_CB_0x73(byte fake)   => Bit(6, processor.registers.E);
    private void OP_CB_0x74(byte fake)   => Bit(6, processor.registers.H);
    private void OP_CB_0x75(byte fake)   => Bit(6, processor.registers.L);
    private void OP_CB_0x76(byte fake)   => Bit(6, processor.memory.ReadByte(processor.registers.HL));
    private void OP_CB_0x77(byte fake)   => Bit(6, processor.registers.A);
    private void OP_CB_0x78(byte fake)   => Bit(7, processor.registers.B);
    private void OP_CB_0x79(byte fake)   => Bit(7, processor.registers.C);
    private void OP_CB_0x7A(byte fake)   => Bit(7, processor.registers.D);
    private void OP_CB_0x7B(byte fake)   => Bit(7, processor.registers.E);
    private void OP_CB_0x7C(byte fake)   => Bit(7, processor.registers.H);
    private void OP_CB_0x7D(byte fake)   => Bit(7, processor.registers.L);
    private void OP_CB_0x7E(byte fake)   => Bit(7, processor.memory.ReadByte(processor.registers.HL));
    private void OP_CB_0x7F(byte fake)   => Bit(7, processor.registers.A);

    private void OP_CB_0x80(byte fake)   => processor.registers.B = Res(0, processor.registers.B);
    private void OP_CB_0x81(byte fake)   => processor.registers.C = Res(0, processor.registers.C);
    private void OP_CB_0x82(byte fake)   => processor.registers.D = Res(0, processor.registers.D);
    private void OP_CB_0x83(byte fake)   => processor.registers.E = Res(0, processor.registers.E);
    private void OP_CB_0x84(byte fake)   => processor.registers.H = Res(0, processor.registers.H);
    private void OP_CB_0x85(byte fake)   => processor.registers.L = Res(0, processor.registers.L);
    private void OP_CB_0x86(byte fake)   => processor.memory.WriteByte(processor.registers.HL, Res(0, processor.memory.ReadByte(processor.registers.HL)));
    private void OP_CB_0x87(byte fake)   => processor.registers.A = Res(0, processor.registers.A);
    private void OP_CB_0x88(byte fake)   => processor.registers.B = Res(1, processor.registers.B);
    private void OP_CB_0x89(byte fake)   => processor.registers.C = Res(1, processor.registers.C);
    private void OP_CB_0x8A(byte fake)   => processor.registers.D = Res(1, processor.registers.D);
    private void OP_CB_0x8B(byte fake)   => processor.registers.E = Res(1, processor.registers.E);
    private void OP_CB_0x8C(byte fake)   => processor.registers.H = Res(1, processor.registers.H);
    private void OP_CB_0x8D(byte fake)   => processor.registers.L = Res(1, processor.registers.L);
    private void OP_CB_0x8E(byte fake)   => processor.memory.WriteByte(processor.registers.HL, Res(1, processor.memory.ReadByte(processor.registers.HL)));
    private void OP_CB_0x8F(byte fake)   => processor.registers.A = Res(1, processor.registers.A);

    private void OP_CB_0x90(byte fake)   => processor.registers.B = Res(2, processor.registers.B);
    private void OP_CB_0x91(byte fake)   => processor.registers.C = Res(2, processor.registers.C);
    private void OP_CB_0x92(byte fake)   => processor.registers.D = Res(2, processor.registers.D);
    private void OP_CB_0x93(byte fake)   => processor.registers.E = Res(2, processor.registers.E);
    private void OP_CB_0x94(byte fake)   => processor.registers.H = Res(2, processor.registers.H);
    private void OP_CB_0x95(byte fake)   => processor.registers.L = Res(2, processor.registers.L);
    private void OP_CB_0x96(byte fake)   => processor.memory.WriteByte(processor.registers.HL, Res(2, processor.memory.ReadByte(processor.registers.HL)));
    private void OP_CB_0x97(byte fake)   => processor.registers.A = Res(2, processor.registers.A);
    private void OP_CB_0x98(byte fake)   => processor.registers.B = Res(3, processor.registers.B);
    private void OP_CB_0x99(byte fake)   => processor.registers.C = Res(3, processor.registers.C);
    private void OP_CB_0x9A(byte fake)   => processor.registers.D = Res(3, processor.registers.D);
    private void OP_CB_0x9B(byte fake)   => processor.registers.E = Res(3, processor.registers.E);
    private void OP_CB_0x9C(byte fake)   => processor.registers.H = Res(3, processor.registers.H);
    private void OP_CB_0x9D(byte fake)   => processor.registers.L = Res(3, processor.registers.L);
    private void OP_CB_0x9E(byte fake)   => processor.memory.WriteByte(processor.registers.HL, Res(3, processor.memory.ReadByte(processor.registers.HL)));
    private void OP_CB_0x9F(byte fake)   => processor.registers.A = Res(3, processor.registers.A);

    private void OP_CB_0xA0(byte fake)   => processor.registers.B = Res(4, processor.registers.B);
    private void OP_CB_0xA1(byte fake)   => processor.registers.C = Res(4, processor.registers.C);
    private void OP_CB_0xA2(byte fake)   => processor.registers.D = Res(4, processor.registers.D);
    private void OP_CB_0xA3(byte fake)   => processor.registers.E = Res(4, processor.registers.E);
    private void OP_CB_0xA4(byte fake)   => processor.registers.H = Res(4, processor.registers.H);
    private void OP_CB_0xA5(byte fake)   => processor.registers.L = Res(4, processor.registers.L);
    private void OP_CB_0xA6(byte fake)   => processor.memory.WriteByte(processor.registers.HL, Res(4, processor.memory.ReadByte(processor.registers.HL)));
    private void OP_CB_0xA7(byte fake)   => processor.registers.A = Res(4, processor.registers.A);
    private void OP_CB_0xA8(byte fake)   => processor.registers.B = Res(5, processor.registers.B);
    private void OP_CB_0xA9(byte fake)   => processor.registers.C = Res(5, processor.registers.C);
    private void OP_CB_0xAA(byte fake)   => processor.registers.D = Res(5, processor.registers.D);
    private void OP_CB_0xAB(byte fake)   => processor.registers.E = Res(5, processor.registers.E);
    private void OP_CB_0xAC(byte fake)   => processor.registers.H = Res(5, processor.registers.H);
    private void OP_CB_0xAD(byte fake)   => processor.registers.L = Res(5, processor.registers.L);
    private void OP_CB_0xAE(byte fake)   => processor.memory.WriteByte(processor.registers.HL, Res(5, processor.memory.ReadByte(processor.registers.HL)));
    private void OP_CB_0xAF(byte fake)   => processor.registers.A = Res(5, processor.registers.A);

    private void OP_CB_0xB0(byte fake)   => processor.registers.B = Res(6, processor.registers.B);
    private void OP_CB_0xB1(byte fake)   => processor.registers.C = Res(6, processor.registers.C);
    private void OP_CB_0xB2(byte fake)   => processor.registers.D = Res(6, processor.registers.D);
    private void OP_CB_0xB3(byte fake)   => processor.registers.E = Res(6, processor.registers.E);
    private void OP_CB_0xB4(byte fake)   => processor.registers.H = Res(6, processor.registers.H);
    private void OP_CB_0xB5(byte fake)   => processor.registers.L = Res(6, processor.registers.L);
    private void OP_CB_0xB6(byte fake)   => processor.memory.WriteByte(processor.registers.HL, Res(6, processor.memory.ReadByte(processor.registers.HL)));
    private void OP_CB_0xB7(byte fake)   => processor.registers.A = Res(6, processor.registers.A);
    private void OP_CB_0xB8(byte fake)   => processor.registers.B = Res(7, processor.registers.B);
    private void OP_CB_0xB9(byte fake)   => processor.registers.C = Res(7, processor.registers.C);
    private void OP_CB_0xBA(byte fake)   => processor.registers.D = Res(7, processor.registers.D);
    private void OP_CB_0xBB(byte fake)   => processor.registers.E = Res(7, processor.registers.E);
    private void OP_CB_0xBC(byte fake)   => processor.registers.H = Res(7, processor.registers.H);
    private void OP_CB_0xBD(byte fake)   => processor.registers.L = Res(7, processor.registers.L);
    private void OP_CB_0xBE(byte fake)   => processor.memory.WriteByte(processor.registers.HL, Res(7, processor.memory.ReadByte(processor.registers.HL)));
    private void OP_CB_0xBF(byte fake)   => processor.registers.A = Res(7, processor.registers.A);

    private void OP_CB_0xC0(byte fake)   => processor.registers.B = Set(0, processor.registers.B);
    private void OP_CB_0xC1(byte fake)   => processor.registers.C = Set(0, processor.registers.C);
    private void OP_CB_0xC2(byte fake)   => processor.registers.D = Set(0, processor.registers.D);
    private void OP_CB_0xC3(byte fake)   => processor.registers.E = Set(0, processor.registers.E);
    private void OP_CB_0xC4(byte fake)   => processor.registers.H = Set(0, processor.registers.H);
    private void OP_CB_0xC5(byte fake)   => processor.registers.L = Set(0, processor.registers.L);
    private void OP_CB_0xC6(byte fake)   => processor.memory.WriteByte(processor.registers.HL, Set(0, processor.memory.ReadByte(processor.registers.HL)));
    private void OP_CB_0xC7(byte fake)   => processor.registers.A = Set(0, processor.registers.A);
    private void OP_CB_0xC8(byte fake)   => processor.registers.B = Set(1, processor.registers.B);
    private void OP_CB_0xC9(byte fake)   => processor.registers.C = Set(1, processor.registers.C);
    private void OP_CB_0xCA(byte fake)   => processor.registers.D = Set(1, processor.registers.D);
    private void OP_CB_0xCB(byte fake)   => processor.registers.E = Set(1, processor.registers.E);
    private void OP_CB_0xCC(byte fake)   => processor.registers.H = Set(1, processor.registers.H);
    private void OP_CB_0xCD(byte fake)   => processor.registers.L = Set(1, processor.registers.L);
    private void OP_CB_0xCE(byte fake)   => processor.memory.WriteByte(processor.registers.HL, Set(1, processor.memory.ReadByte(processor.registers.HL)));
    private void OP_CB_0xCF(byte fake)   => processor.registers.A = Set(1, processor.registers.A);
                                         
    private void OP_CB_0xD0(byte fake)   => processor.registers.B = Set(2, processor.registers.B);
    private void OP_CB_0xD1(byte fake)   => processor.registers.C = Set(2, processor.registers.C);
    private void OP_CB_0xD2(byte fake)   => processor.registers.D = Set(2, processor.registers.D);
    private void OP_CB_0xD3(byte fake)   => processor.registers.E = Set(2, processor.registers.E);
    private void OP_CB_0xD4(byte fake)   => processor.registers.H = Set(2, processor.registers.H);
    private void OP_CB_0xD5(byte fake)   => processor.registers.L = Set(2, processor.registers.L);
    private void OP_CB_0xD6(byte fake)   => processor.memory.WriteByte(processor.registers.HL, Set(2, processor.memory.ReadByte(processor.registers.HL)));
    private void OP_CB_0xD7(byte fake)   => processor.registers.A = Set(2, processor.registers.A);
    private void OP_CB_0xD8(byte fake)   => processor.registers.B = Set(3, processor.registers.B);
    private void OP_CB_0xD9(byte fake)   => processor.registers.C = Set(3, processor.registers.C);
    private void OP_CB_0xDA(byte fake)   => processor.registers.D = Set(3, processor.registers.D);
    private void OP_CB_0xDB(byte fake)   => processor.registers.E = Set(3, processor.registers.E);
    private void OP_CB_0xDC(byte fake)   => processor.registers.H = Set(3, processor.registers.H);
    private void OP_CB_0xDD(byte fake)   => processor.registers.L = Set(3, processor.registers.L);
    private void OP_CB_0xDE(byte fake)   => processor.memory.WriteByte(processor.registers.HL, Set(3, processor.memory.ReadByte(processor.registers.HL)));
    private void OP_CB_0xDF(byte fake)   => processor.registers.A = Set(3, processor.registers.A);
                                         
    private void OP_CB_0xE0(byte fake)   => processor.registers.B = Set(4, processor.registers.B);
    private void OP_CB_0xE1(byte fake)   => processor.registers.C = Set(4, processor.registers.C);
    private void OP_CB_0xE2(byte fake)   => processor.registers.D = Set(4, processor.registers.D);
    private void OP_CB_0xE3(byte fake)   => processor.registers.E = Set(4, processor.registers.E);
    private void OP_CB_0xE4(byte fake)   => processor.registers.H = Set(4, processor.registers.H);
    private void OP_CB_0xE5(byte fake)   => processor.registers.L = Set(4, processor.registers.L);
    private void OP_CB_0xE6(byte fake)   => processor.memory.WriteByte(processor.registers.HL, Set(4, processor.memory.ReadByte(processor.registers.HL)));
    private void OP_CB_0xE7(byte fake)   => processor.registers.A = Set(4, processor.registers.A);
    private void OP_CB_0xE8(byte fake)   => processor.registers.B = Set(5, processor.registers.B);
    private void OP_CB_0xE9(byte fake)   => processor.registers.C = Set(5, processor.registers.C);
    private void OP_CB_0xEA(byte fake)   => processor.registers.D = Set(5, processor.registers.D);
    private void OP_CB_0xEB(byte fake)   => processor.registers.E = Set(5, processor.registers.E);
    private void OP_CB_0xEC(byte fake)   => processor.registers.H = Set(5, processor.registers.H);
    private void OP_CB_0xED(byte fake)   => processor.registers.L = Set(5, processor.registers.L);
    private void OP_CB_0xEE(byte fake)   => processor.memory.WriteByte(processor.registers.HL, Set(5, processor.memory.ReadByte(processor.registers.HL)));
    private void OP_CB_0xEF(byte fake)   => processor.registers.A = Set(5, processor.registers.A);
                                         
    private void OP_CB_0xF0(byte fake)   => processor.registers.B = Set(6, processor.registers.B);
    private void OP_CB_0xF1(byte fake)   => processor.registers.C = Set(6, processor.registers.C);
    private void OP_CB_0xF2(byte fake)   => processor.registers.D = Set(6, processor.registers.D);
    private void OP_CB_0xF3(byte fake)   => processor.registers.E = Set(6, processor.registers.E);
    private void OP_CB_0xF4(byte fake)   => processor.registers.H = Set(6, processor.registers.H);
    private void OP_CB_0xF5(byte fake)   => processor.registers.L = Set(6, processor.registers.L);
    private void OP_CB_0xF6(byte fake)   => processor.memory.WriteByte(processor.registers.HL, Set(6, processor.memory.ReadByte(processor.registers.HL)));
    private void OP_CB_0xF7(byte fake)   => processor.registers.A = Set(6, processor.registers.A);
    private void OP_CB_0xF8(byte fake)   => processor.registers.B = Set(7, processor.registers.B);
    private void OP_CB_0xF9(byte fake)   => processor.registers.C = Set(7, processor.registers.C);
    private void OP_CB_0xFA(byte fake)   => processor.registers.D = Set(7, processor.registers.D);
    private void OP_CB_0xFB(byte fake)   => processor.registers.E = Set(7, processor.registers.E);
    private void OP_CB_0xFC(byte fake)   => processor.registers.H = Set(7, processor.registers.H);
    private void OP_CB_0xFD(byte fake)   => processor.registers.L = Set(7, processor.registers.L);
    private void OP_CB_0xFE(byte fake)   => processor.memory.WriteByte(processor.registers.HL, Set(7, processor.memory.ReadByte(processor.registers.HL)));
    private void OP_CB_0xFF(byte fake)   => processor.registers.A = Set(7, processor.registers.A);
}
