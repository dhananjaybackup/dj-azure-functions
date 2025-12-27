using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
namespace DJFunctions;
public static class BlobOrchestrator
{
    [Function("BlobOrchestrator")]
    public static async Task Run(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var blobEvent = context.GetInput<string>();

        var tasks = new List<Task<string>>();

        // Simulate splitting into pages
        for (int i = 1; i <= 5; i++)
        {
            tasks.Add(context.CallActivityAsync<string>(
                "ProcessPage",
                $"Page-{i} | {blobEvent}"));
        }

        var results = await Task.WhenAll(tasks);

        await context.CallActivityAsync("SaveResults", results);
    }
}