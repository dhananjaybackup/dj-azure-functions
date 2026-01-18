using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Storage;
using DJFunctions.Models;
using Azure.Data.Tables;
using Azure.Identity;
using Microsoft.Azure.Cosmos;

namespace DJFunctions;

public class SendToDlqActivity
{
     private readonly ILogger<SendToDlqActivity> _logger;

    public SendToDlqActivity(ILogger<SendToDlqActivity> logger)
    {
        _logger = logger;
    }

    /*
        [Function("SendToDlqActivity")]
        public async Task Run(
        [ActivityTrigger] DlqMessage dlq,
        FunctionContext context)
        {
            var logger = context.GetLogger("SendToDlqActivity");

            var table = DlqTableFactory.GetDlqTable();
            await table.CreateIfNotExistsAsync();

            var entity = new TableEntity(
                partitionKey: dlq.UserId,       // 👈 USER is the partition
                rowKey: dlq.RowKey)            // 👈 ONE ROW per workflow
            {
                ["UserName"] = string.IsNullOrWhiteSpace(dlq.UserName) ? "UNKNOWN" : dlq.UserName,
                ["CorrelationId"] = dlq.CorrelationId,
                ["LastError"] = dlq.Reason,
                ["FailedAt"] = dlq.FailedAt,
                ["ReplayCount"] = 0,                   // 👈 BIRTH of replay tracking
                ["Status"] = "Failed"
            };

            await table.UpsertEntityAsync(entity);     // 👈 overwrite on repeated failures

            logger.LogError(
                "User sent to DLQ | User={UserId} | Instance={InstanceId} | Reason={Reason}",
                dlq.UserId,
                dlq.RowKey,
                dlq.Reason);
        }
        */
    [Function("SendToDlqActivity")]
    public async Task Run(
    [ActivityTrigger] DlqMessage dlq,
    FunctionContext context)
    {
        // var logger = context.GetLogger("SendToDlqActivity");

        try
        {
            var client = new CosmosClient(
                Environment.GetEnvironmentVariable("CosmosConnection"));
            var container = client.GetContainer(
                "UserManagement",
                "DurableDlq");
// logger.LogInformation("CosmosClient created for DLQ");
_logger.LogInformation("CosmosClient SendToDlqActivity Writing to Cosmos DLQ for User {UserId} and Instance {InstanceId}", dlq.UserId, dlq.RowKey);
            var doc = new CosmosDlqMessage
            {
                Id = dlq.RowKey,           // mapped → "id"
                UserId = dlq.UserId,       // mapped → "userId"
                UserName = dlq.UserName ?? "UNKNOWN",
                CorrelationId = dlq.CorrelationId,
                Reason = dlq.Reason,
                FailedAt = dlq.FailedAt,
                ReplayCount = 0,
                Status = "Failed"
            };

            await container.UpsertItemAsync(
                doc,
                new PartitionKey(doc.UserId));

            _logger.LogError(
                "🚨 DLQ | User={UserId} | Instance={InstanceId} | Reason={Reason}",
                doc.UserId,
                doc.Id,
                doc.Reason);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write DLQ item to Cosmos");
            throw; // VERY important for Durable visibility
        }
    }

}