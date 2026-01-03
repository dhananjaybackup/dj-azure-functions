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
        // var connection = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        // var service = new TableServiceClient(connection);
        // var table = service.GetTableClient("WorkflowDLQ");
        var table = DlqTableFactory.GetDlqTable();
        // LEGAL Azure Table scan
        var rows = table.Query<DlqMessage>(
            x => x.PartitionKey.CompareTo("user-") >= 0
        ).ToList();

        var users = rows
     .GroupBy(x => x.PartitionKey)
     .Select(g => new
     {
         InstanceId = g.Key,
         User = g.First().UserName ?? "UNKNOWN",
         Failures = g.Count(),
         LastError = g
             .OrderByDescending(x => x.FailedAt)
             .First().Reason ?? "Unknown failure",
         LastTime = g.Max(x => x.FailedAt)
     })
     .OrderByDescending(x => x.LastTime)
     .ToList();

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(users);
        return response;
    }
}
