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
            while (true)
            {
                var divisionProgram = "003,1001,003,1002,1101,0,0,1003,1101,0,0,1004,0001,1002,1003,1003,0101,1,1004,1004,0007,1003,1001,1005,1005,1005,12,0008,1003,1001,1006,1005,1006,38,0101,-1,1004,1004,0004,1004,99";
                var divisionComputer = new IntComputer(divisionProgram);
                divisionComputer.Run();
            }

            var intComputer = new IntComputer(File.ReadAllText("./input.txt"));
            intComputer.Input = () => 2;
            intComputer.Run();
            var elapsed = sw.Elapsed;
            Console.WriteLine($"Terminated in {elapsed.TotalMilliseconds} ms");
        }
    }
}
