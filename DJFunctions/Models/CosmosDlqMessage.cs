using System.Text.Json.Serialization;

public class CosmosDlqMessage
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("userId")]
    public string UserId { get; set; } = string.Empty;
    
    [JsonPropertyName("userName")]
    public string UserName { get; set; } = string.Empty;
    
    [JsonPropertyName("correlationId")]
    public string CorrelationId { get; set; } = string.Empty;
    
    [JsonPropertyName("reason")]
    public string Reason { get; set; } = string.Empty;
    
    [JsonPropertyName("failedAt")]
    public string FailedAt { get; set; } = string.Empty;  // ✅ Must be string, not DateTime
    
    [JsonPropertyName("replayCount")]
    public int ReplayCount { get; set; }
    
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}
