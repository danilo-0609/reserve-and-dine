using API;
using API.OptionsSetup;
using Carter;
using API.Connections;
using API.Modules.Dinners;
using API.Modules.Users;

var builder = WebApplication.CreateBuilder(args);

var keyVaultUri = Environment.GetEnvironmentVariable("KEY_VAULT_URI");

builder.Configuration["ConnectionStrings:AzureKeyVaultUri"] = keyVaultUri;

var connectionsManager = new ConnectionsManager(builder.Configuration);
var databaseConnectionString = await connectionsManager.GetDatabaseConnectionString();
var redisConnectionString = await connectionsManager.GetAzureRedisConnectionString();
var dockerDatabaseConnectionString = await connectionsManager.GetDockerDatabaseConnectionString();
var azureBlobStorageConnectionString = await connectionsManager.GetAzureBlobStorageConnectionString();

var jwtIssuer = await connectionsManager.GetJWTIssuer();
var jwtAudience = await connectionsManager.GetJWTAudience();
var jwtSecretKey = await connectionsManager.GetJWTSecretKey();

builder.Configuration["JWT:Issuer"] = jwtIssuer;
builder.Configuration["JWT:Audience"] = jwtAudience;
builder.Configuration["JWT:SecretKey"] = jwtSecretKey;

builder.Configuration["ConnectionStrings:AzureSqlDatabase"] = databaseConnectionString;
builder.Configuration["ConnectionStrings:RedisConnectionString"] = redisConnectionString;
builder.Configuration["ConnectionStrings:DockerSqlDatabase"] = dockerDatabaseConnectionString;
builder.Configuration["ConnectionStrings:AzureBlobStorage"] = azureBlobStorageConnectionString;

builder.Services.AddSingleton<IConnectionsManager, ConnectionsManager>();
builder.Services.ConfigureOptions<JwtOptionsSetup>();

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

app.UseAuthentication();

app.UseAuthorization();

app.MapCarter();

app.Run();

public partial class Program() { }
