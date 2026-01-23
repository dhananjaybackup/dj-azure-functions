public class AiDecision
{
    public string RiskLevel { get; set; }   // Low | Medium | High
    public string SuggestedAction { get; set; } // AutoApprove | ManualReview | Reject
    public string Reason { get; set; }
}