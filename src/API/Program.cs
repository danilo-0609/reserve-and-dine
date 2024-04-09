using API;
using API.Configuration;
using API.Modules.Dinners.Startup;
using Carter;

var builder = WebApplication.CreateBuilder(args);

//Presentation services
builder.Services.AddPresentation(await ConnectionsManager.GetAzureBlobStorageConnectionString());

//Modules services
builder.Services.AddDinners(await ConnectionsManager.GetDatabaseConnectionString(), 
    await ConnectionsManager.GetAzureRedisConnectionString());

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
