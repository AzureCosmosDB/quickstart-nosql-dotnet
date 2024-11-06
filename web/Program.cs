using Azure.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Microsoft.Samples.Cosmos.NoSQL.Quickstart.Services;
using Microsoft.Samples.Cosmos.NoSQL.Quickstart.Services.Interfaces;
using Microsoft.Samples.Cosmos.NoSQL.Quickstart.Web.Components;

using Settings = Microsoft.Samples.Cosmos.NoSQL.Quickstart.Models.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();

builder.Services.AddOptions<Settings.Configuration>().Bind(builder.Configuration.GetSection(nameof(Settings.Configuration)));

builder.Services.AddSingleton<CosmosClient>((serviceProvider) =>
{
    IOptions<Settings.Configuration> configurationOptions = serviceProvider.GetRequiredService<IOptions<Settings.Configuration>>();
    Settings.Configuration configuration = configurationOptions.Value;

    CosmosClient client = new(
        connectionString: "<azure-cosmos-db-nosql-connection-string>"
    );
    return client;
});

builder.Services.AddTransient<IDemoService, DemoService>();

var app = builder.Build();

app.UseDeveloperExceptionPage();

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
