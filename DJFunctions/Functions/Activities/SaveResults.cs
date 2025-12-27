using Microsoft.Azure.Functions.Worker;
namespace DJFunctions;
public static class SaveResults
{
    [Function("SaveResults")]
    public static void Run([ActivityTrigger] string[] results)
    {
        foreach (var r in results)
        {
            Console.WriteLine(r);
        }
    }
}