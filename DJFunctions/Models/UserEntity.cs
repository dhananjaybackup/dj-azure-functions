using Azure;
using Azure.Data.Tables;

namespace DJFunctions.Models;

public class UserEntity : ITableEntity
{
   public string PartitionKey { get; set; } = "USER";
    public string RowKey { get; set; }  // Remove the default value

    public string Name { get; set; }
    public string Email { get; set; }

    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}
