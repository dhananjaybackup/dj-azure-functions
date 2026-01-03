using System.Net;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Text.Json;

public class GetBrokenUsers
{
    [Function("GetBrokenUsers")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "dlq/users")]
        HttpRequestData req)
    {
        var connection = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        var service = new TableServiceClient(connection);
        var table = service.GetTableClient("WorkflowDLQ");

        // LEGAL Azure Table scan
        var rows = table.Query<DlqMessage>(
            x => x.PartitionKey.CompareTo("user-") >= 0
        ).ToList();

        var users = rows
            .GroupBy(x => x.PartitionKey)   // Durable InstanceId
            .Select(g => new
            {
                InstanceId = g.Key,
                User = g.First().UserName,
                Failures = g.Count(),
                LastError = g.OrderByDescending(x => x.FailedAt).First().Reason,
                LastTime = g.Max(x => x.FailedAt)
            })
            .OrderByDescending(x => x.LastTime)
            .ToList();

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(users);
        return response;
    }
}
