using Cosmos.Samples.NoSQL.Quickstart.Web.Models;
using Microsoft.Azure.Cosmos;

internal interface IDemoService
{
    Task RunAsync(Func<string, Task> writeOutputAync);

    string GetEndpoint();
}

internal sealed class DemoService(CosmosClient client) : IDemoService
{
    public string GetEndpoint() => $"{client.Endpoint}";

    public async Task RunAsync(Func<string, Task> writeOutputAync)
    {
        Database database = client.GetDatabase("cosmicworks");

        database = await database.ReadAsync();
        await writeOutputAync($"Get database:\t{database.Id}");

        Container container = database.GetContainer("products");

        container = await container.ReadContainerAsync();
        await writeOutputAync($"Get container:\t{container.Id}");

        {
            Product item = new(
                id: "68719518391",
                category: "gear-surf-surfboards",
                name: "Yamba Surfboard",
                quantity: 12,
                price: 850.00m,
                clearance: false
            );

            ItemResponse<Product> response = await container.UpsertItemAsync<Product>(
                item: item,
                partitionKey: new PartitionKey("gear-surf-surfboards")
            );

            await writeOutputAync($"Upserted item:\t{response.Resource}");
            await writeOutputAync($"Status code:\t{response.StatusCode}");
            await writeOutputAync($"Request charge:\t{response.RequestCharge:0.00}");
        }

        {
            Product item = new(
                id: "68719518371",
                category: "gear-surf-surfboards",
                name: "Kiama Classic Surfboard",
                quantity: 25,
                price: 790.00m,
                clearance: false
            );

            ItemResponse<Product> response = await container.UpsertItemAsync<Product>(
                item: item,
                partitionKey: new PartitionKey("gear-surf-surfboards")
            );
            await writeOutputAync($"Upserted item:\t{response.Resource}");
            await writeOutputAync($"Status code:\t{response.StatusCode}");
            await writeOutputAync($"Request charge:\t{response.RequestCharge:0.00}");
        }

        {
            ItemResponse<Product> response = await container.ReadItemAsync<Product>(
                id: "68719518391",
                partitionKey: new PartitionKey("gear-surf-surfboards")
            );

            await writeOutputAync($"Read item id:\t{response.Resource.id}");
            await writeOutputAync($"Read item:\t{response.Resource}");
            await writeOutputAync($"Status code:\t{response.StatusCode}");
            await writeOutputAync($"Request charge:\t{response.RequestCharge:0.00}");
        }

        {
            var query = new QueryDefinition(
                query: "SELECT * FROM products p WHERE p.category = @category"
            )
                .WithParameter("@category", "gear-surf-surfboards");

            using FeedIterator<Product> feed = container.GetItemQueryIterator<Product>(
                queryDefinition: query
            );

            await writeOutputAync($"Ran query:\t{query.QueryText}");

            List<Product> items = new();
            double requestCharge = 0d;
            while (feed.HasMoreResults)
            {
                FeedResponse<Product> response = await feed.ReadNextAsync();
                foreach (Product item in response)
                {
                    items.Add(item);
                }
                requestCharge += response.RequestCharge;
            }

            foreach (var item in items)
            {
                await writeOutputAync($"Found item:\t{item.name}\t[{item.id}]");
            }
            await writeOutputAync($"Request charge:\t{requestCharge:0.00}");
        }
    }
}
