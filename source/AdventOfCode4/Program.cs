using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode4
{
    class Program
    {
        static void Main(string[] args)
        {
            int min = 372304;
            int max = 847060;

            var possibilities = Enumerable.Range(min, max - min + 1);
            
            var valid1 = possibilities.Where(i => IsValid(i));
            var count = valid1.Count();

            Console.WriteLine($"{count} codes satisfy the first condition");
            var count2 = valid1.Count(i => IsValid2(i));
            Console.WriteLine($"{count2} codes satisfy the second condition");
        }
        
        static int[] ToDigits(int num)
        {
            string str = "" + num;
            return str.Select(c => int.Parse("" + c)).ToArray();
        }

        static bool IsValid(int code)
        {
            var digits = ToDigits(code);

            int max = int.MinValue;
            bool haspair = false;
            foreach(int digit in digits)
            {
                if (digit < max) return false;
                if (digit == max) haspair = true; 
                max = digit;
            }

            return haspair;
        }

        static bool IsValid2(int code)
        {
            string str = "" + code;
            int[] digits = str.Select(c => int.Parse("" + c)).ToArray();

            var distinct = digits.Distinct();
            return distinct.Any(d => str.Contains($"{d}{d}") && !str.Contains($"{d}{d}{d}"));
        }

    }
}
