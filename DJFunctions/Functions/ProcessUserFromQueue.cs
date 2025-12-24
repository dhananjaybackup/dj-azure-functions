using DJFunctions.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Microsoft.DurableTask.Client;
using Microsoft.Azure.Functions.Worker.Extensions.DurableTask;
namespace DJFunctions;

public class ProcessUserFromQueue
{
    private readonly ILogger<ProcessUserFromQueue> _logger;

    public ProcessUserFromQueue(ILogger<ProcessUserFromQueue> logger)
    {
        _logger = logger;
    }

    /* Process messages from the "user-queue" queue
     * and insert them into the "Users" table.
        uncomment this section to enable the function
        now below code we are moving for durable function.     
only using DJFunctions.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;
namespace are required.
    
    [Function("ProcessUserFromQueue")]
    [TableOutput("Users")]
    public UserEntity Run(
        [QueueTrigger("user-queue")] string queueMessage)
    {
        _logger.LogInformation("Queue message received: {Message}", queueMessage);

        var user = JsonSerializer.Deserialize<UserEntity>(queueMessage);

        // 🔑 Ensure keys (important)
        user.PartitionKey = user.PartitionKey ?? "Users";
        user.RowKey = user.RowKey ?? Guid.NewGuid().ToString();

        return user;
    }
 */

    /* Durable Function implementation */

    [Function("ProcessUserFromQueue")]
    public async Task Run(
        [QueueTrigger("user-queue")] string message,
        [DurableClient] DurableTaskClient client,
        FunctionContext context)
    {
        var logger = context.GetLogger("ProcessUserFromQueue");

        // Temporary comment out to test durable function
        /*
            string instanceId =
                await client.ScheduleNewOrchestrationInstanceAsync(
                    "UserOrchestrator",
                    message);
        */
        // For durable function with retry policy  
        var user = JsonSerializer.Deserialize<UserDto>(message);

       string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
            "UserOnboardingOrchestrator",
            user);

        logger.LogInformation(
            "Started orchestration with InstanceId: {InstanceId}", instanceId);
    }

}
