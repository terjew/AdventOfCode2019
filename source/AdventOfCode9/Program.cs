using Common;
using System;
using System.Diagnostics;
using System.IO;

namespace AdventOfCode9
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch sw = Stopwatch.StartNew();
            var intComputer = new IntComputer(File.ReadAllText("./input.txt"));
            intComputer.Input = () => 2;
            intComputer.Run();
            var elapsed = sw.Elapsed;
            Console.WriteLine($"Terminated in {elapsed.TotalMilliseconds} ms");
        }
    }
}
