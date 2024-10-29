using System.Collections.Generic;
using System.Linq;
using SkiaSharp;
using WordCloud.Primitives;
using WordCloud.Sizers;
using WordCloud.Layouts;
using WordCloud.Coloring;
using WordCloud.Drawing;

namespace WordCloud
{
    public class WordCloudGenerator
    {
        private readonly SpiralLayout layout;
        private readonly WordDrawer drawer;
        private readonly ISizer sizer;
        private readonly RandomColorizer colorizer;

        public WordCloudGenerator(int width, int height)
        {
            layout = new SpiralLayout(width, height);
            drawer = new WordDrawer(width, height);
            sizer = new LogSizer();
            colorizer = new RandomColorizer();
        }

        public SKBitmap Generate(IEnumerable<WordEntry> entries, double minFontSize, double maxFontSize)
        {
            var maxFrequency = entries.Max(e => e.Frequency);

            foreach (var entry in entries)
            {
                var fontSize = sizer.GetFontSize(entry, maxFrequency, minFontSize, maxFontSize);
                double wordWidth = fontSize * entry.Word.Length * 0.6;
                double wordHeight = fontSize;

                if (layout.TryFindFreePosition(wordWidth, wordHeight, out SKPoint location))
                {
                    var item = new LayoutItem
                    {
                        Entry = entry,
                        FontSize = fontSize,
                        Location = location
                    };

                    drawer.DrawWord(item);
                }
            }

            return drawer.GetBitmap();
        }
    }
}