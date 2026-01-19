using Azure;
using Azure.Data.Tables;

namespace DJFunctions.Models;

public class UserEntity : ITableEntity
{
   public string PartitionKey { get; set; }
    public string RowKey { get; set; }  // Remove the default value
    public string UserId { get; set; }

    public string UserName { get; set; }
    public string Email { get; set; }

    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}

public class UserDto
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string CorrelationId { get; set; }
    public string BlobUrl { get; set; }
    public int ReplayCount { get; set; }
}