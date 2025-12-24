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
    // 1. Temporary comment out to test durable function
/*
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
*/
    // 2. For durable function with retry policy 
    [Function("UserOnboardingOrchestrator")]
    public async Task Run(
     [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var user = context.GetInput<UserEntity>();

        var retryPolicy = new RetryPolicy(3, TimeSpan.FromSeconds(5));
        var taskOptions = new TaskOptions(retryPolicy);

        await context.CallActivityAsync("ValidateUserActivity", user, taskOptions);
        await context.CallActivityAsync("SaveUserActivity", user, taskOptions);
    }

}