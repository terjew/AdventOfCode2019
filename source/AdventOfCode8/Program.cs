using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode8
{
    class Program
    {
        static void Main(string[] args)
        {
            int width = 25;
            int height = 6;
            int layerSize = width * height;

            var text = File.ReadAllText("./input.txt");
            var pixels = text.Trim('\n').Select(c => int.Parse("" + c)).ToList();
            var numLayers = pixels.Count / layerSize;

            var layers = new List<int[]>();
            for(int i = 0; i < numLayers; i++)
            {
                layers.Add(pixels.Skip(i * layerSize).Take(layerSize).ToArray());
            }

            var fewestZeros = layers.OrderBy(l => l.Count(i => i == 0)).First();
            var checksum = fewestZeros.Count(i => i == 1) * fewestZeros.Count(i => i == 2);
            Console.WriteLine(checksum);


            var sum = layers.First();
            foreach (var layer in layers.Skip(1))
            {
                for (int i = 0; i < layerSize; i++)
                {
                    if (sum[i] == 2) sum[i] = layer[i];
                }
            }

            for (int y = 0; y < height; y++)
            {
                Console.WriteLine(string.Join("", sum.Skip(y * width).Take(width).Select(i => i == 1 ? '*' : ' ')));
            }
        }
    }
}
