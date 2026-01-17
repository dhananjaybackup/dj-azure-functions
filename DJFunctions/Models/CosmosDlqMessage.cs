using System.Text.Json.Serialization;

public class CosmosDlqMessage
{
    // REQUIRED by Cosmos
    [JsonPropertyName("id")]
    public string Id { get; set; }               // OrchestrationId

    // Partition key
    [JsonPropertyName("userId")]
    public string UserId { get; set; }

    // Data
    public string UserName { get; set; }
    public string CorrelationId { get; set; }
    public string Reason { get; set; }
    public DateTime FailedAt { get; set; }
    public int ReplayCount { get; set; }
    public string Status { get; set; }
}
