using System.Collections.Generic;
using System.Threading.Tasks;
using WordCloud.Models;

namespace WordCloud.Services.Interfaces
{
    public interface IWordCloudService
    {
        Task<IEnumerable<WordEntry>> GetWordCloudDataAsync();
    }
}