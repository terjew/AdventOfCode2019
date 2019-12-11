using System;
using System.IO;
using System.Linq;

namespace AdventOfCode1
{
    class Module
    {
        public int Mass { get; private set; }

        public int FuelReq => TotalFuelReq();

        private int TotalFuelReq()
        {
            var fuel = CalculateFuelReq(Mass);
            var totalfuel = fuel;
            while (fuel > 0)
            {
                fuel = CalculateFuelReq(fuel);
                totalfuel += fuel;
            }
            return totalfuel;
        }

        public Module(int mass)
        {
            Mass = mass;
        }

        public static int CalculateFuelReq(int mass)
        {
            return Math.Max(0, (mass / 3) - 2);
        }

    }
    class Program
    {

        static void Main(string[] args)
        {
            var test = new Module(100756);
            var f = test.FuelReq;

            var input = File.ReadAllLines("./input.txt");
            var modules = input.Select(l => new Module(int.Parse(l)));
            var fuelreq = modules.Sum(m => m.FuelReq);
        }
    }
}
