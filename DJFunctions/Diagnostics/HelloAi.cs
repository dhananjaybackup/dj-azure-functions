using Azure.AI.OpenAI;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using Azure;
using OpenAI.Chat;

namespace DJFunctions.Diagnostics
{
    public class HelloAi
    {
        private readonly AzureOpenAIClient _client;
        private readonly string _deploymentName;
        // public HelloAi(AzureOpenAIClient client)
        // {
        //     var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
        //     var key = Environment.GetEnvironmentVariable("AZURE_OPENAI_KEY");
        //     _deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME");

        //     _client = new AzureOpenAIClient(
        //         new Uri(endpoint),
        //         new AzureKeyCredential(key)
        //     );
        // }
        public HelloAi(AzureOpenAIClient client)
        {
            _client = client;
        }
        [Function("HelloAi")]
        public async Task<HttpResponseData> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {

            /* var deployment = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT");
             // Get message from query string or request body
             string userMessage = req.Query["message"] ?? "Hello, how are you?";
             // Define system prompt
             string systemPrompt = "Be concise.";
             var chatClient = _client.GetChatClient(_deploymentName);
             var messages = new List<ChatMessage>
             {
                 new SystemChatMessage(systemPrompt),
                 new UserChatMessage(userMessage)
             };
             var chatOptions = new ChatCompletionOptions
             {
                 Temperature = 0.7f,
                 MaxOutputTokenCount = 40,
                 TopP = 0.95f
             };

 */
// var endpoint = new Uri("https://abhis-mkm7pubf-eastus2.cognitiveservices.azure.com/");
var endpoint = new Uri(Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT_MS_FOUNDRY"));
// var deploymentName = "gpt-4o-mini";
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME");
var apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_KEY__MS_FOUNDRY");

AzureOpenAIClient azureClient = new(
    endpoint,
    new AzureKeyCredential(apiKey));
ChatClient chatClient = azureClient.GetChatClient(deploymentName);

var requestOptions = new ChatCompletionOptions()
{
    MaxOutputTokenCount = 4096,
    Temperature = 1.0f,
    TopP = 1.0f,
    
};
 string userMessage = req.Query["message"] ?? "Hello, how are you?";
             // Define system prompt
string systemPrompt = "Be concise.";

List<ChatMessage> messages = new List<ChatMessage>()
{
    new SystemChatMessage(systemPrompt),
    new UserChatMessage(userMessage),
};

var response = chatClient.CompleteChat(messages, requestOptions);
string reply = response.Value.Content[0].Text;

// System.Console.WriteLine(response.Value.Content[0].Text);
return await Task.Run(() =>
            {
                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
                response.WriteString(reply);
                return response;
            }); 


    
//  var endpoint = new Uri("https://abhis-mkm7pubf-eastus2.cognitiveservices.azure.com/");
// var model = "gpt-4o-mini";
// var deploymentName = "gpt-4o-mini";
// var apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_KEY");

// AzureOpenAIClient azureClient = new(
//     endpoint,
//     new AzureKeyCredential(apiKey));
// ChatClient chatClient = azureClient.GetChatClient(deploymentName);

            // 🔹 Create ChatClient from AzureOpenAIClient
            
        }
    }

}