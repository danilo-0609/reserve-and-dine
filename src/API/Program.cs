using API;
using API.Modules.Dinners.Startup;
using API.Modules.Users.Configuration;
using API.Modules.Users.Entities;
using API.Modules.Users.Policies.Dinners;
using Carter;
using Dinners.Infrastructure.Connections;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);


var connectionsManager = new ConnectionsManager();
var databaseConnectionString = await connectionsManager.GetDatabaseConnectionString();
var redisConnectionString = await connectionsManager.GetAzureRedisConnectionString();
var dockerDatabaseConnectionString = await connectionsManager.GetDockerDatabaseConnectionString();
var azureBlobStorageConnectionString = await connectionsManager.GetAzureBlobStorageConnectionString();

builder.Configuration["ConnectionStrings:AzureSqlDatabase"] = databaseConnectionString;
builder.Configuration["ConnectionStrings:RedisConnectionString"] = redisConnectionString;
builder.Configuration["ConnectionStrings:DockerSqlDatabase"] = dockerDatabaseConnectionString;
builder.Configuration["ConnectionStrings:AzureBlobStorage"] = azureBlobStorageConnectionString;

builder.Services.AddSingleton<IConnectionsManager, ConnectionsManager>();
builder.Services.AddUsers(builder.Configuration);

//Presentation services

builder.Services.AddPresentation(builder.Configuration);

//Modules services
builder.Services.AddDinners(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapIdentityApi<User>();

app.UseAuthentication();

app.UseAuthorization();

app.MapCarter();

app.Run();

public partial class Program() { }
