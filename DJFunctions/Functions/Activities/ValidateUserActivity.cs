using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker;
using DJFunctions.Models;

namespace DJFunctions;

public class ValidateUserActivity
{
    [Function("ValidateUserActivity")]
    public Task Run(
       [ActivityTrigger] UserDto user,
        FunctionContext context)
    {
        var logger = context.GetLogger("ValidateUserActivity");
        logger.LogInformation("Validating user data {User}", user.UserName);
        if (string.IsNullOrEmpty(user.Email))
            throw new Exception("Email is required");

        // validation logic
        return Task.CompletedTask;
    }
}