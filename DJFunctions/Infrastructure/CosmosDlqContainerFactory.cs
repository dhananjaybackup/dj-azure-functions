using Microsoft.Azure.Cosmos;

public static class CosmosDlqContainerFactory
{
    private static Container _container;
    public static Container GetContainer()
    {
        if (_container != null)
            return _container;

        var conn = Environment.GetEnvironmentVariable("CosmosConnection");
        var client = new CosmosClient(conn);

        _container = client
            .GetDatabase("UserManagement")
            .GetContainer("DurableDlq");

        return _container;
    }
}
