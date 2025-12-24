using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using System.Text.Json;
using DJFunctions.Models;
using Microsoft.Extensions.Logging;

namespace DJFunctions;

public class EnqueueUserFunction
{
     private readonly ILogger _logger;

    public EnqueueUserFunction(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<InsertUser>();
    }
//     [Function("EnqueueUser")]
// public async Task<HttpResponseData> Run(
//     [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
//     [QueueOutput("user-queue")] IAsyncCollector<string> queue)
// {
//     var user = await JsonSerializer.DeserializeAsync<UserEntity>(req.Body);

//     await queue.AddAsync(JsonSerializer.Serialize(user));

//     var response = req.CreateResponse(HttpStatusCode.Accepted);
//     response.WriteString("User queued successfully");
//     return response;
// }
[Function("EnqueueUser")]
[QueueOutput("user-queue")] // The return string goes to this queue
public async Task<string> Run(
    [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
{
    _logger.LogWarning("🔥 EnqueueUser triggered");
    // var user = await JsonSerializer.DeserializeAsync<UserEntity>(req.Body);
    var user = await JsonSerializer.DeserializeAsync<UserDto>(req.Body);
    // The string returned here is automatically added to the queue
    return JsonSerializer.Serialize(user);
}
}
