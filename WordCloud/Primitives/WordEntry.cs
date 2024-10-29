using SkiaSharp;

namespace WordCloud.Primitives
{
    public class WordEntry
    {
        public string Word { get; set; }
        public int Frequency { get; set; }
    }

    public class LayoutItem
    {
        public WordEntry Entry { get; set; }
        public double FontSize { get; set; }
        public SKRect Measured { get; set; }
        public SKPoint Location { get; set; }
    }
}