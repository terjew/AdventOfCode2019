using System;

namespace AdventOfCode3
{
    public class MoveCommand
    {
        public int DirX { get; }
        public int DirY { get; }

        public int Size { get; }

        public MoveCommand(string str)
        {
            var d = str[0];
            switch (d)
            {
                case 'U':
                    DirY = 1;
                    break;
                case 'D':
                    DirY = -1;
                    break;
                case 'L':
                    DirX = -1;
                    break;
                case 'R':
                    DirX = 1;
                    break;
                default:
                    throw new ArgumentException();
            }
            Size = int.Parse(str.Substring(1));
        }
    }
}