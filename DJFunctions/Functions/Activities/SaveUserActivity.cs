using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker;
using DJFunctions.Models;

namespace DJFunctions;

public class SaveUserActivity
{
    [Function("SaveUserActivity")]
    public async Task Run(
        [ActivityTrigger] UserDto user,
        FunctionContext context)
    {
        var logger = context.GetLogger("SaveUserActivity");
        logger.LogInformation("Saving user to storage");
        var entity = new UserEntity
        {
            PartitionKey = user.PartitionKey,
            RowKey = user.RowKey,
            Name = user.Name,
            Email = user.Email
        };
        // Simulate failure for testing retry policy    
        // existing SaveUser logic
        logger.LogInformation("Saving user {User} to database", entity.Name );
        // throw new Exception("SQL Server down");
    }
}