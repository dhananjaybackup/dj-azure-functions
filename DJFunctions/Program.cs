
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.AI.OpenAI;
using Azure;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
         var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
        var key = Environment.GetEnvironmentVariable("AZURE_OPENAI_KEY");

        services.AddSingleton(_ =>
            new AzureOpenAIClient(
                new Uri(endpoint),
                new AzureKeyCredential(key)
            ));
    })
    .ConfigureLogging(logging =>
    {
        logging.AddApplicationInsights();
        logging.SetMinimumLevel(LogLevel.Information);
    })
    .Build();
    // 🔥 TEMP SMOKE TEST — REMOVE AFTER SUCCESS
// await DJFunctions.Diagnostics.CosmosSmokeTest.RunAsync(
//     host.Services.GetRequiredService<ILoggerFactory>()
//         .CreateLogger("CosmosSmokeTest"));

host.Run();
