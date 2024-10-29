using System;
using System.Collections.Generic;
using SkiaSharp;
using WordCloud.Primitives;

namespace WordCloud.Layouts
{
    public class SpiralLayout
    {
        private readonly double centerX;
        private readonly double centerY;
        private const double StepAlpha = Math.PI / 60;  // Spiral step size (angle increment)
        private const int MaxPoints = 1000;  // Maximum number of points to check along the spiral

        private readonly List<SKRect> placedRectangles = new List<SKRect>();

        public SpiralLayout(double width, double height)
        {
            centerX = width / 2;
            centerY = height / 2;
        }


        public bool TryFindFreePosition(double wordWidth, double wordHeight, out SKRect foundRectangle)
        {
            foundRectangle = SKRect.Empty;
            double alpha = 0;
            const double SpiralStep = 5;

            for (int i = 0; i < MaxPoints; i++)
            {
                double dX = SpiralStep * i * Math.Sin(alpha);
                double dY = SpiralStep * i * Math.Cos(alpha);

                var proposedRectangle = new SKRect(
                    (float)(centerX + dX - wordWidth / 2),
                    (float)(centerY + dY - wordHeight / 2),
                    (float)(centerX + dX + wordWidth / 2),
                    (float)(centerY + dY + wordHeight / 2));


                if (!IsColliding(proposedRectangle))
                {
                    foundRectangle = proposedRectangle;
                    placedRectangles.Add(proposedRectangle);
                    return true;
                }


                alpha += StepAlpha;
            }

            return false;
        }


        private bool IsColliding(SKRect proposedRectangle)
        {
            foreach (var rect in placedRectangles)
            {
                if (rect.IntersectsWith(proposedRectangle))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
