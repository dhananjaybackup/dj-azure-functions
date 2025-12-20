using DJFunctions.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace DJFunctions;

public class ProcessUserFromQueue
{
    private readonly ILogger<ProcessUserFromQueue> _logger;

    public ProcessUserFromQueue(ILogger<ProcessUserFromQueue> logger)
    {
        _logger = logger;
    }

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
}
