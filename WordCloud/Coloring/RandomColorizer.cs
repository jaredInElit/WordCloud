using SkiaSharp;
using System;

namespace WordCloud.Coloring
{
    public class RandomColorizer
    {
        private readonly Random random = new Random();

        public SKColor GetRandomColor()
        {
            return new SKColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
        }
    }
}