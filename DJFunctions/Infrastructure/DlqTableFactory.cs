using Azure.Data.Tables;
using Azure.Identity;

public static class DlqTableFactory
{
    public static TableClient GetDlqTable()
    {
        var accountName = Environment.GetEnvironmentVariable("StorageAccountName");
        var service = new TableServiceClient(
            new Uri($"https://{accountName}.table.core.windows.net"),
            new DefaultAzureCredential());

        var table = service.GetTableClient("WorkflowDLQ");
        return table;
    }
}
