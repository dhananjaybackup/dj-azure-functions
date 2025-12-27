using Azure.Messaging.EventGrid;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker;
using System.Text.Json;

namespace DJFunctions.Events
{
    public class BlobUploadedEvent
    {
        private readonly ILogger<BlobUploadedEvent> _logger;

        public BlobUploadedEvent(ILogger<BlobUploadedEvent> logger)
        {
            _logger = logger;
        }

        [Function("BlobUploadedEvent")]
        [QueueOutput("user-queue")]
        public string Run(
    [EventGridTrigger] EventGridEvent eventGridEvent,
    FunctionContext context)
        {
            var logger = context.GetLogger("BlobUploadedEvent");

            logger.LogInformation("Event type: {Type}", eventGridEvent.EventType);

            // Event Grid gives raw JSON
            var json = eventGridEvent.Data.ToString();

            using var doc = JsonDocument.Parse(json);

            var url = doc.RootElement
                         .GetProperty("url")
                         .GetString();

            logger.LogInformation("Blob URL: {Url}", url);

            return JsonSerializer.Serialize(new
            {
                BlobUrl = url,
                FileName = Path.GetFileName(url)
            });
        }
    }
}