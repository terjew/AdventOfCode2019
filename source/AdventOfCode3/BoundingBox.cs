namespace AdventOfCode3
{
    internal class BoundingBox
    {
        public int Left { get; private set; }
        public int Right{ get; private set; }
        public int Top { get; private set; }
        public int Bottom { get; private set; }

        public int Width => Right - Left + 1;
        public int Height => Top - Bottom + 1;

        public BoundingBox(Coordinate c)
        {
            Left = Right = c.X;
            Top = Bottom = c.Y;
        }

        public void ExtendBy(Coordinate c)
        {
            if (c.X > Right) Right = c.X;
            if (c.X < Left) Left = c.X;
            if (c.Y > Top) Top = c.Y;
            if (c.Y < Bottom) Bottom = c.Y;
        }
    }
}