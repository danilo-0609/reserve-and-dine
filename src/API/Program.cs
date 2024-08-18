using API;
using API.OptionsSetup;
using Carter;
using API.Connections;
using API.Modules.Dinners;
using API.Modules.Users;
using Microsoft.AspNetCore.Antiforgery;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureOptions<JwtOptionsSetup>();

builder.Services.AddUsers(builder.Configuration);

//Presentation services

builder.Services.AddPresentation(builder.Configuration);

//Modules services
builder.Services.AddDinners(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var allowFrontendApp = "frontend_application";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowFrontendApp,
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
});

builder.Services.AddControllers();

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "XSRF-TOKEN"; 
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("antiforgery/token", (IAntiforgery forgeryService, HttpContext context) =>
{
    var tokens = forgeryService.GetAndStoreTokens(context);
    context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken!,
        new CookieOptions { HttpOnly = false });

    return Results.Ok();
})
    .RequireAuthorization();

app.UseCors(allowFrontendApp);

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseAntiforgery();

app.MapCarter();

app.MapControllers();

app.Run();

public partial class Program() { }
