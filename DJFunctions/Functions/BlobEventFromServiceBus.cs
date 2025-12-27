using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.DurableTask.Client;
namespace DJFunctions;

public class BlobEventFromServiceBus
{
    private readonly ILogger<BlobEventFromServiceBus> _logger;

    public BlobEventFromServiceBus(ILogger<BlobEventFromServiceBus> logger)
    {
        _logger = logger;
    }

    /* Temporary comment out to test durable function
    uncomment when needed


        [Function("BlobEventFromServiceBus")]
        public void Run(
            [ServiceBusTrigger(
                "blob-events",
                "durable-workers",
                Connection = "ServiceBusConnection")]
            ServiceBusReceivedMessage message)
        {
            string body = message.Body.ToString();

            _logger.LogInformation("Service Bus Event Received: {body}", body);

            var eventGridEvent = JsonDocument.Parse(body);

            _logger.LogInformation("Blob event JSON parsed successfully");
        }
        */

    // For poison message handling DLQ
    /*
    This will log into DL
     // uncomment when needed

        [Function("BlobEventFromServiceBus")]
        public async Task Run(
        [ServiceBusTrigger("blob-events", "durable-workers", Connection = "ServiceBusConnection")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions actions,
        ILogger logger)
        {
            try
            {
                var body = message.Body.ToString();

                logger.LogInformation("Received: {body}", body);

                var json = JsonDocument.Parse(body); // will fail for bad payload

                logger.LogInformation("JSON parsed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Poison message detected");

                var props = new Dictionary<string, object>
        {
            { "Reason", "InvalidPayload" },
            { "Exception", ex.Message }
        };

                await actions.DeadLetterMessageAsync(message, props);
            }
        }
        */
    [Function("BlobEventFromServiceBus")]
    public async Task Run(
    [ServiceBusTrigger("blob-events", "durable-workers", Connection = "ServiceBusConnection")]
    ServiceBusReceivedMessage message,
    [DurableClient] DurableTaskClient client)
    {
        var body = message.Body.ToString();

        await client.ScheduleNewOrchestrationInstanceAsync(
            "BlobOrchestrator",
            body);
    }

}