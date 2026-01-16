using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using DJFunctions.Models;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
namespace DJFunctions;

public class GetUser
{
    private readonly ILogger _logger;

    public GetUser(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<GetUser>();
    }
    // Get user by partition key and row key
    [Function("GetUser")]
    public HttpResponseData Run( 
    [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req,

    [TableInput(
        tableName: "Users",
        partitionKey: "{query.pk}",
        rowKey: "{query.rk}")]
    UserEntity? user)
    {
        var response = req.CreateResponse();

        if (user == null)
        {
            response.StatusCode = HttpStatusCode.NotFound;
            response.WriteString("User not found");
            return response;
        }

        response.StatusCode = HttpStatusCode.OK;
        response.WriteAsJsonAsync(user);
        return response;
    }
    [Function("GetUserByPrimaryKey")]
    public HttpResponseData GetUserByPrimaryKey(
    [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req,

    [TableInput(
        tableName: "Users",
        partitionKey: "{query.pk}"
        )]
    UserEntity[]? users)
    {
        var response = req.CreateResponse();

        if (users == null)
        {
            response.StatusCode = HttpStatusCode.NotFound;
            response.WriteString("User not found");
            return response;
        }

        response.StatusCode = HttpStatusCode.OK;
        response.WriteAsJsonAsync(users);
        return response;
    }
}