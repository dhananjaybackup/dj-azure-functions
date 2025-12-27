using Microsoft.Azure.Functions.Worker;
namespace DJFunctions;
public static class ProcessPage
{
    [Function("ProcessPage")]
    public static async Task<string> Run([ActivityTrigger] string page)
    {
        await Task.Delay(1000); // simulate work
        return $"Processed {page}";
    }
}