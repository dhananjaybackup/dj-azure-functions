using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker;

namespace DJFunctions;

public class SendWelcomeEmail
{
    public async Task Run([ActivityTrigger] ActivityInput input, FunctionContext context)
    {
        var logger = context.GetLogger("SendWelcomeEmail");

        try
        {
            logger.LogInformation(
                "Email Start | User={UserId} | InstanceId={InstanceId}",
                input.UserId,
                input.InstanceId);

            // TODO: Call real email service here
            // await Task.Delay(500); // simulate SMTP
            throw new Exception("SMTP server unreachable");
            logger.LogInformation(
                "Email Success | User={UserId} | InstanceId={InstanceId}",
                input.UserId,
                input.InstanceId);
        }
        catch (Exception ex)
        {
            // 🔥 This is critical for DLQ accuracy
            logger.LogError(
                ex,
                "Email Failed | User={UserId} | InstanceId={InstanceId}",
                input.UserId,
                input.InstanceId);

            throw; // DO NOT swallow — let Durable retry & escalate
        }
    }
}
