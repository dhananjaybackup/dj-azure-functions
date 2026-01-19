using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker;

namespace DJFunctions;

public class ProcessUserWithAi
{
    [Function("ProcessUserWithAi")]
    public async Task<AiResult> Run(
        [ActivityTrigger] AiRequest request,
        FunctionContext context)
    {
        var logger = context.GetLogger("ProcessUserWithAi");

        logger.LogInformation(
            "AI processing started for UserId={UserId}",
            request.UserId
        );

        // Placeholder — we simulate AI for now
        return new AiResult
        {
            IsSuccess = true,
            IsConfident = false, // force decision path
            Reason = "Confidence below threshold",
            ExtractedData = new Dictionary<string, string>()
        };

        // We intentionally return IsConfident = false
        // This allows us to wire workflow logic first, AI later.
    }
}
