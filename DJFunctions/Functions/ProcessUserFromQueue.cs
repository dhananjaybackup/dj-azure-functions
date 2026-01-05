using DJFunctions.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Microsoft.DurableTask.Client;
using Microsoft.Azure.Functions.Worker.Extensions.DurableTask;
using Azure.Storage.Queues.Models;
using Microsoft.DurableTask;
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

    /* Durable Function implementation to process messages from the "user-queue" queue
     and start a new orchestration instance for each message.
    

    [Function("ProcessUserFromQueue")]
    public async Task Run(
        [QueueTrigger("user-queue")] string message,
        [DurableClient] DurableTaskClient client,
        FunctionContext context)
    {
        var logger = context.GetLogger("ProcessUserFromQueue");

        // Temporary comment out to test durable function
        
            // string instanceId =
            //     await client.ScheduleNewOrchestrationInstanceAsync(
            //         "UserOrchestrator",
            //         message);
        
        // For durable function with retry policy  
        var user = JsonSerializer.Deserialize<UserDto>(message);

       string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
            "UserOnboardingOrchestrator",
            user);

        logger.LogInformation(
            "Started orchestration with InstanceId: {InstanceId}", instanceId);
    }
    */
    /* Correct, production-grade version
    only one workflow per user
    check if workflow already exists
   
    [Function("ProcessUserFromQueue")]
    public async Task Run(
    [QueueTrigger("user-queue")] QueueMessage queueMessage,
    [DurableClient] DurableTaskClient client,
    FunctionContext context)
    {
        var logger = context.GetLogger("ProcessUserFromQueue");

        var body = queueMessage.MessageText;
        var user = JsonSerializer.Deserialize<UserDto>(body);

        // 🔑 One user = one deterministic workflow
        var instanceId = $"user-{user.UserId}";

        // Prevent duplicate workflows
        var existing = await client.GetInstanceAsync(instanceId);
        if (existing != null)
        {
            logger.LogWarning(
                "Workflow already exists | User={UserId} | MessageId={MessageId} | InstanceId={InstanceId}",
                user.UserId,
                queueMessage.MessageId,
                instanceId);
            return;
        }

        await client.ScheduleNewOrchestrationInstanceAsync(
            "UserOnboardingOrchestrator",
            user,
            new StartOrchestrationOptions
            {
                InstanceId = instanceId
            });

        logger.LogInformation(
            "Workflow started | User={UserId} | MessageId={MessageId} | InstanceId={InstanceId} | Dequeue={Dequeue}",
            user.UserId,
            queueMessage.MessageId,
            instanceId,
            queueMessage.DequeueCount);
    }
 */
 /* Final version with CorrelationContext
    to track correlation across services
    */
    [Function("ProcessUserFromQueue")]
    public async Task Run(
    [QueueTrigger("user-queue")] QueueMessage queueMessage,
    [DurableClient] DurableTaskClient client,
    FunctionContext context)
    {
        
        var logger = context.GetLogger("ProcessUserFromQueue");

        var body = queueMessage.MessageText;
        var user = JsonSerializer.Deserialize<UserDto>(body);

        // 🔑 One user = one deterministic workflow
        var instanceId = $"user-{user.UserId}";

        // Prevent duplicate workflows
        var existing = await client.GetInstanceAsync(instanceId);
        if (existing != null)
        {
            logger.LogWarning(
                "Workflow already exists | User={UserId} | MessageId={MessageId} | InstanceId={InstanceId}",
                user.UserId,
                queueMessage.MessageId,
                instanceId);
            return;
        }

        var correlationContext = new CorrelationContext
        {
            UserId = user.UserId,
            UserName = user.UserName,
            CorrelationId = Guid.NewGuid().ToString(),
            OrchestrationId = instanceId
        };

        var orchestratorInput = new OrchestratorInput
        {
            User = user,
            Context = correlationContext
        };

        await client.ScheduleNewOrchestrationInstanceAsync(
            "UserOnboardingOrchestrator",
            orchestratorInput,
            new StartOrchestrationOptions
            {
                InstanceId = instanceId
            });

        logger.LogInformation(
            "Workflow started | User={UserId} | MessageId={MessageId} | InstanceId={InstanceId} | Dequeue={Dequeue}",
            user.UserId,
            queueMessage.MessageId,
            instanceId,
            queueMessage.DequeueCount);
    }

}
