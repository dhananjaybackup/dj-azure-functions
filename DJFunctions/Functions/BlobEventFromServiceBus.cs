using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.DurableTask.Client;
using DJFunctions.Models;
using Microsoft.DurableTask;
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
    ServiceBusMessageActions actions,
    [DurableClient] DurableTaskClient client)
    {
        var deliveryCount = message.DeliveryCount;
        var messageId = message.MessageId;
        UserDto? user = null;
        try
        {
            // Always try to deserialize first
            var body = message.Body.ToString();
            user = JsonSerializer.Deserialize<UserDto>(body);
            // 1️⃣ Poison detection FIRST
            if (deliveryCount >= 5)
            {
                await client.ScheduleNewOrchestrationInstanceAsync(
                    "SendToDlqOrchestrator",
                    new DlqMessage
                    {
                        UserId = user?.UserId ?? messageId,
                        UserName = user?.UserName ?? "UNKNOWN",
                        Reason = $"Poison message after {deliveryCount} retries",
                        FailedAt = DateTime.UtcNow,
                        OrchestrationId = "INGRESS"
                    });

                await actions.DeadLetterMessageAsync(message);
                return;
            }

            // 2️⃣ Simulate failure (remove this later)
            if (message.ApplicationProperties.TryGetValue("SimulateError", out var v)
                && v?.ToString() == "true")
            {
                throw new Exception("🔥 Simulated corruption");
            }

            // 4️⃣ Exactly-once instance id
            var instanceId = $"user-{messageId}";

            // 5️⃣ Start or resume workflow
            await client.ScheduleNewOrchestrationInstanceAsync(
                "UserOnboardingOrchestrator",
                user,
                new StartOrchestrationOptions { InstanceId = instanceId });

            // 6️⃣ Complete message
            await actions.CompleteMessageAsync(message);
        }
        catch
        {
            await actions.AbandonMessageAsync(message);
            throw;
        }
    }

}