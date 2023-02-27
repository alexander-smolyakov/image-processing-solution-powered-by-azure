// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using ImageProcessing.Infrastructure.Services;
using Microsoft.Azure.Cosmos;

static async Task<CosmosDbService> InitializeCosmosClientInstanceAsync(IConfigurationSection configurationSection)
{
    var cosmosClientOptions = new CosmosClientOptions()
    {
        HttpClientFactory = () =>
        {
            HttpMessageHandler httpMessageHandler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (req, cert, chain, errors) => true
            };
            return new HttpClient(httpMessageHandler);
        },
        ConnectionMode = ConnectionMode.Gateway
    };

    var databaseName = configurationSection["DatabaseName"];
    var containerName = configurationSection["ContainerName"];
    var account = configurationSection["Account"];
    var key = configurationSection["Key"];
    var client = new CosmosClient(account, key, cosmosClientOptions);
    var database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
    await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");
    var cosmosDbService = new CosmosDbService(client, databaseName, containerName);

    return cosmosDbService;
}

static BlobStorageService InitializeBlobStorageClientInstance(IConfigurationSection configurationSection)
{
    var connectionString = configurationSection["ConnectionString"];

    var blobStorageService = new BlobStorageService(
        connectionString: connectionString,
        blobContainerName: "image-storage"
    );

    return blobStorageService;
}

var builder = WebApplication.CreateBuilder(args);

// Set config files
builder.Configuration.AddJsonFile(
        path: "appsettings.json",
        optional: false,
        reloadOnChange: true);
builder.Configuration.AddJsonFile(
    path: $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
    optional: true,
    reloadOnChange: true);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureSwaggerGen(setup =>
{
    setup.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Image Processing Service",
        Version = "v1"
    });
});
builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddSingleton<ICosmosDbService>
(
    InitializeCosmosClientInstanceAsync
    (
        builder.Configuration.GetSection("CosmosDb")
    ).GetAwaiter().GetResult()
);

builder.Services.AddSingleton<IBlobStorageService>
(
    InitializeBlobStorageClientInstance
    (
        builder.Configuration.GetSection("AzureStorage")
    )
);

var app = builder.Build();
try
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Logger.LogInformation("Starting up service");
    app.Run();
    app.Logger.LogInformation("Shutting down service");
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "Host terminated unexpectedly");
}
