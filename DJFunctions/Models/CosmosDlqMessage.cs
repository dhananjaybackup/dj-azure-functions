using System.Text.Json.Serialization;

public class CosmosDlqMessage
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    // Partition Key
    public string UserId { get; set; }

    // Operational metadata
    public string UserName { get; set; }
    public string CorrelationId { get; set; }
    public string Reason { get; set; }

    public DateTime FailedAt { get; set; }
    public int ReplayCount { get; set; }
}
