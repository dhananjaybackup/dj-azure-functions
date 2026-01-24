using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker;
using Azure.AI.OpenAI;
using OpenAI.Chat;
using System.Text.Json;
using Azure;
using DJFunctions.Models;
// using OpenAI;

namespace DJFunctions;

public class ProcessUserWithAi
{

    private readonly AzureOpenAIClient _openAiClient;
    private readonly ILogger<ProcessUserWithAi> _logger;

    public ProcessUserWithAi(
        AzureOpenAIClient openAiClient,
        ILogger<ProcessUserWithAi> logger)
    {
        _openAiClient = openAiClient;
        _logger = logger;
    }

    [Function("ProcessUserWithAi")]
    public async Task<AiResult> Run(
      [ActivityTrigger] UserDto user)
    {
        _logger.LogInformation(
            "AI Processing STARTED | UserId={UserId} | CorrelationId={CorrelationId}",
            user.UserId, user.CorrelationId);
        var systemPrompt =
                    """
            You are an AI assistant helping with user onboarding decisions.

            Rules:
            - Analyze the provided user information
            - Extract structured data
            - Decide if onboarding can be auto-approved
            - Be conservative: if unsure, mark as NOT confident
            - Respond ONLY in valid JSON
            """;
        var userPrompt = $@"
User onboarding request:

UserId: {user.UserId}
Name: {user.UserName}
Email: {user.Email}
CorrelationId: {user.CorrelationId}

Blob reference: {user.BlobUrl}

Output JSON format:
{{
  ""isSuccess"": true|false,
  ""isConfident"": true|false,
  ""reason"": ""short explanation"",
  ""extractedData"": {{
     ""emailDomain"": """",
     ""riskLevel"": """",
     ""decision"": """"
  }}
}}
"; ;

        var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME");
        var chatClient = _openAiClient.GetChatClient(deploymentName); // Foundry deployment name
        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(systemPrompt),
            new UserChatMessage(userPrompt)
        };
        var options = new ChatCompletionOptions
        {
            MaxOutputTokenCount = 4096,
            Temperature = 0.1f,
            TopP = 1.0f
        };
        try
        {
            var response = await chatClient.CompleteChatAsync(messages);

            var raw = response.Value.Content[0].Text;

            _logger.LogInformation(
                "AI RAW RESPONSE | UserId={UserId} | Response={Response}",
                user.UserId, raw);

            var aiResult = JsonSerializer.Deserialize<AiResult>(
                raw,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (aiResult == null)
            {
                return new AiResult
                {
                    IsSuccess = false,
                    IsConfident = false,
                    Reason = "AI returned empty or invalid response"
                };
            }

            return aiResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "AI Processing FAILED | UserId={UserId}",
                user.UserId);

            return new AiResult
            {
                IsSuccess = false,
                IsConfident = false,
                Reason = "AI exception: " + ex.Message
            };
        }
    }
}
