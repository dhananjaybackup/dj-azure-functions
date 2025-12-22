using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker;

namespace DJFunctions;

public class SendNotificationActivity
{
    [Function("SendNotificationActivity")]
    public Task Run(
     [ActivityTrigger] string userJson,
     FunctionContext context)
    {
        var logger = context.GetLogger("SendNotificationActivity");
        logger.LogInformation("Sending notification");

        return Task.CompletedTask;
    }
}