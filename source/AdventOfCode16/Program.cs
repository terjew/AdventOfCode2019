using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace AdventOfCode16
{
    class Program
    {
        static List<int[]> FactorArrays = new List<int[]>();
        
        public static void PhaseShift100(int[] input)
        {
            int[] output = new int[input.Length];
            int[] basePattern = { 0, 1, 0, -1 };
            FactorArrays.Clear();
            for (int l = 0; l < input.Length; l++)
            {
                var sequence = basePattern.SelectMany(b => Enumerable.Repeat(b, l + 1));
                var sequenceCounts = (input.Length + 1) / ((l + 1) * 4);
                var fullSequence = Enumerable.Repeat(sequence, sequenceCounts + 1).SelectMany(b => b);
                FactorArrays.Add(fullSequence.Skip(1).Take(input.Length).ToArray());
            }
            for (int i = 0; i < 50; i++)
            {
                ApplyFFT(input, output);
                ApplyFFT(output, input);
            }
        }

        static int[] ReadInput(string input)
        {
            return input.Select(b => (int)(b - '0')).ToArray();
        }

        static string Calculate(string input)
        {
            var inputArray = ReadInput(input.Trim());
            //inputArray = Enumerable.Repeat(inputArray, 10000).SelectMany(i => i).ToArray();
            PhaseShift100(inputArray);
            return string.Join("", inputArray.Take(8));
        }

        static void Main(string[] args)
        {
            var input = File.ReadAllText("./input.txt");

            var result1 = Calculate("80871224585914546619083218645595"); //24176176.
            //var result2 = Calculate("19617804207202209144916044189917"); //73745418.
            //var result3 = Calculate("69317163492948606335995924319873"); //52432133.

            var result = Calculate(input);
            //PhaseShift100(input);
            //var output = 

            //"19617804207202209144916044189917"; //73745418.
            //"69317163492948606335995924319873"; //52432133.


        }

        static void ApplyFFT(int[] input, int[] output)
        {
            var length = input.Length;
            for (int line = 0; line < length; line++)
            {
                //int size = Vector<byte>.Count;
                //Vector<byte> input = new Vector<byte>();

                long sum = 0;
                int[] factors = FactorArrays[line];
                for (int i = 0; i < length; i++)
                {
                    sum += input[i] * factors[i];
                }
                output[line] = int.Parse("" + sum.ToString().Last());
            }
        }

        //Note to self: For the larger problem case, try the following:
        //Look at the 100th iteration, and identify the 8 digits asked for.
        //look at which digits from step 99 are combined to create these, 
        //and go back another step to find the digits in step 98 used to create those again.

        //Hopefully, this can be tracked all the way back to iteration 0, so not all the digits have to be calculated all the way through.
    }
}
