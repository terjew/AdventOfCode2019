using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AdventOfCode3
{
    public class Canvas
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        private int[] arr;

        public IEnumerable<int> Array => arr;

        public Canvas(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            arr = new int[Width * Height];
        }

        public void Paint(int x, int y, int color)
        {
            var index = y * Width + x;
            if (arr[index] > 0 && arr[index] != color)
            {

            }
            arr[index] += color;
        }

        public void SaveImage()
        {
            Bitmap bmp = new Bitmap(Width, Height);
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var color = arr[y * Width + x];
                    int r = (color & 2) > 0 ? 255 : 0;
                    int g = (color & 1) > 0 ? 255 : 0;
                    int b = 0;
                    bmp.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }
            bmp.Save("c:/temp/cables.png", ImageFormat.Png);
        }
    }
}