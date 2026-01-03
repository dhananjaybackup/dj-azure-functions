using Azure.Data.Tables;

public static class DlqTableFactory
{
    public static TableClient GetDlqTable()
    {
        var conn = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        var client = new TableServiceClient(conn);

        var table = client.GetTableClient("WorkflowDLQ");
        table.CreateIfNotExists();

        return table;
    }
}
