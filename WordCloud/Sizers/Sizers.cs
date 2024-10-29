using System.Collections.Generic;
using WordCloud.Primitives;

namespace WordCloud.Sizers
{
    public interface ISizer
    {
        double GetFontSize(WordEntry entry, int maxFrequency, double minSize, double maxSize);
    }

    public class LogSizer : ISizer
    {
        public double GetFontSize(WordEntry entry, int maxFrequency, double minSize, double maxSize)
        {
            double weight = Math.Log(entry.Frequency) / Math.Log(maxFrequency);
            return minSize + (maxSize - minSize) * weight;
        }
    }
}