using AdventOfCode11;
using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode17
{
    class Program
    {
        private static void SetTile(Vec2i pos, char value)
        {
            map[pos.Y, pos.X] = value;
        }

        private static char GetTile(Vec2i pos)
        {
            if (pos.X < 0 || pos.X > width - 1) return (char)0;
            if (pos.Y < 0 || pos.Y > height - 1) return (char)0;
            return map[pos.Y, pos.X];
        }

        static Vec2i Up = new Vec2i(0, -1);
        static Vec2i Right = new Vec2i(1, 0);
        static Vec2i Down = new Vec2i(0, 1);
        static Vec2i Left = new Vec2i(-1, 0);

        private static char[,] map;
        private static int width;
        private static int height;

        private static IEnumerable<Vec2i> EnumerateNeighbors(Vec2i pos)
        {
            if (pos.Y != 0) yield return pos + Up;
            if (pos.Y != height - 1) yield return pos + Down;
            if (pos.X != 0) yield return pos + Left;
            if (pos.X != width - 1) yield return pos + Right;
        }

        private static IEnumerable<Vec2i> EnumerateTiles()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var pos = new Vec2i(x, y);
                    yield return pos;
                }
            }
        }

        static void PrintMap()
        {
            int n = 0;
            foreach (var v in EnumerateTiles())
            {
                if (n % width == 0) Console.WriteLine();
                Console.Write(GetTile(v));
                n++;
            }
        }

        static void ReadMap(char[] buffer)
        {
            string s = new string(buffer);
            var lines = s.Split("\n", StringSplitOptions.RemoveEmptyEntries);
            width = lines[0].Length;
            height = lines.Length;

            map = new char[height, width];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    map[y, x] = lines[y][x];
                }
            }
        }

        static string NavigateMaze()
        {
            var start = EnumerateTiles().Where(t => GetTile(t) == '^').Single();
            var scaffoldNeighbor = EnumerateNeighbors(start).Where(t => GetTile(t) == '#').Single();

            var directions = new[] { Up, Right, Down, Left };

            var direction = Up;
            //scaffoldNeighbor - start;
            int directionIndex = 0;// Array.IndexOf(directions, direction);

            var pos = start;
            var next = scaffoldNeighbor;

            var walkable = new[] { '#', 'O' };

            int count = 0;
            var nextValue = '#';

            StringBuilder sb = new StringBuilder();

            while (true)
            {
                //find turn direction, rinse, repeat
                var leftIndex = (directionIndex + 3) % 4;
                var rightIndex = (directionIndex + 1) % 4;
                var leftTurn = directions[leftIndex];
                var rightTurn = directions[rightIndex];

                if (walkable.Contains(GetTile(pos + leftTurn)))
                {
                    directionIndex = leftIndex;
                    direction = leftTurn;
                    sb.Append("L");
                }
                else if (walkable.Contains(GetTile(pos + rightTurn)))
                {
                    directionIndex = rightIndex;
                    direction = rightTurn;
                    sb.Append("R");
                }
                else
                {
                    //if no valid turn, complete!
                    break;
                }

                count = 0;
                next = pos + direction;
                nextValue = GetTile(next);

                while (walkable.Contains(nextValue))
                {
                    count++;
                    if (nextValue != 'O') SetTile(next, 'X');
                    pos = next;
                    next = pos + direction;
                    nextValue = GetTile(next);
                }
                sb.Append("" + count + "");
            }

            return sb.ToString();
        }

        static void Main(string[] args)
        {
            var program = File.ReadAllText("./input.txt");
            IntComputer computer = new IntComputer(program);
            char[] buffer = new char[8192];
            int i = 0;
            computer.Output = (l) =>
            {
                buffer[i++] = (char)l;
            };

            computer.Run();
            ReadMap(buffer);

            var intersections = EnumerateTiles()
                .Where(t => GetTile(t) == '#')
                .Where(t =>
                    EnumerateNeighbors(t)
                    .Where(n => GetTile(n) == '#')
                    .Count() > 2)
                .ToArray();

            foreach (var intersection in intersections)
            {
                SetTile(intersection, 'O');
            }

            Console.WriteLine($"Sum of products is {intersections.Select(v => v.X * v.Y).Sum()}");

            var moves = NavigateMaze();
            var components = TrySplitMoveString(moves);

            computer = new IntComputer(program);
            computer.SetMemory(0, 2);

            var symbols = new[] { "A", "B", "C", "L", "R" };
            var inputstring = string.Join("\n",
                components
                .Take(1)
                .Select(str => string.Join(",", str.Cast<char>()))
                .Concat(
                    components
                    .Skip(1)
                    .Select(str =>
                    {
                        var tmp = str;
                        foreach (var symbol in symbols) tmp = tmp.Replace(symbol, $",{symbol},");
                        return tmp.Trim(',');
                    })
                )
                .Concat(new[] { "n\n" })
            );

            int position = 0;
            computer.Input = () =>
            {
                if (position < inputstring.Length)
                {
                    var c = inputstring[position++];
                    Console.Write(c);
                    return c;
                }
                else return Console.ReadKey().KeyChar;
            };
            long lastOutput = 0;
            computer.Output = (l) =>
            {
                Console.Write((char)l);
                lastOutput = l;
            };
            computer.Run();
            Console.WriteLine(lastOutput);
        }

        static string[] TrySplitMoveString(string moves)
        {
            for (int i = 1; i < moves.Length && i < 20; i++)
            {
                var a = moves.Substring(0, i);
                var a_replaced = moves.Replace(a, "A");

                int startB = 0;
                while (a_replaced[startB] == 'A') startB++;

                for (int j = 1; j < a_replaced.Length && j < 20; j++)
                {
                    var b = a_replaced.Substring(startB, j);
                    var ab_replaced = a_replaced.Replace(b, "B");

                    int startC = 0;
                    while (ab_replaced[startC] == 'A' || ab_replaced[startC] == 'B') startC++;

                    for (int k = 1; k < ab_replaced.Length && k < 20; k++)
                    {
                        var c = ab_replaced.Substring(startC, k);
                        var abc_replaced = ab_replaced.Replace(c, "C");

                        if (!abc_replaced.Any(c => c != 'A' && c != 'B' && c != 'C'))
                        {
                            return new[] { abc_replaced, a, b, c };
                        }
                    }
                }
            }
            return null;
        }

    }
}
