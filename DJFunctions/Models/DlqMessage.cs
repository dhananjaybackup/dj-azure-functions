using DJFunctions.Models;

public class DlqMessage
{
    public UserDto User { get; set; }
    public string Reason { get; set; }
    public DateTime FailedAt { get; set; }
    public string OrchestrationId { get; set; }
}
