using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker;

namespace DJFunctions;

public class ValidateUserActivity
{
    [Function("ValidateUserActivity")]
    public Task Run(
        [ActivityTrigger] string userJson,
        FunctionContext context)
    {
        var logger = context.GetLogger("ValidateUserActivity");
        logger.LogInformation("Validating user data");

        // validation logic
        return Task.CompletedTask;
    }
}