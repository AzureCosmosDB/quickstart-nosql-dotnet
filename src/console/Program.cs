using Azure.Identity;
using Microsoft.Azure.Cosmos;

string endpoint = "";

var credential = new DefaultAzureCredential();

using CosmosClient client = new(endpoint, credential);

Database database = client.GetDatabase("cosmicworks");
Container container = database.GetContainer("products");

Item newItem = new("0000", "Test item", "Test SKU");

await container.UpsertItemAsync<Item>(newItem);

record Item(string id, string name, string sku);