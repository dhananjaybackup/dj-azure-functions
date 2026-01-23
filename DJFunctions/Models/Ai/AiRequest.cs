public class AiRequest
{
   public string UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string? Notes { get; set; }
    public string CorrelationId { get; set; }
    public string SourceBlobUrl { get; set; }
}