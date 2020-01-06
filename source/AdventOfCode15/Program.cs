using AdventOfCode11;
using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace AdventOfCode15
{
    enum Command
    {
        Abort = 0,
        North = 1,
        South,
        West,
        East
    }

    enum Status
    {
        Wall = 0,
        Moved,
        TargetReached
    }

    class Mapper
    {
        private static Dictionary<Command, Vec2i> moves = new Dictionary<Command, Vec2i>()
        {
            {Command.North, new Vec2i(0, -1)},
            {Command.South, new Vec2i(0, 1)},
            {Command.West, new Vec2i(-1, 0)},
            {Command.East, new Vec2i(1, 0)},
        };

        private const char EMPTY = ' ';
        private const char WALL = '▒';
        private const char START = 'S';
        private const char TARGET = 'T';
        private const char OXYGEN = 'O';
        private const char UNKNOWN = '?';
        private const int DIMS = 41;
        private Vec2i pos;

        public char[,] Map { get; private set; } = new char[DIMS, DIMS];
        public Vec2i Start { get; private set; } = new Vec2i(DIMS / 2 + 1, DIMS / 2 + 1);
        public Vec2i Target { get; private set; }
        public int MovesToTarget { get; private set; }

        Command nextMove;
        Stack<Vec2i> track = new Stack<Vec2i>();

        public Mapper()
        {
            pos = new Vec2i(Start.X, Start.Y);
            for (int x = 0; x < DIMS; x++)
            {
                for (int y = 0; y < DIMS; y++)
                {
                    Map[x, y] = UNKNOWN;
                }
            }
            SetTile(pos, START);
            Print();
            nextMove = GetManualInput();
            track.Push(pos);
        }

        internal long SendNextMove()
        {
            return (long)nextMove;
        }

        internal void StatusReceived(long l)
        {
            var targetPos = pos + moves[nextMove];
            var status = (Status)l;

            switch (status)
            {
                case Status.Wall:
                    SetTile(targetPos, WALL);
                    break;
                case Status.Moved:
                    if (GetTile(targetPos) == UNKNOWN)
                    {
                        SetTile(targetPos, EMPTY);
                    }
                    pos = targetPos;
                    track.Push(pos);
                    break;
                case Status.TargetReached:
                    SetTile(targetPos, TARGET);
                    pos = targetPos;
                    Target = targetPos;
                    track.Push(pos);
                    MovesToTarget = track.Count - 1;
                    Target = pos;
                    break;
            }
            MoveCursor(targetPos);
            Console.Write(GetTile(targetPos));
            MoveCursor(pos);
            nextMove = DetermineNextMove();
        }

        private Command GetManualInput()
        {
            while (true)
            {
                var input = Console.ReadKey();
                Console.CursorLeft = pos.X;
                Console.CursorTop = pos.Y;
                Console.Write(Map[pos.X, pos.Y]);
                Console.CursorLeft = pos.X;
                Console.CursorTop = pos.Y;

                switch (input.Key)
                {
                    case ConsoleKey.UpArrow:
                        return Command.North;
                    case ConsoleKey.DownArrow:
                        return Command.South;
                    case ConsoleKey.LeftArrow:
                        return Command.West;
                    case ConsoleKey.RightArrow:
                        return Command.East;
                }
            }
        }

        private void SetTile(Vec2i pos, char value)
        {
            Map[pos.X, pos.Y] = value;
            MoveCursor(pos);
            Console.Write(value);
        }

        private char GetTile(Vec2i pos)
        {
            return Map[pos.X, pos.Y];
        }

        private void MoveCursor(Vec2i pos)
        {
            Console.CursorLeft = pos.X;
            Console.CursorTop = pos.Y;
        }

        private IEnumerable<Vec2i> EnumerateNeighbors(Vec2i pos)
        {
            yield return pos + moves[Command.North];
            yield return pos + moves[Command.South];
            yield return pos + moves[Command.West];
            yield return pos + moves[Command.East];
        }

        private void Print()
        {
            for (int y = 0; y < DIMS; y++)
            {
                for (int x = 0; x < DIMS; x++)
                {
                    Console.CursorLeft = x;
                    Console.CursorTop = y;
                    Console.Write(Map[x, y]);
                }
            }
        }

        private Command DetermineNextMove()
        {
            Thread.Sleep(3);
            //if there is an unvisited neighbor, visit it:
            if (GetTile(pos + moves[Command.North]) == UNKNOWN) return Command.North;
            if (GetTile(pos + moves[Command.South]) == UNKNOWN) return Command.South;
            if (GetTile(pos + moves[Command.East]) == UNKNOWN) return Command.East;
            if (GetTile(pos + moves[Command.West]) == UNKNOWN) return Command.West;

            //if all neighbors are walls or visited, backtrack
            try
            {
                if (track.Peek() == pos) track.Pop(); //remove current
                var prev = track.Pop();
                return moves.Where(kvp => kvp.Value == (prev - pos)).Single().Key;
            }
            catch
            {
                //back at start
                return Command.Abort;
            }
        }

        public int FillOxygen()
        {
            var closed = new[] { OXYGEN, WALL, UNKNOWN };
            List<Vec2i> openTiles = new List<Vec2i>();
            openTiles.Add(Target);
            SetTile(Target, OXYGEN);
            int t = 0;
            while (openTiles.Any())
            {
                foreach (var tile in openTiles.ToList())
                {
                    SetTile(tile, OXYGEN);
                    openTiles.Remove(tile);
                    var neighbors = EnumerateNeighbors(tile).Where(n => !closed.Contains(GetTile(n)));
                    foreach (var n in neighbors)
                    {
                        if (!openTiles.Contains(n)) openTiles.Add(n);
                    }
                }
                t++;
                Thread.Sleep(3);
            }
            return t - 1;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var mapper = new Mapper();
            IntComputer computer = new IntComputer(File.ReadAllText("./input.txt"));
            computer.Input = () => mapper.SendNextMove();
            computer.Output = (l) => mapper.StatusReceived(l);
            computer.Run();
            Console.CursorLeft = 0;
            Console.CursorTop = 42;
            Console.WriteLine($"Min moves to target was: {mapper.MovesToTarget}");
            Console.ReadKey();
            int t = mapper.FillOxygen();
            Console.CursorLeft = 0;
            Console.CursorTop = 43;
            Console.WriteLine($"Oxygen fills in: {t}");
            Console.ReadKey();

        }
    }
}
