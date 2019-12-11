using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Numerics;

namespace AdventOfCode10
{
    class AsteroidGroup
    {
        public double Angle;
        public Stack<Vector2> Offsets;
    }

    class Base
    {
        public Vector2 location;
        public List<AsteroidGroup> Groups;
    }

    class Program
    {
        static List<Vector2> asteroids;
        static void LoadMap(string[] lines)
        {
            var width = lines[0].Length;
            var height = lines.Length;
            asteroids = new List<Vector2>();
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    if (lines[y][x] == '#')
                    {
                        asteroids.Add(new Vector2() { X = x, Y = y });
                    }
                }
            }
        }

        private static double GetPositiveAngle(Vector2 offset)
        {
            var angle = Math.Atan2(offset.X, -offset.Y);
            return (angle + Math.PI * 2) % (Math.PI * 2);
        }

        private static List<AsteroidGroup> GroupByDirection(Vector2 @base)
        {
            return asteroids
                .Where(a => a != @base)
                .Select(a => a - @base)
                .GroupBy(v => GetPositiveAngle(v))
                .OrderBy(g => g.Key)
                .Select(g => new AsteroidGroup()
                {
                    Angle = g.Key,
                    Offsets = new Stack<Vector2>(g.OrderByDescending(v => v.Length()))
                })
                .ToList();
        }

        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("./input.txt");
            LoadMap(lines);

            var @base = asteroids
                .Select(a => new Base(){
                    location = a,
                    Groups = GroupByDirection(a)
                })
                .OrderByDescending(candidate => candidate.Groups.Count)
                .First();

            Console.WriteLine($"Best position can see {@base.Groups.Count} other asteroids");

            int vaporized = 0;
            int i = 0;
            Vector2 winner;
            while (true)
            {
                var targetGroup = @base.Groups[i];
                if (vaporized == 199)
                {
                    winner = targetGroup.Offsets.Peek();
                    break;
                }
                var target = @base.location + targetGroup.Offsets.Pop();
                //Console.WriteLine($"Vaporizing target at {target}");
                ++vaporized;
                if (!targetGroup.Offsets.Any())
                {
                    @base.Groups.Remove(targetGroup);
                }
                else
                {
                    i++;
                }
                i %= @base.Groups.Count;
            }

            var winnerLocation = @base.location + winner;
            Console.WriteLine($"Number 200 to be vaporized is {winnerLocation}");

        }
    }
}
