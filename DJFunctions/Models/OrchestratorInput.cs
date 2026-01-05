namespace DJFunctions.Models;

public class OrchestratorInput
{
    public UserDto User { get; set; } = default!;
    public CorrelationContext Context { get; set; } = default!;
}
