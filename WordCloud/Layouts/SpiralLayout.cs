using System;
using SkiaSharp;
using WordCloud.Primitives;

namespace WordCloud.Layouts
{
    public class SpiralLayout
    {
        private readonly double centerX;
        private readonly double centerY;
        private const double StepAlpha = Math.PI / 60;
        private const int MaxPoints = 500;

        public SpiralLayout(double width, double height)
        {
            centerX = width / 2;
            centerY = height / 2;
        }

        public bool TryFindFreePosition(double wordWidth, double wordHeight, out SKPoint location)
        {
            location = new SKPoint();
            double alpha = 0;

            for (int i = 0; i < MaxPoints; i++)
            {
                double dX = (i / (double)MaxPoints) * Math.Sin(alpha) * centerX;
                double dY = (i / (double)MaxPoints) * Math.Cos(alpha) * centerY;

                location = new SKPoint((float)(centerX + dX - wordWidth / 2), (float)(centerY + dY - wordHeight / 2));
                return true; // Replace with collision detection logic in the full implementation
            }

            return false;
        }
    }
}