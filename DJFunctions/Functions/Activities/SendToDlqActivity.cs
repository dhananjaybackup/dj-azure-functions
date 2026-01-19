using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Storage;
using DJFunctions.Models;
using Azure.Data.Tables;
using Azure.Identity;
using Microsoft.Azure.Cosmos;
using System.Text;

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
    private string SanitizeId(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return Guid.NewGuid().ToString();
        }

        // Cosmos DB ID restrictions:
        // - Can't contain: / \ ? #
        // - Max 255 characters
        var sanitized = id
            .Replace("/", "-")
            .Replace("\\", "-")
            .Replace("?", "-")
            .Replace("#", "-")
            .Trim();

        if (sanitized.Length > 255)
        {
            sanitized = sanitized.Substring(0, 255);
        }

        return string.IsNullOrWhiteSpace(sanitized) ? Guid.NewGuid().ToString() : sanitized;
    }
    [Function("SendToDlqActivity")]
    public async Task Run(
    [ActivityTrigger] DlqMessage dlq,
    FunctionContext context)
    {
        // var logger = context.GetLogger("SendToDlqActivity");

        try
        {
            _logger.LogInformation(
                    "SendToDlqActivity START - UserId: {UserId}, RowKey: {RowKey}",
                    dlq?.UserId, dlq?.RowKey);

            if (dlq == null)
            {
                throw new ArgumentNullException(nameof(dlq), "DlqMessage cannot be null");
            }
            var cosmosEndpoint = Environment.GetEnvironmentVariable("CosmosEndpoint");

            if (string.IsNullOrEmpty(cosmosEndpoint))
            {
                _logger.LogError("CosmosEndpoint environment variable is not set");
                throw new InvalidOperationException("CosmosEndpoint not configured. Please add it to Application Settings.");
            }

            _logger.LogInformation("Creating CosmosClient with Managed Identity - Endpoint: {Endpoint}", cosmosEndpoint);

            // ✅ CORRECT: Use endpoint + Managed Identity (NOT connection string)
            var client = new CosmosClient(
                accountEndpoint: cosmosEndpoint,
                tokenCredential: new DefaultAzureCredential());
            var container = client.GetContainer("UserManagement", "DurableDlq");

            // logger.LogInformation("CosmosClient created for DLQ");

//             var safeId = Convert.ToHexString(
//      System.Security.Cryptography.SHA256.HashData(
//          Encoding.UTF8.GetBytes(dlq.RowKey)
//      )
//  );
            var safeId = SanitizeId(dlq.RowKey);
            _logger.LogInformation("DLQ RAW RowKey = {RowKey}", dlq.RowKey);
            var doc = new CosmosDlqMessage
            {
                Id = safeId,
                UserId = dlq.UserId ?? "UNKNOWN",
                UserName = dlq.UserName ?? "UNKNOWN",
                // CorrelationId = dlq.CorrelationId ?? string.Empty,
                // Reason = dlq.Reason ?? string.Empty, 
                // FailedAt = dlq.FailedAt.ToString("o"),
                // ReplayCount = 0,//dlq.ReplayCount,
                Status = "Failed"
            };
            // var doc = new 
            // {
            //     id = safeId,
            //     userId = SanitizeId(dlq.UserId) ?? "UNKNOWN",
            //     userName = dlq.UserName ?? "UNKNOWN"               
            // };
            if (string.IsNullOrWhiteSpace(safeId))
                throw new Exception("DLQ Cosmos id is empty");

            if (string.IsNullOrWhiteSpace(doc.UserId))
                throw new Exception("DLQ Cosmos partition key (userId) is empty");
            _logger.LogInformation(
                      "Upserting document - Id: {Id}, UserId: {UserId}, UserName: {UserName}",
                      doc.Id, doc.UserId, doc.UserName);
            var response = await container.UpsertItemAsync(
                  doc,
                  new PartitionKey(doc.UserId));
            //             var response = await container.UpsertItemAsync(
            //     new
            //     {
            //         id = "test-123",
            //         userId = "U1",
            //         status = "Failed"
            //     },
            //     new PartitionKey("U1")
            // );
            _logger.LogInformation("SendToDlqActivity SUCCESS - StatusCode: {StatusCode}, RU: {RU}",
                       response.StatusCode, response.RequestCharge);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
             "SendToDlqActivity FAILED - UserId: {UserId}, Message: {Message}",
            dlq.UserId, ex.Message);
            throw; // VERY important for Durable visibility
        }
    }

}