using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
namespace DJFunctions;

public static class SendToDlqOrchestrator
{
    [Function("SendToDlqOrchestrator")]
    public static async Task Run(
        [OrchestrationTrigger] TaskOrchestrationContext ctx)
    {
        var msg = ctx.GetInput<DlqMessage>();

        await ctx.CallActivityAsync("WriteDlqRecord", msg);
    }
}