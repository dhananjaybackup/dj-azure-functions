using System.Text.Json.Serialization;

public class CosmosDlqMessage
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("userId")]  // ✅ This maps UserId → userId in JSON
    public string UserId { get; set; }
    
    [JsonPropertyName("userName")]
    public string UserName { get; set; }
    
    [JsonPropertyName("correlationId")]
    public string? CorrelationId { get; set; }
    
    [JsonPropertyName("reason")]
    public string Reason { get; set; }
    
    [JsonPropertyName("failedAt")]
    public string FailedAt { get; set; }
    
    [JsonPropertyName("replayCount")]
    public int ReplayCount { get; set; }
    
    [JsonPropertyName("status")]
    public string Status { get; set; }
}
