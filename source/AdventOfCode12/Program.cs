using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;

using Common;

namespace AdventOfCode12
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllLines("./input.txt").Select(l => ParseVector(l)).ToList();
            List<int[]> axisPositions = new List<int[]>()
            {
                input.Select(p => (int)p.X).ToArray(),
                input.Select(p => (int)p.Y).ToArray(),
                input.Select(p => (int)p.Z).ToArray(),
            };

            int[] periods = new int[3];

            for (int axis = 0; axis < 3; axis++)
            {
                var positions = axisPositions[axis];
                var initialPositions = (int[])positions.Clone();

                var velocities = Enumerable.Repeat(0, 4).ToArray();
                int count = 1;
                while (true)
                {
                    StepSimulationAxis(positions, velocities);
                    count++;

                    if (positions.SequenceEqual(initialPositions))
                    {
                        Console.WriteLine($"Axis {axis} repeated after {count} steps");
                        periods[axis] = count;
                        break;
                    }
                }
            }
            var systemPeriod = LCM(periods[0], LCM(periods[1], periods[2]));

            Console.WriteLine($"Period of the entire system is {systemPeriod}");
        }

        public static long LCM(long a, long b)
        {
            long num1, num2;
            if (a > b)
            {
                num1 = a; num2 = b;
            }
            else
            {
                num1 = b; num2 = a;
            }

            for (long i = 1; i < num2; i++)
            {
                if ((num1 * i) % num2 == 0)
                {
                    return i * num1;
                }
            }
            return num1 * num2;
        }

        private static void StepSimulationAxis(int[] positions, int[] velocities)
        {
            for (int i = 0; i < positions.Length; i++)
            {
                for (int j = i + 1; j < positions.Length; j++)
                {
                    var a = positions[i];
                    var b = positions[j];
                    var diff = a - b;
                    var sign = Math.Sign(diff);
                    velocities[i] -= sign;
                    velocities[j] += sign;
                }
            }

            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] += velocities[i];
            }
        }

        private static int CalculateEnergy(List<Vector3> positions, List<Vector3> velocities)
        {
            var potential = positions.Select(p => Math.Abs(p.X) + Math.Abs(p.Y) + Math.Abs(p.Z));
            var kinetic = velocities.Select(v => Math.Abs(v.X) + Math.Abs(v.Y) + Math.Abs(v.Z));

            return (int)potential.Zip(kinetic).Select(pair => pair.First * pair.Second).Sum();
        }

        private static void StepSimulation(List<Vector3> positions, List<Vector3> velocities)
        {
            for(int i = 0; i < positions.Count; i++)
            {
                for(int j=i + 1; j < positions.Count; j++)
                {
                    var a = positions[i];
                    var b = positions[j];
                    var diff = a - b;
                    var sign = new Vector3(Math.Sign(diff.X), Math.Sign(diff.Y), Math.Sign(diff.Z));
                    velocities[i] -= sign;
                    velocities[j] += sign;
                }
            }

            for (int i = 0; i < positions.Count; i++)
            {
                positions[i] += velocities[i];
            }
        }

        private static Vector3 ParseVector(string l)
        {
            Regex re = new Regex(@"((?:(?:x=(?<x>\-?\d+))|(?:y=(?<y>\-?\d+))|(?:z=(?<z>\-?\d+)))[\s,]*){3}", RegexOptions.Compiled);
            var match = re.Match(l);
            return new Vector3(match.GetInt("x"), match.GetInt("y"), match.GetInt("z"));
        }
    }
}
