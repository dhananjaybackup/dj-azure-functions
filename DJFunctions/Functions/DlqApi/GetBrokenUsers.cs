using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

public class GetBrokenUsers
{
    [Function("GetBrokenUsers")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "dlq/users")] 
        HttpRequestData req)
    {
        var table = DlqTableFactory.GetDlqTable();

        var all = table.Query<DlqMessage>().ToList();

        var grouped = all
            .GroupBy(x => x.PartitionKey)
            .Select(g => new
            {
                InstanceId = g.Key,
                User = g.Last().UserName,
                Failures = g.Count(),
                LastError = g.Last().Reason,
                LastTime = g.Max(x => x.Timestamp)
            })
            .OrderByDescending(x => x.LastTime);

        var res = req.CreateResponse(HttpStatusCode.OK);
        await res.WriteAsJsonAsync(grouped);
        return res;
    }
}
