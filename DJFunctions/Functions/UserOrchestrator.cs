using DJFunctions.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;

namespace DJFunctions;

public class UserOrchestrator
{
    private readonly ILogger<UserOrchestrator> _logger;

    public UserOrchestrator(ILogger<UserOrchestrator> logger)
    {
        _logger = logger;
    }
[Function("UserOrchestrator")]
public async Task RunOrchestrator(
    [OrchestrationTrigger] TaskOrchestrationContext context)
{
    var userJson = context.GetInput<string>();

    await context.CallActivityAsync("ValidateUserActivity", userJson);
    await context.CallActivityAsync("InsertUserActivity", userJson);
    await context.CallActivityAsync("GetVaultSecretActivity", null);
    await context.CallActivityAsync("SendNotificationActivity", userJson);
}

}