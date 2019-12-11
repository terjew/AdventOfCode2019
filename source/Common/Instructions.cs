using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public enum Instruction
    {
        Add = 1,
        Multiply,
        Input,
        Output,
        JumpIfTrue,
        JumpIfFalse,
        LessThan,
        Equals,
        SetBaseOffset,
        Terminate = 99
    }

    public static class Instructions
    {
        public static Dictionary<Instruction, int> size = new Dictionary<Instruction, int>()
        {
            {Instruction.Add, 4 },
            {Instruction.Multiply, 4 },
            {Instruction.Input, 2 },
            {Instruction.Output, 2 },
            {Instruction.JumpIfTrue, 3 },
            {Instruction.JumpIfFalse, 3 },
            {Instruction.LessThan, 4 },
            {Instruction.Equals, 4 },
            {Instruction.SetBaseOffset, 2 },
            {Instruction.Terminate, 1 },
        };
    }
}
