using Azure;
using Azure.Data.Tables;
using DJFunctions.Models;
public class DlqMessage : ITableEntity
{
    // Required by Table Storage
   // Table keys
    public string PartitionKey { get; set; }   // UserId
    public string RowKey { get; set; }         // OrchestrationId

    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    // Data
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string CorrelationId { get; set; }

    public string Reason { get; set; }
    public DateTime FailedAt { get; set; }

    public int ReplayCount { get; set; }
}
