using API;
using API.Modules.Dinners.Startup;
using Carter;
using Dinners.Infrastructure.Connections;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IConnectionsManager, ConnectionsManager>();
//Presentation services
var blobStorageConnectionString = await new ConnectionsManager().GetAzureBlobStorageConnectionString();

builder.Services.AddPresentation(blobStorageConnectionString);


//Modules services
var connectionsManager = new ConnectionsManager();

var redisConnectionString = await connectionsManager.GetAzureRedisConnectionString();
var dockerDatabaseConnectionString = await connectionsManager.GetDockerDatabaseConnectionString();
var databaseConnectionString = await connectionsManager.GetDatabaseConnectionString();

builder.Services.AddDinners(redisConnectionString, databaseConnectionString, dockerDatabaseConnectionString);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapCarter();

app.Run();

public partial class Program() { }
