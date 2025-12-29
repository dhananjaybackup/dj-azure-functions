using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Storage;
using DJFunctions.Models;
using Azure.Data.Tables;

namespace DJFunctions;

public class SendToDlqActivity
{

    [Function("SendToDlqActivity")]
    public async Task Run(
    [ActivityTrigger] DlqMessage dlq,
    FunctionContext context)
    {
        var logger = context.GetLogger("SendToDlqActivity");

        var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

        var service = new TableServiceClient(connectionString);
        var table = service.GetTableClient("WorkflowDLQ");

        await table.CreateIfNotExistsAsync();
        await table.AddEntityAsync(dlq);

        logger.LogError("User {UserId} sent to DLQ. Reason: {Error}",
            dlq.UserId, dlq.Reason);
    }
}