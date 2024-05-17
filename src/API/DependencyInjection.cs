using API.Configuration;
using Azure.Storage.Blobs;
using BuildingBlocks.Application;
using Carter;
using MassTransit;
using API.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;
using Users.Infrastructure.Authentication;
using API.Modules.Users.Policies.Dinners.Menus;
using API.Modules.Users.Policies.Dinners;
using Microsoft.AspNetCore.Authorization;
using API.Modules.Users.Policies.Dinners.Menus.Publish;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

namespace API;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCarter();

        services.AddScoped(serviceProvider => new BlobServiceClient(configuration.GetConnectionString("AzureBlobStorage")));

        services.AddSingleton<HttpContextAccessor>();
        services.AddHttpContextAccessor();

        var jwtOptions = services.BuildServiceProvider().GetRequiredService<IOptions<JwtOptions>>();

        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(opt =>
        {
            opt.IncludeErrorDetails = true;

            opt.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Value.Issuer,
                ValidAudience = jwtOptions.Value.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtOptions.Value.SecretKey))
            };

            opt.Events = new JwtBearerEvents
            {
                OnMessageReceived = ctx =>
                {
                    ctx.Request.Cookies.TryGetValue("accessToken", out var accesToken);

                    if (!string.IsNullOrEmpty(accesToken))
                        ctx.Token = accesToken;

                    return Task.CompletedTask;
                }
            };
        });

        services.AddTransient<IAuthorizationRequirement, CanUpdateOrDeleteMenuRequirement>();
        services.AddTransient<IAuthorizationRequirement, CanPublishAMenuRequirement>();

        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policy.CanDeleteOrUpdateMenu, x =>
            {
                x.AddRequirements(new CanUpdateOrDeleteMenuRequirement());
            });
        });

        //Exception handlers
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        services.AddScoped<IExecutionContextAccessor, ExecutionContextAccessor>();

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();

            busConfigurator.UsingInMemory((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
