using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using WordCloud.Models;
using WordCloud.Services.Interfaces;

namespace WordCloud.Services.Implementations
{
    public class WordCloudService : IWordCloudService
    {
        private readonly string _jsonFilePath;

        public WordCloudService(IConfiguration configuration, IWebHostEnvironment env)
        {
            // Resolving the absolute path for the JSON file
            _jsonFilePath = Path.Combine(env.ContentRootPath, configuration["WordCloud:JsonFilePath"]);
        }

        public async Task<IEnumerable<WordEntry>> GetWordCloudDataAsync()
        {
            if (File.Exists(_jsonFilePath))
            {
                var jsonData = await File.ReadAllTextAsync(_jsonFilePath);
                return JsonConvert.DeserializeObject<IEnumerable<WordEntry>>(jsonData);
            }

            return new List<WordEntry>();
        }
    }
}