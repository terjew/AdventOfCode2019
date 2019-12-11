using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode3
{
    class Program
    {

        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("./input.txt");
            var cable1 = lines[0].Split(",").Select(str => new MoveCommand(str));
            var cable2 = lines[1].Split(",").Select(str => new MoveCommand(str));
            var port = new Coordinate(0, 0);

            var box = new BoundingBox(port);
            foreach(var cable in new[] { cable1, cable2 })
            {
                var current = port;
                foreach(var command in cable)
                {
                    current = current.Move(command);
                    box.ExtendBy(current);
                }
            }

            var width = box.Width;
            var height = box.Height;

            //bool[] canvas = new bool[width * height];
            Canvas canvas = new Canvas(box.Width, box.Height);
            port = new Coordinate(-box.Left, -box.Bottom);

            int color = 1;
            foreach (var cable in new[] { cable1, cable2 })
            {
                var current = port;
                foreach (var command in cable)
                {
                    current = current.Draw(command, color, canvas);
                    box.ExtendBy(current);
                }
                color <<= 1;
            }

            //canvas.SaveImage();
            var intersections = canvas.Array.Select((c, i) => new { index = i, color = c }).Where(p => p.color == 3).ToList();

            var coords = new List<Coordinate>();
            foreach (var intersection in intersections)
            {
                var y = intersection.index / width;
                var x = intersection.index - (y * width);
                coords.Add(new Coordinate(x - port.X, y - port.Y));
            }

            var orderedCoords = coords.OrderByDescending(c => c.X + c.Y);

            var origin = new Coordinate(0, 0);
            int lowest = int.MaxValue;

            foreach(var intersection in orderedCoords)
            {
                var cable1Dist = CountMoves(origin, cable1, intersection);
                var cable2Dist = CountMoves(origin, cable2, intersection);
                if (cable2Dist < 0 || cable2Dist < 0) continue;
                int sum =cable1Dist + cable2Dist;
                if (sum < lowest)
                {
                    lowest = sum;
                }
            }
        }

        private static int CountMoves(Coordinate start, IEnumerable<MoveCommand> cable, Coordinate intersection)
        {
            var pos = start;
            var moves = 0;
            foreach(var move in cable)
            {
                if (pos.WillCross(move, intersection))
                {
                    moves += Math.Abs(intersection.X - pos.X) + Math.Abs(intersection.Y - pos.Y);
                    return moves;
                }
                else
                {
                    moves += move.Size;
                    pos = pos.Move(move);
                }
            }
            return -1;
        }
    }
}
