using API;
using API.Configuration;
using API.Modules.Dinners.Startup;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Presentation service
builder.Services.AddPresentation(await ConnectionsManager.GetAzureBlobStorageConnectionString());

//Modules services
builder.Services.AddDinners(await ConnectionsManager.GetDatabaseConnectionString());


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
