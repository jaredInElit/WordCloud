using Microsoft.AspNetCore.Mvc;
using KnowledgePicker.WordCloud;
using KnowledgePicker.WordCloud.Primitives;
using KnowledgePicker.WordCloud.Sizers;
using KnowledgePicker.WordCloud.Layouts;
using KnowledgePicker.WordCloud.Coloring;
using SkiaSharp;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KnowledgePicker.WordCloud.Drawing;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WordController : ControllerBase
    {
        private static Dictionary<string, int> wordFrequencies = new Dictionary<string, int>
        {
            { "Kochava", 10 },
            { "CSM", 5 }
        };

        [HttpGet("generate-wordcloud")]
        public IActionResult GenerateWordCloudImage()
        {
            var wordEntries = wordFrequencies.Select(p => new WordCloudEntry(p.Key, p.Value));
            var wordCloud = new WordCloudInput(wordEntries)
            {
                Width = 1600,
                Height = 800,
                MinFontSize = 30,
                MaxFontSize = 80
            };

            var sizer = new LogSizer(wordCloud);
            using var engine = new SkGraphicEngine(sizer, wordCloud);
            var layout = new SpiralLayout(wordCloud);
            var colorizer = new RandomColorizer();
            var wcg = new WordCloudGenerator<SKBitmap>(wordCloud, engine, layout, colorizer);

            using var final = new SKBitmap(wordCloud.Width, wordCloud.Height);
            using var canvas = new SKCanvas(final);
            canvas.Clear(SKColors.White);
            using var bitmap = wcg.Draw();
            canvas.DrawBitmap(bitmap, 0, 0);

            using var data = final.Encode(SKEncodedImageFormat.Png, 100);
            using var stream = new MemoryStream();
            data.SaveTo(stream);

            return File(stream.ToArray(), "image/png", "wordcloud.png");
        }

        [HttpGet("generate-wordcloud-svg")]
        public IActionResult GenerateWordCloudSvg()
        {
            var wordEntries = wordFrequencies.Select(p => new WordCloudEntry(p.Key, p.Value));
            var wordCloud = new WordCloudInput(wordEntries)
            {
                Width = 1024,
                Height = 512,
                MinFontSize = 10,
                MaxFontSize = 50
            };

            var sizer = new LogSizer(wordCloud);
            using var engine = new SkGraphicEngine(sizer, wordCloud);
            var layout = new SpiralLayout(wordCloud);
            var colorizer = new RandomColorizer();
            var wcg = new WordCloudGenerator<SKBitmap>(wordCloud, engine, layout, colorizer);

            IEnumerable<(LayoutItem Item, double FontSize)> items = wcg.Arrange();

            var svg = GenerateSvg(items, wordCloud, wcg);

            return Content(svg, "image/svg+xml");
        }

        [HttpPost("add-word")]
        public IActionResult AddWord([FromBody] string word)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                return BadRequest("Word cannot be empty");
            }

            word = word.ToLower().Trim();
            if (wordFrequencies.ContainsKey(word))
            {
                wordFrequencies[word]++;
            }
            else
            {
                wordFrequencies[word] = 1;
            }

            return Ok(new { message = "Word added successfully!", word, count = wordFrequencies[word] });
        }
        [HttpPost("reset-wordcloud")]
        public IActionResult ResetWordCloud()
        {
            wordFrequencies.Clear();
            return Ok(new { message = "Word cloud has been reset." });
        }


        private string GenerateSvg(IEnumerable<(LayoutItem Item, double FontSize)> items, WordCloudInput wordCloud, WordCloudGenerator<SKBitmap> wcg)
        {
            var svg = $"<svg viewBox='0 0 {wordCloud.Width} {wordCloud.Height}' xmlns='http://www.w3.org/2000/svg'>\n";

            foreach (var (item, fontSize) in items)
            {
                var x = item.Location.X - item.Measured.Left;
                var y = item.Location.Y - item.Measured.Top;
                var color = wcg.GetColorHexString(item);

                svg += $"<text x='{x:0.##}' y='{y:0.##}' font-size='{fontSize:0.##}' fill='{color}'>{item.Entry.Word}</text>\n";
            }

            svg += "</svg>";
            return svg;
        }
    }
}
