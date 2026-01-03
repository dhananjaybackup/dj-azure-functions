using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Storage;
using DJFunctions.Models;
using Azure.Data.Tables;
using Azure.Identity;

namespace DJFunctions;

public class WriteDlqRecordActivity
{
    [Function("WriteDlqRecord")]
    public async Task Run(
        [ActivityTrigger] DlqMessage msg)
    {
        var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

        var service = new TableServiceClient(connectionString);
        var table = service.GetTableClient("WorkflowDLQ");

        await table.CreateIfNotExistsAsync();

        msg.PartitionKey = "DLQ";
        msg.RowKey = Guid.NewGuid().ToString();
        msg.Timestamp = DateTimeOffset.UtcNow;

        await table.AddEntityAsync(msg);
    }
}
