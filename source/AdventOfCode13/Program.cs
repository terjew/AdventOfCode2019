using Common;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace AdventOfCode13
{
    class Program
    {
        const int WIDTH = 40;
        const int HEIGHT = 25;
        
        static int[,] screen = new int[WIDTH, HEIGHT];
        static string[] pixelTypes =
        {
            "  ",
            "||",
            "##",
            "__",
            "()"
        };

        static int score = 0;
        static int paddlePos = 0;
        static int ballPos = 0;
        static bool headless = false;

        static void Main(string[] args)
        {
            var str = File.ReadAllText("./input.txt");
            var computer = new IntComputer(str);

            Stopwatch sw = Stopwatch.StartNew();
            TimeSpan? period = null;
            if (args.Length > 0)
            {
                int targetfps = int.Parse(args[0]);
                if (targetfps > 0) period = TimeSpan.FromSeconds(1) / targetfps;
            }

            if (args.Length > 1 && args[1] == "headless")
            {
                headless = true;
            }
            computer.Input = () =>
            {
                while (period != null && sw.Elapsed < period) Thread.Sleep(1);
                sw.Restart();
                if (ballPos > paddlePos) return 1;
                if (ballPos < paddlePos) return -1;
                return 0;
            };

            int x = 0;
            int y = 0;
            int state = 0;
            computer.Output = (o) =>
            {
                int value = (int)o;
                switch (state)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        if (x == -1 && y == 0)
                        {
                            SetScore(value);
                        }
                        else
                        {
                            SetPixel(x, y, value);
                        }
                        break;
                }
                state++;
                state %= 3;
            };

            computer.SetMemory(0, 2L);
            DrawScreen();
            computer.Run();
            //Console.WriteLine($"Blocks set: {screen.Cast<int>().Count(p => p == 2)}");
            Console.WriteLine($"Final score: {score}");
        }

        private static void SetScore(int score)
        {
            Program.score = score;
            if (headless) return;

            Console.CursorLeft = 0;
            Console.CursorTop = 0;
            Console.Write($"Score: {score}");
        }

        private static void DrawScreen()
        {
            if (headless) return;
            Console.CursorVisible = false;
            Console.Clear();
            Console.WriteLine($"Score: {score}");
            for(int y = 0; y < HEIGHT; y++)
            {
                Console.WriteLine();
                for (int x = 0; x < WIDTH; x++)
                {
                    Console.Write(pixelTypes[screen[x, y]]);
                }
            }
        }

        public static void SetPixel(int x, int y, int color)
        {
            screen[x, y] = (int)color;
            if (color == 4) ballPos = x;
            if (color == 3) paddlePos = x;
            if (headless) return;
            Console.CursorLeft = x * 2;
            Console.CursorTop = y + 2;
            Console.Write(pixelTypes[color]);
        }
    }
}
