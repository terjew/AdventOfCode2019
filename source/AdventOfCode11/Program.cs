using Common;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Numerics;

namespace AdventOfCode11
{
    public struct Vec2i : IEquatable<Vec2i>
    {
        public int X { get; }
        public int Y { get; }
        public Vec2i(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            return obj is Vec2i i && Equals(i);
        }

        public bool Equals([AllowNull] Vec2i other)
        {
            return X == other.X &&
                   Y == other.Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public static Vec2i operator +(Vec2i a, Vec2i b)
        {
            return new Vec2i(a.X + b.X, a.Y + b.Y);
        }

        public static Vec2i operator -(Vec2i a, Vec2i b)
        {
            return new Vec2i(a.X - b.X, a.Y - b.Y);
        }

        public static bool operator==(Vec2i a, Vec2i b)
        {
            return a.Equals(b);
        }

        public static bool operator!=(Vec2i a, Vec2i b)
        {
            return !a.Equals(b);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            int dims = 96;

            bool[,] map = new bool[dims, dims];
            Vec2i pos = new Vec2i(dims / 2, dims / 2);
            HashSet<Vec2i> painted = new HashSet<Vec2i>();
            Dictionary<int, Vec2i> offsets = new Dictionary<int, Vec2i>()
            {
                {0, new Vec2i(1,0) },
                {90, new Vec2i(0,1) },
                {180, new Vec2i(-1,0) },
                {270, new Vec2i(0,-1) },
            };

            int direction = 90;
            int count = 0;
            bool white = false;
            map[pos.X, pos.Y] = true;

            var computer = new IntComputer(File.ReadAllText("./input.txt"));

            computer.Input = () =>
            {
                return map[pos.X, pos.Y] ? 1 : 0;
            };

            computer.Output = (o) =>
            {
                if (count % 2 == 0)
                {
                    white = o == 1;
                    map[pos.X, pos.Y] = white;
                    painted.Add(pos);
                }
                else
                {
                    direction = (direction + (o == 1 ? -90 : 90) + 720) % 360;
                    var offset = offsets[direction];
                    pos += offset;
                }

                count++;
            };

            computer.Run();

            Console.WriteLine($"Finished execution, {painted.Count} tiles were painted");
            for (int y = 0; y < dims; y++)
            {
                for (int x = 0; x < dims; x++)
                {
                    Console.Write(map[x, dims - y - 1] ? "*" : " ");
                }
                Console.WriteLine();
            }

        }

    }
}
