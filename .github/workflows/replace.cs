CosmosClient client = new(
    connectionString: builder.Configuration["AZURE_COSMOS_DB_NOSQL_CONNECTION_STRING"]!
);