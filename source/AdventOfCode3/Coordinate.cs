using System;

namespace AdventOfCode3
{
    public class Coordinate
    {
        public int X { get; }
        public int Y { get; }
        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Coordinate Move(MoveCommand command)
        {
            int x = command.DirX * command.Size;
            int y = command.DirY * command.Size;
            return new Coordinate(X + x, Y + y);
        }

        public bool WillCross(MoveCommand command, Coordinate target)
        {
            if (command.DirX != 0){
                if (Y != target.Y) return false;
                if (Math.Abs(X - target.X) > command.Size) return false;
                return Math.Sign(command.DirX) == Math.Sign(target.X - X);
            }
            else
            {
                if (X != target.X) return false;
                if (Math.Abs(Y - target.Y) > command.Size) return false;
                return Math.Sign(command.DirY) == Math.Sign(target.Y - Y);
            }
        }

        public Coordinate Draw(MoveCommand command, int color, Canvas canvas)
        {
            for (int n = 0; n < command.Size; n++)
            {
                int x = X + command.DirX * n;
                int y = Y + command.DirY * n;
                canvas.Paint(x, y, color);

            }
            return Move(command);
        }
    }
}