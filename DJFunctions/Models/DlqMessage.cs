using Azure;
using Azure.Data.Tables;
using DJFunctions.Models;
public class DlqMessage : ITableEntity
{
    // Required by Table Storage
    public string PartitionKey { get; set; } = "DLQ";
    public string RowKey { get; set; } = Guid.NewGuid().ToString();

    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    // Your data
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Reason { get; set; }
    public DateTime FailedAt { get; set; }
    public string OrchestrationId { get; set; }
}
