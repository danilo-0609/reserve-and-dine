using API.Configuration;
using Azure.Storage.Blobs;
using BuildingBlocks.Application;
using Carter;
using MassTransit;
using API.Exceptions;

namespace API;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCarter();

        services.AddScoped(serviceProvider => new BlobServiceClient(configuration.GetConnectionString("AzureBlobStorage")));

        services.AddSingleton<HttpContextAccessor>();
        services.AddHttpContextAccessor();

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
