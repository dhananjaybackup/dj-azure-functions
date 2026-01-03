using System.Net;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

public class GetUserFailures
{
    [Function("GetUserFailures")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "dlq/user/{instanceId}")] 
        HttpRequestData req,
        string instanceId)
    {
        var table = DlqTableFactory.GetDlqTable();

        var rows = table
            .Query<DlqMessage>(x => x.PartitionKey == instanceId)
            .OrderByDescending(x => x.Timestamp)
            .ToList();

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(rows);

        return response;
    }
}
