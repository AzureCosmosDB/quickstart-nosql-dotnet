using Azure.Identity;
using Microsoft.Azure.Cosmos;

string endpoint = "";

var credential = new DefaultAzureCredential();

using CosmosClient client = new(endpoint, credential);

Database database = client.GetDatabase("cosmicworks");
Container container = database.GetContainer("products");

Item item1 = new("0001", "Test item", "Test SKU");
Item item2 = new("0002", "Another item", "Test SKU");

await container.UpsertItemAsync<Item>(item1);
await container.UpsertItemAsync<Item>(item2);

var iterator = container.GetItemQueryIterator<Item>("SELECT * FROM c");

while(iterator.HasMoreResults)
{
    foreach(var item in await iterator.ReadNextAsync())
    {
        Console.WriteLine(item);
    }
}

record Item(string id, string name, string sku);