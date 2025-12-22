using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker;

namespace DJFunctions;

public class GetVaultSecretActivity
{
    [Function("GetVaultSecretActivity")]
    public async Task Run(
     [ActivityTrigger] string _,
     FunctionContext context)
    {
        var logger = context.GetLogger("GetVaultSecretActivity");
        logger.LogInformation("Fetching secret from Key Vault");

        // existing Key Vault MSI logic
    }
}