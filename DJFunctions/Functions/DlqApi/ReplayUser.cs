using System.Net;
using Azure.Data.Tables;
using DJFunctions.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;

namespace DJFunctions;

public class ReplayUser
{
    [Function("ReplayUser")]
    public async Task<HttpResponseData> Run(
    [HttpTrigger(AuthorizationLevel.Function, "post", Route = "dlq/replay/{instanceId}")] HttpRequestData req,
    string instanceId,
    [DurableClient] DurableTaskClient durable)
    {
        var table = DlqTableFactory.GetDlqTable();

        var last = table
            .Query<DlqTableEntity>(x => x.PartitionKey == instanceId)
            .OrderByDescending(x => x.Timestamp)
            .First();

        var user = new UserDto
        {
            UserId = last.UserId,
            UserName = last.UserName
        };

        await durable.ScheduleNewOrchestrationInstanceAsync(
            "UserOnboardingOrchestrator",
            user,
            new StartOrchestrationOptions
            {
                InstanceId = instanceId + "-replay"
            });

        var res = req.CreateResponse(HttpStatusCode.OK);
        await res.WriteStringAsync("Replay started");
        return res;
    }


}