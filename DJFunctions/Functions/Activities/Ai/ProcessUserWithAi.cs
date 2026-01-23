using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker;
using Azure.AI.OpenAI;
using OpenAI.Chat;
using System.Text.Json;
using Azure;
// using OpenAI;

namespace DJFunctions;

public class ProcessUserWithAi
{

    private readonly ChatClient _chatClient;
    private readonly ILogger _logger;

    private readonly AzureOpenAIClient _openAiClient;
    // private readonly ILogger _logger;

    public ProcessUserWithAi(
         AzureOpenAIClient azureClient,
         ILoggerFactory loggerFactory)
    {
        _chatClient = azureClient.GetChatClient(
            Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT"));

        _logger = loggerFactory.CreateLogger<ProcessUserWithAi>();
    }
    [Function("ProcessUserWithAi")]
    public async Task<AiResult> Run(
        [ActivityTrigger] AiRequest request)
    {
        _logger.LogInformation(
            "AI processing started | UserId={UserId} | CorrelationId={CorrelationId}",
            request.UserId,
            request.CorrelationId);

        try
        {
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(
                    "You are an onboarding AI. You MUST return valid JSON only."),
                new UserChatMessage(BuildPrompt(request))
            };

            var options = new ChatCompletionOptions
            {
                MaxOutputTokenCount = 4096,
                Temperature = 0.1f,
                TopP = 1.0f
            };

            ChatCompletion completion =
                await _chatClient.CompleteChatAsync(messages, options);

            var content = completion.Content[0].Text;

            _logger.LogInformation(
                "AI raw response | UserId={UserId}: {Response}",
                request.UserId,
                content);

            var result = JsonSerializer.Deserialize<AiResult>(
                content,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (result == null)
            {
                throw new Exception("AI returned invalid JSON");
            }

            result.IsSuccess = true;
            result.ExtractedData ??= new Dictionary<string, string>();

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "AI processing failed | UserId={UserId} | CorrelationId={CorrelationId}",
                request.UserId,
                request.CorrelationId);

            return new AiResult
            {
                IsSuccess = false,
                IsConfident = false,
                Reason = ex.Message,
                ExtractedData = new Dictionary<string, string>()
            };
        }
    }
    private static string BuildPrompt(AiRequest request)
    {
        return $$"""
        Analyze the following user onboarding data.

        UserName: {request.UserName}
        Email: {request.Email}
        Notes: {request.Notes}

        Decide onboarding risk and next action.

        Return ONLY valid JSON in this exact format:
        {
          "IsSuccess": true,
          "IsConfident": true | false,
          "Reason": "Short explanation",
          "ExtractedData": {
            "RiskLevel": "Low | Medium | High",
            "SuggestedAction": "AutoApprove | ManualReview | Reject",
            "EmailDomainType": "Corporate | Free | Disposable"
          }
        }
        """;
    }
}
