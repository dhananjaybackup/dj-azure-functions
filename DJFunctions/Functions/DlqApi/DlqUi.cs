using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

public class DlqUi
{
    [Function("DlqUi")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "dlq")]
        HttpRequestData req)
    {
        var html = await File.ReadAllTextAsync("wwwroot/dlq.html");

        var res = req.CreateResponse(HttpStatusCode.OK);
        res.Headers.Add("Content-Type", "text/html");
        await res.WriteStringAsync(html);
        return res;
    }
}
