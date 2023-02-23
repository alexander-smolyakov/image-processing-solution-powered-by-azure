// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using ImageProcessing.Infrastructure.Services;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Azure;
var builder = WebApplication.CreateBuilder(args);

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

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddSingleton<ICosmosDbService>(
    InitializeCosmosClientInstanceAsync(
        builder.Configuration.GetSection("CosmosDb")
    ).GetAwaiter().GetResult()
);

builder.Services.AddAzureClients(
    clientBuilder =>
    {
        var connectionString = builder.Configuration["AzureStorage:ConnectionString"];
        clientBuilder.AddBlobServiceClient(connectionString);
    }
);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v2");
    options.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
