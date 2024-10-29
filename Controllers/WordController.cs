using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WordCloud;
using WordCloud.Primitives;
using WordCloud.Sizers;
using WordCloud.Layouts;
using WordCloud.Coloring;
using SkiaSharp;
using Microsoft.AspNetCore.SignalR;
using WordCloud.Hubs;
using WordCloud.Drawing;
using System.Text.Json;

namespace WordCloud.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WordController : ControllerBase
    {
        private readonly IHubContext<WordCloudHub> _hubContext;
        private readonly ILogger<WordController> _logger;
        private readonly string _filePath;

        public WordController(IHubContext<WordCloudHub> hubContext, ILogger<WordController> logger, IConfiguration configuration)
        {
            _hubContext = hubContext;
            _logger = logger;

            // Fetch file path from configuration
            _filePath = configuration.GetSection("WordCloud:JsonFilePath").Value;

            EnsureDirectoryExists();
        }

        private void EnsureDirectoryExists()
        {
            var directoryPath = Path.GetDirectoryName(_filePath);

            if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        private WordCloudModel LoadWordEntries()
        {
            try
            {
                if (!System.IO.File.Exists(_filePath))
                {
                    _logger.LogInformation("JSON file not found, initializing a new WordCloudModel instance.");
                    var defaultWordCloud = new WordCloudModel { Words = new List<WordEntry>() };
                    SaveWordEntries(defaultWordCloud);
                    return defaultWordCloud;
                }

                var json = System.IO.File.ReadAllText(_filePath);

                if (string.IsNullOrWhiteSpace(json))
                {
                    _logger.LogWarning("JSON file is empty. Initializing with default content.");
                    var defaultWordCloud = new WordCloudModel { Words = new List<WordEntry>() };
                    SaveWordEntries(defaultWordCloud);
                    return defaultWordCloud;
                }

                _logger.LogInformation("Successfully read JSON data.");
                return JsonSerializer.Deserialize<WordCloudModel>(json) ?? new WordCloudModel();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "An error occurred during JSON deserialization.");
                throw;
            }
        }

        private void SaveWordEntries(WordCloudModel wordCloud)
        {
            var json = JsonSerializer.Serialize(wordCloud);
            System.IO.File.WriteAllText(_filePath, json);
            _logger.LogInformation("Successfully saved JSON data.");
        }

        [HttpGet("generate-wordcloud")]
        [ResponseCache(Duration = 60)]
        public IActionResult GenerateWordCloudImage()
        {
            _logger.LogInformation("GenerateWordCloudImage endpoint called.");

            try
            {
                
                var wordCloud = LoadWordEntries();


                var wordEntries = wordCloud.Words
                    .Select(p => new WordCloud.Primitives.WordEntry { Word = p.Word, Frequency = p.Frequency })
                    .ToList();

                var generator = new WordCloud.WordCloudGenerator(1600, 1000);
        
                var bitmap = generator.Generate(wordEntries, 60, 120);

                using var image = SKImage.FromBitmap(bitmap);
                using var data = image.Encode(SKEncodedImageFormat.Png, 100);
                using var stream = new MemoryStream();
                data.SaveTo(stream);

                _logger.LogInformation("Word cloud image generated successfully.");
                return File(stream.ToArray(), "image/png", "wordcloud.png");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON error: {Message}", ex.Message);
                return BadRequest($"JSON error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal server error: {Message}", ex.Message);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPost("add-word")]
        public IActionResult AddWord([FromBody] string word)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                return BadRequest("Word cannot be empty");
            }

            try
            {
                var wordCloud = LoadWordEntries();
                word = word.ToLower().Trim();
                var wordEntry = wordCloud.Words.FirstOrDefault(w => w.Word == word);

                if (wordEntry != null)
                {
                    wordEntry.Frequency++;
                }
                else
                {
                    wordCloud.Words.Add(new WordEntry { Word = word, Frequency = 1 });
                }

                SaveWordEntries(wordCloud);
                _hubContext.Clients.All.SendAsync("ReceiveWordCloudUpdate");

                return Ok(new { message = "Word added successfully!", word, count = wordEntry?.Frequency ?? 1 });
            }
            catch (JsonException ex)
            {
                return BadRequest($"JSON error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("reset-wordcloud")]
        public IActionResult ResetWordCloud()
        {
            try
            {
                var wordCloud = new WordCloudModel { Words = new List<WordEntry>() };
                SaveWordEntries(wordCloud);
                _hubContext.Clients.All.SendAsync("ReceiveWordCloudUpdate");

                return Ok(new { message = "Word cloud has been reset." });
            }
            catch (JsonException ex)
            {
                return BadRequest($"JSON error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private string GenerateSvg(IEnumerable<LayoutItem> items, int width, int height)
        {
            var svg = $"<svg viewBox='0 0 {width} {height}' xmlns='http://www.w3.org/2000/svg'>\n";

            foreach (var item in items)
            {
                var x = item.Location.X - item.Measured.Left;
                var y = item.Location.Y - item.Measured.Top;
        
                var color = $"#{item.Entry.Frequency:X2}{item.Entry.Frequency:X2}{item.Entry.Frequency:X2}";

                svg += $"<text x='{x:0.##}' y='{y:0.##}' font-size='{item.FontSize:0.##}' fill='{color}'>{item.Entry.Word}</text>\n";
            }
            svg += "</svg>";
            return svg;
        }
    }

    public class WordCloudModel
    {
        public List<WordEntry> Words { get; set; }
    }

    public class WordEntry
    {
        public string Word { get; set; }
        public int Frequency { get; set; }
    }
}