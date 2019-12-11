using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace AdventOfCode7
{
    class Program
    {
        static void Main(string[] args)
        {
            //var programText = "3,52,1001,52,-5,52,3,53,1,52,56,54,1007,54,5,55,1005,55,26,1001,54,-5,54,1105,1,12,1,53,54,53,1008,54,0,55,1001,55,1,55,2,53,55,53,4,53,1001,56,-1,56,1005,56,6,99,0,0,0,0,10";
            var programText = File.ReadAllText("./input.txt");

            //FindBestThrust(programText, Enumerable.Range(0, 5).ToArray(), CalculateThrust);
            FindBestThrust(programText, Enumerable.Range(5, 5).ToArray(), CalculateThrustFeedback);
        }

        public static void FindBestThrust(string programText, int[] options,  Func<int[], string, int> calculateThrustFunc)
        {
            int tries = 0;
            int highestThrust = 0;
            int[] bestSettings = null;

            List<int[]> permutations = new List<int[]>();
            GeneratePermutations(options, options.Length, permutations);

            foreach (var phaseSettings in permutations)
            {
                var thrust = calculateThrustFunc(phaseSettings, programText);
                if (thrust > highestThrust)
                {
                    highestThrust = thrust;
                    bestSettings = phaseSettings;
                }
                tries++;
                //Console.WriteLine($"Found thrust value {thrust} with phase settings [{string.Join(",", phaseSettings)}]");

            }

            Console.WriteLine($"Best thrust value was {highestThrust} found with phase settings [{string.Join(",", bestSettings)}] ({tries} tries)");
        }


        //Heap's algorithm:
        public static void GeneratePermutations<T>(T[] array, int n, List<T[]> permutations)
        {
            if (n == 1)
            {
                // (got a new permutation)
                permutations.Add(array.ToArray()); //clone the ordered array
                return;
            }
            GeneratePermutations(array, n - 1, permutations);
            for (int i = 0; i < (n - 1); i++)
            {
                // always swap the first when odd,
                // swap the i-th when even
                if (n % 2 == 0) Swap(array, n - 1, i);
                else Swap(array, n - 1, 0);
                GeneratePermutations(array, n - 1, permutations);
            }
    }

        public static void Swap<T>(T[] array, int a, int b)
        {
            T tmp = array[a];
            array[a] = array[b];
            array[b] = tmp;
        }

        static int CalculateThrust(int[] phaseSettings, string programText)
        {
            int output = 0;
            for (int i = 0; i < phaseSettings.Length; i++)
            {
                int j = 0;
                var inputValues = new[] { phaseSettings[i], output };
                var computer = new IntComputer(programText);
                computer.Input = () => (long)inputValues[j++];
                computer.Output = (v) => output = (int)v;
                computer.Run();
            }
            return output;
        }

        static int CalculateThrustFeedback(int[] phaseSettings, string programText)
        {
            //int output = 0;
            var firstInput = Channel.CreateBounded<int>(1);
            var previousOutput = firstInput;

            var tasks = new List<Task>();
            int lastOutput = 0;
            for (int i = 0; i < phaseSettings.Length; i++)
            {
                int stage = i;
                var computer = new IntComputer(programText);

                var output = i < phaseSettings.Length - 1 ? Channel.CreateBounded<int>(1) : firstInput;
                var input = previousOutput;
                input.Writer.TryWrite(phaseSettings[i]);
                computer.Input = () =>
                {
                    var inputTask = Task.Run(async () =>
                    {
                        await input.Reader.WaitToReadAsync();
                        input.Reader.TryRead(out var val);
                        return val;
                    });
                    return inputTask.Result;
                };

                computer.Output = (v) =>
                {
                    output.Writer.TryWrite((int)v);
                    if (stage == 4) lastOutput = (int)v;
                };

                previousOutput = output;
                tasks.Add(Task.Run(() => computer.Run()));
            }

            var startTask = Task.Run(async () =>
            {
                await firstInput.Writer.WaitToWriteAsync();
                firstInput.Writer.TryWrite(0);
            });
            startTask.Wait();

            Task.WaitAll(tasks.ToArray());
            return lastOutput;
        }

    }
}
