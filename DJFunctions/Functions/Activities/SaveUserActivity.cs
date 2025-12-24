using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker;
using DJFunctions.Models;

namespace DJFunctions;

public class SaveUserActivity
{
    [Function("SaveUserActivity")]
    public async Task Run(
        [ActivityTrigger] UserEntity user,
        FunctionContext context)
    {
        var logger = context.GetLogger("SaveUserActivity");
        logger.LogInformation("Saving user to storage");

        // existing SaveUser logic
        logger.LogInformation("Saving user");
        throw new Exception("SQL Server down");
    }
}