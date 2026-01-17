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
         var logger = context.GetLogger("SendToDlqActivity");
        var client = new CosmosClient(
    Environment.GetEnvironmentVariable("CosmosConnection"));

        var container = client
            .GetContainer("UserManagement", "DurableDlq");

        var doc = new CosmosDlqMessage
        {
            Id = dlq.RowKey,                // OrchestrationId
            UserId = dlq.UserId,            // Partition key
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

        logger.LogError(
            "User sent to DLQ | User={UserId} | Correlation={CorrelationId} | Reason={Reason}",
            dlq.UserId,
            dlq.CorrelationId,
            dlq.Reason);
    }

}