using API.Configuration;
using Azure.Storage.Blobs;
using BuildingBlocks.Application;
using Carter;
using MassTransit;

namespace API;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, string azureBlobStorageConnectionString)
    {
        services.AddCarter();

        services.AddSingleton(x => new BlobServiceClient(azureBlobStorageConnectionString));

        services.AddSingleton<HttpContextAccessor>();
        services.AddHttpContextAccessor();

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
