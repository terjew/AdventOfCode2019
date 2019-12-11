using System;
using System.IO;
using System.Linq;

namespace AdventOfCode2
{
    class Program
    {
        public static int ExecuteInstruction(int[] memory, int pc)
        {
            int opcode = memory[pc];
            if (opcode == 99) return -1;
            int operand1Address = memory[pc + 1];
            int operand2Address = memory[pc + 2];
            int outputAddress = memory[pc + 3];

            int op1 = memory[operand1Address];
            int op2 = memory[operand2Address];

            switch (opcode)
            {
                case 1:
                    memory[outputAddress] = op1 + op2;
                    break;
                case 2:
                    memory[outputAddress] = op1 * op2;
                    break;
                default:
                    throw new InvalidOperationException();
            }
            return pc + 4;
        }

        public static void RunProgram(int[] memory)
        {
            int pc = 0;
            while (pc >= 0)
            {
                pc = ExecuteInstruction(memory, pc);
            }
        }

        static Tuple<int, int> FindInput(int[] program, int desiredOutput)
        {
            for (int noun = 0; noun < 100; noun++)
            {
                for (int verb = 0; verb < 100; verb++)
                {
                    var mem = program.ToArray();
                    mem[1] = noun;
                    mem[2] = verb;
                    RunProgram(mem);
                    if (mem[0] == desiredOutput) return new Tuple<int, int>(noun, verb);
                }
            }
            throw new InvalidOperationException("Could not find correct input");
        }
        static void Main(string[] args)
        {
            var programText = File.ReadAllText("./input.txt");            
            var program = programText.Split(",").Select(s => int.Parse(s)).ToArray();
            var input = FindInput(program, 19690720);

            Console.WriteLine($"Program finished, value of 19690720 found with input {input.Item1}{input.Item2}" );
            while (!Console.KeyAvailable) { }
        }
    }
}
