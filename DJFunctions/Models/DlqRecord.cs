public class DlqRecord
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string CorrelationId { get; set; }
    public string Reason { get; set; }
    public DateTime FailedAt { get; set; }
    public int ReplayCount { get; set; }
}
