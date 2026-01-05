namespace DJFunctions.Models;

public class CorrelationContext
{
    public string UserId { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string CorrelationId { get; set; } = default!;
    public string OrchestrationId { get; set; } = default!;
}
