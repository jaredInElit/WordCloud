using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace WordCloud.Hubs
{
    public class WordCloudHub : Hub
    {
        private readonly ILogger<WordCloudHub> _logger;

        // Constructor to inject logger
        public WordCloudHub(ILogger<WordCloudHub> logger)
        {
            _logger = logger;
        }

        // Method to notify clients to update the word cloud
        public async Task SendWordCloudUpdate()
        {
            // Logging the method call
            _logger.LogInformation("Broadcasting word cloud update to all clients.");

            try
            {
                // Sending message to all connected clients to trigger an update
                await Clients.All.SendAsync("ReceiveWordCloudUpdate");
            }
            catch (Exception ex)
            {
                // Logging any potential exceptions
                _logger.LogError(ex, "Error occurred while sending word cloud update.");
                throw;
            }
        }
    }
}