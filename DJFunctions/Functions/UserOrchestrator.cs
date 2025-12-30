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
    /*
    [Function("UserOnboardingOrchestrator")]
    public async Task Run(
     [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var user = context.GetInput<UserDto>();

        var retryPolicy = new RetryPolicy(3, TimeSpan.FromSeconds(5));
        var taskOptions = new TaskOptions(retryPolicy);

        await context.CallActivityAsync("ValidateUserActivity", user, taskOptions);
        await context.CallActivityAsync("SaveUserActivity", user, taskOptions);
    }
    */
    // 3. Fan-Out / Fan-In distributed processing
    [Function("UserOnboardingOrchestrator")]
    public async Task Run(
    [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var user = context.GetInput<UserDto>();
        var retryPolicy = new RetryPolicy(3, TimeSpan.FromSeconds(5));
        var taskOptions = new TaskOptions(retryPolicy);
        var instanceId = context.InstanceId;
        var input = new ActivityInput
        {
            UserId = user.UserId,
            UserName = user.Name,
            InstanceId = instanceId
        };

        try
        {
            // FAN-OUT (run in parallel)
            var validateTask = context.CallActivityAsync<bool>("ValidateUserActivity", user);
            var saveTask = context.CallActivityAsync("SaveUserActivity", user);
            await context.CallActivityAsync("SendWelcomeEmail", input);
            // FAN-IN (wait for all to complete)
            await Task.WhenAll(validateTask, saveTask);
        }
        catch (Exception ex)
        {
            await context.CallActivityAsync("SendToDlqActivity", new DlqMessage
            {
                UserId = user.UserId,
                UserName = user.Name,
                Reason = ex.Message,
                FailedAt = context.CurrentUtcDateTime,
                OrchestrationId = instanceId
            });
        }
    }


}