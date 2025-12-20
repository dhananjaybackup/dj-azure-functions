using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using DJFunctions.Models;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
namespace DJFunctions;

public class InsertUser
{
    private readonly ILogger _logger;

    public InsertUser(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<InsertUser>();
    }

    [Function("InsertUser")]
    [TableOutput("Users")]
    public UserEntity Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {

        try
        {
            req.Headers.TryGetValues("name", out var nameValues);
            req.Headers.TryGetValues("email", out var emailValues);
            var name = nameValues?.FirstOrDefault() ?? "Dhananjay";
            var email = emailValues?.FirstOrDefault() ?? "dhananjaysharma7@gmail.com";
        _logger.LogInformation("User inserted into Table Storage.");
            return new UserEntity
            {
                PartitionKey = "Users",
                RowKey = Guid.NewGuid().ToString(),
                Name = name,
                Email = email
            };

        }catch  (Exception ex)
        {
            _logger.LogError($"Error inserting user: {ex.Message}");
        }
        return null;
    }
}