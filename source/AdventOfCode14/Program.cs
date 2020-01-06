using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AdventOfCode14
{
    class Chemical
    {
        public static Dictionary<string, Chemical> Chemicals = new Dictionary<string, Chemical>();

        public Reaction Synthesis { get; private set; }

        public static Chemical Get(string name)
        {
            if (!Chemicals.ContainsKey(name))
            {
                Chemicals.Add(name, new Chemical(name));
            }
            return Chemicals[name];
        }

        public string Name { get; }
        private Chemical(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return $"({Name})";
        }

        public void SetSynthesis(Reaction reaction)
        {
            if (Synthesis != null) throw new InvalidOperationException("Synthesis already defined");
            Synthesis = reaction;
        }
    }

    class ReactionComponent
    {
        public Chemical Chemical { get; }
        public int Count { get; }

        public ReactionComponent(string definition)
        {
            var pair = definition.Trim().Split(" ");
            Count = int.Parse(pair[0]);
            Chemical = Chemical.Get(pair[1]);
        }

        public override string ToString()
        {
            return $"{Count} {Chemical}";
        }
    }

    class Reaction
    {
        public List<ReactionComponent> Consumes { get; } = new List<ReactionComponent>();
        public ReactionComponent Produces { get; }

        public Reaction(string definition)
        {
            var parts = definition.Split(" => ");
            var inputs = parts[0].Split(", ");
            var output = parts[1];

            foreach (var input in inputs)
            {
                Consumes.Add(new ReactionComponent(input));
            }

            Produces = new ReactionComponent(output);
            Produces.Chemical.SetSynthesis(this);
        }
    }

    class Program
    {
        public static void TestCostOfOneFuel(string input, int expectedCost)
        {
            ReadReactions(input);
            var actualCost = CountOreForOneFuel();
            if (actualCost != expectedCost) throw new InvalidOperationException("Test failed");
        }

        public static void TestMaximumFuel(string input, long expectedMax)
        {
            ReadReactions(input);
            var actualMaxFuel = CalculateMaximumFuel();
            if (actualMaxFuel != expectedMax) throw new InvalidOperationException("Test failed");

        }

        public static void RunTests()
        {
            var input1 = @"10 ORE => 10 A
                            1 ORE => 1 B
                            7 A, 1 B => 1 C
                            7 A, 1 C => 1 D
                            7 A, 1 D => 1 E
                            7 A, 1 E => 1 FUEL";
            TestCostOfOneFuel(input1, 31);

            var input2 = @"9 ORE => 2 A
                            8 ORE => 3 B
                            7 ORE => 5 C
                            3 A, 4 B => 1 AB
                            5 B, 7 C => 1 BC
                            4 C, 1 A => 1 CA
                            2 AB, 3 BC, 4 CA => 1 FUEL";
            TestCostOfOneFuel(input2, 165);

            var input3 = @"157 ORE => 5 NZVS
                            165 ORE => 6 DCFZ
                            44 XJWVT, 5 KHKGT, 1 QDVJ, 29 NZVS, 9 GPVTF, 48 HKGWZ => 1 FUEL
                            12 HKGWZ, 1 GPVTF, 8 PSHF => 9 QDVJ
                            179 ORE => 7 PSHF
                            177 ORE => 5 HKGWZ
                            7 DCFZ, 7 PSHF => 2 XJWVT
                            165 ORE => 2 GPVTF
                            3 DCFZ, 7 NZVS, 5 HKGWZ, 10 PSHF => 8 KHKGT";
            TestCostOfOneFuel(input3, 13312);
            //TestMaximumFuel(input3, 82892753);

            var input4 = @"2 VPVL, 7 FWMGM, 2 CXFTF, 11 MNCFX => 1 STKFG
                            17 NVRVD, 3 JNWZP => 8 VPVL
                            53 STKFG, 6 MNCFX, 46 VJHF, 81 HVMC, 68 CXFTF, 25 GNMV => 1 FUEL
                            22 VJHF, 37 MNCFX => 5 FWMGM
                            139 ORE => 4 NVRVD
                            144 ORE => 7 JNWZP
                            5 MNCFX, 7 RFSQX, 2 FWMGM, 2 VPVL, 19 CXFTF => 3 HVMC
                            5 VJHF, 7 MNCFX, 9 VPVL, 37 CXFTF => 6 GNMV
                            145 ORE => 6 MNCFX
                            1 NVRVD => 8 CXFTF
                            1 VJHF, 6 MNCFX => 4 RFSQX
                            176 ORE => 6 VJHF";
            TestCostOfOneFuel(input4, 180697);
            //TestMaximumFuel(input4, 5586022);

            var input5 = @"171 ORE => 8 CNZTR
                            7 ZLQW, 3 BMBT, 9 XCVML, 26 XMNCP, 1 WPTQ, 2 MZWV, 1 RJRHP => 4 PLWSL
                            114 ORE => 4 BHXH
                            14 VRPVC => 6 BMBT
                            6 BHXH, 18 KTJDG, 12 WPTQ, 7 PLWSL, 31 FHTLT, 37 ZDVW => 1 FUEL
                            6 WPTQ, 2 BMBT, 8 ZLQW, 18 KTJDG, 1 XMNCP, 6 MZWV, 1 RJRHP => 6 FHTLT
                            15 XDBXC, 2 LTCX, 1 VRPVC => 6 ZLQW
                            13 WPTQ, 10 LTCX, 3 RJRHP, 14 XMNCP, 2 MZWV, 1 ZLQW => 1 ZDVW
                            5 BMBT => 4 WPTQ
                            189 ORE => 9 KTJDG
                            1 MZWV, 17 XDBXC, 3 XCVML => 2 XMNCP
                            12 VRPVC, 27 CNZTR => 2 XDBXC
                            15 KTJDG, 12 BHXH => 5 XCVML
                            3 BHXH, 2 VRPVC => 7 MZWV
                            121 ORE => 7 VRPVC
                            7 XCVML => 6 RJRHP
                            5 BHXH, 4 VRPVC => 5 LTCX";
            TestCostOfOneFuel(input5, 2210736);
            //TestMaximumFuel(input5, 460664);
        } 

        static void Main(string[] args)
        {
            RunTests();

            ReadReactions(File.ReadAllText("./input.txt"));
            var amount = CountOreForOneFuel();
            Console.WriteLine($"In order to produce 1 unit of fuel, {amount} ore is required");
            var max = CalculateMaximumFuel();
            Console.WriteLine($"Maximum possible fuel is {max}");
        }

        public static void ReadReactions(string input)
        {
            Chemical.Chemicals.Clear();
            var reactions = input.Split("\n", StringSplitOptions.RemoveEmptyEntries).Select(l => new Reaction(l)).ToList();
        }

        public static long CountOreForOneFuel()
        {
            var fuel = Chemical.Get("FUEL");
            var store = Chemical.Chemicals.Select(kvp => kvp.Value).ToDictionary(c => c, c => 0L);

            var amount = CountRequiredOre(fuel, 1, store);
            return amount;
        }

        public static long CalculateMaximumFuel()
        {
            long ore = 1000000000000;
            var fuel = Chemical.Get("FUEL");
            var store = Chemical.Chemicals.Select(kvp => kvp.Value).ToDictionary(c => c, c => 0L);

            Stopwatch sw = Stopwatch.StartNew();
            long count = 0;
            while (ore > 0)
            {
                int tryProduce = 1;
                //if (ore > 5000000000000) tryProduce = 1000;
                //if (ore > 2000000000000) tryProduce = 100;
                //if (ore > 1000000000000) tryProduce = 10;
                var required = CountRequiredOre(fuel, tryProduce, store);
                if (ore < required) break;
                ore -= required;
                count++;

                if (sw.Elapsed > TimeSpan.FromSeconds(1))
                {
                    long spent = 1000000000000 - ore;
                    double percent = (double)spent / 1000000000000;
                    Console.WriteLine($"{percent * 100}%");
                    sw.Restart();
                }

            }
            return count;
        }

        static long CountRequiredOre(Chemical chemical, long required, Dictionary<Chemical, long> store)
        {
            if (chemical.Name == "ORE")
            {
                return required;
            }

            var synthesis = chemical.Synthesis;
            var output = synthesis.Produces;
            var stored = store[chemical];
            if (stored > 0)
            {

                if (stored > required)
                {
                    store[chemical] = stored - required;
                    return 0;
                }
                else
                {
                    store[chemical] = 0;
                    required -= stored;
                }
            }
            var reactionCount = (int)Math.Ceiling(required / (float)output.Count);
            var produced = reactionCount * output.Count;
            if (produced > required) store[chemical] += (produced - required);

            long ore = 0;
            foreach (var inputComponent in synthesis.Consumes)
            {
                ore += CountRequiredOre(inputComponent.Chemical, inputComponent.Count * reactionCount, store);
            }

            return ore;
        }
    }
}
