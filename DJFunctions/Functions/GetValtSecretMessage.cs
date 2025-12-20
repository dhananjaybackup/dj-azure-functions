using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace DJFunctions;

public class GetValtSecretMessage
{
    private readonly ILogger<ProcessUserFromQueue> _logger;

    public GetValtSecretMessage(ILogger<ProcessUserFromQueue> logger)
    {
        _logger = logger;
    }

    [Function("GetSecretMessage")]
    public HttpResponseData Run(
        [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
    {
        var secret = Environment.GetEnvironmentVariable("AppMessage");

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.WriteString($"Message from Key Vault Changed: {secret}");
        return response;
    }
}
