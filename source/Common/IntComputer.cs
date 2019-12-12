using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    public class IntComputer
    {
        public enum AddressMode
        {
            Pointer = 0,
            Value,
            Relative
        }

        
        private long[] memory;
        private int pc;
        private long relativeBaseOffset = 0;
        private AddressMode[] addressModes;

        private long GetOperand(int parameter)
        {
            long opAddr = memory[pc + parameter + 1];

            switch (addressModes[parameter])
            {
                case AddressMode.Pointer:
                    return memory[opAddr];
                case AddressMode.Value:
                    return opAddr;
                case AddressMode.Relative:
                    return memory[opAddr + relativeBaseOffset];
                default:
                    throw new ArgumentException("Invalid address mode");
            }
        }

        private void Store(long result, int parameter)
        {
            long outputAddress = memory[pc + parameter + 1];
            if (addressModes[parameter] == AddressMode.Relative)
            {
                outputAddress += relativeBaseOffset;
            }
            memory[outputAddress] = result;
        }

        private int ExecuteInstruction()
        {
            long opcode = memory[pc];
            var digits = opcode.ToString().Select(c => c - '0');
            addressModes = digits.Reverse().Skip(2).Select(d => (AddressMode)d).Concat(Enumerable.Repeat(AddressMode.Pointer, 3)).Take(3).ToArray();

            var instruction = (Instruction)((opcode % 1000) % 100);
            int opSize = Instructions.size[instruction];
            switch (instruction)
            {
                case Instruction.Add:
                    Store(GetOperand(0) + GetOperand(1), 2);
                    break;
                case Instruction.Multiply:
                    Store(GetOperand(0) * GetOperand(1), 2);
                    break;
                case Instruction.Input:
                    Store(Input(), 0);
                    break;
                case Instruction.Output:
                    Output(GetOperand(0));
                    break;
                case Instruction.JumpIfTrue:
                    if (GetOperand(0) != 0) return (int)GetOperand(1);
                    break;
                case Instruction.JumpIfFalse:
                    if (GetOperand(0) == 0) return (int)GetOperand(1);
                    break;
                case Instruction.LessThan:
                    Store(GetOperand(0) < GetOperand(1) ? 1 : 0, 2);
                    break;
                case Instruction.Equals:
                    Store(GetOperand(0) == GetOperand(1) ? 1 : 0, 2);
                    break;
                case Instruction.SetBaseOffset:
                    relativeBaseOffset += GetOperand(0);
                    break;
                case Instruction.Terminate:
                    return -1;
                default:
                    throw new InvalidOperationException();
            }
            return pc + opSize;
        }

        private void ConsoleOutput(long value)
        {
            Console.WriteLine(value);
        }

        private long ConsoleInput()
        {
            return long.Parse(Console.ReadLine());
        }

        public void Run()
        {
            pc = 0;
            while (pc >= 0)
            {
                pc = ExecuteInstruction();
            }
        }

        public IntComputer(string programText)
        {
            memory = Enumerable.Repeat(0L, 8192).ToArray();
            var program = programText.Split(",").Select(s => long.Parse(s)).ToArray();
            Array.Copy(program, 0, memory, 0, program.Length);
                
            Input = ConsoleInput;
            Output = ConsoleOutput;
        }

        public Func<long> Input { get; set; }
        public Action<long> Output { get; set; }

    }
}