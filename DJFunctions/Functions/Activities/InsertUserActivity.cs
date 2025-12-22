using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker;

namespace DJFunctions;

public class InsertUserActivity
{
    [Function("InsertUserActivity")]
    public async Task Run(
        [ActivityTrigger] string userJson,
        FunctionContext context)
    {
        var logger = context.GetLogger("InsertUserActivity");
        logger.LogInformation("Inserting user into storage");

        // existing InsertUser logic
    }
}