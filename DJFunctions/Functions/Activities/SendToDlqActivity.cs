using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Storage;
using DJFunctions.Models;
using Azure.Data.Tables;
using Azure.Identity;

namespace DJFunctions;

public class SendToDlqActivity
{

    [Function("SendToDlqActivity")]
    public async Task Run(
    [ActivityTrigger] DlqMessage dlq,
    FunctionContext context)
    {
        var logger = context.GetLogger("SendToDlqActivity");

        // var accountName = Environment.GetEnvironmentVariable("StorageAccountName");

        // var service = new TableServiceClient(
        //     new Uri($"https://{accountName}.table.core.windows.net"),
        //     new DefaultAzureCredential());

        // var table = service.GetTableClient("WorkflowDLQ");
        var table = DlqTableFactory.GetDlqTable();
        
        await table.CreateIfNotExistsAsync();

        var entity = new TableEntity(
            partitionKey: dlq.OrchestrationId,
            rowKey: Guid.NewGuid().ToString())
        {
            ["UserName"] = string.IsNullOrWhiteSpace(dlq.UserName)
    ? "UNKNOWN"
    : dlq.UserName,
            ["Reason"] = dlq.Reason,
            ["FailedAt"] = dlq.FailedAt
        };

        await table.AddEntityAsync(entity);

        logger.LogError("User {User} sent to DLQ. Reason: {Reason}",
            dlq.UserName, dlq.Reason);
    }
}