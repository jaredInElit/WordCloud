using SkiaSharp;
using WordCloud.Primitives;
using WordCloud.Sizers;
using WordCloud.Layouts;
using WordCloud.Coloring;
using System.Collections.Generic;

namespace WordCloud.Drawing
{
    public class WordDrawer
    {
        private readonly SKBitmap bitmap;
        private readonly SKCanvas canvas;
        private readonly SKPaint textPaint;

        public WordDrawer(int width, int height)
        {
            bitmap = new SKBitmap(width, height);
            canvas = new SKCanvas(bitmap);
            textPaint = new SKPaint { IsAntialias = true };
        }

        public void DrawWord(LayoutItem item)
        {
            textPaint.Color = SKColors.Black;
            textPaint.TextSize = (float)item.FontSize;
            canvas.DrawText(item.Entry.Word, item.Location.X, item.Location.Y, textPaint);
        }

        public SKBitmap GetBitmap()
        {
            canvas.Flush();
            return bitmap;
        }
    }
}