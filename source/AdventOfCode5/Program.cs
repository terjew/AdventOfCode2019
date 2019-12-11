using Common;
using System;
using System.IO;
using System.Linq;

namespace AdventOfCode5
{
    public class Program
    {     
        static void Main(string[] args)
        {
            while (true)
            {
                var computer = new IntComputer(File.ReadAllText("./input.txt"));
                computer.Run();
            }
        }
    }
}
