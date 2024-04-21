using API.Configuration;
using Azure.Storage.Blobs;
using BuildingBlocks.Application;
using Carter;
using MassTransit;
using Polly.Retry;
using Polly;
using API.Exceptions;

namespace API;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, string azureBlobStorageConnectionString)
    {
        services.AddCarter();

        services.AddSingleton(x =>
        {
            var blobService = new BlobServiceClient(azureBlobStorageConnectionString);

            var retryOptions = new RetryStrategyOptions<BlobServiceClient>
            {
                ShouldHandle = new PredicateBuilder<BlobServiceClient>().Handle<Exception>(),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                MaxRetryAttempts = 4,
                DelayGenerator = static args =>
                {
                    var delay = args.AttemptNumber switch
                    {
                        0 => TimeSpan.Zero,
                        1 => TimeSpan.FromSeconds(1),
                        _ => TimeSpan.FromSeconds(5)
                    };

                    return new ValueTask<TimeSpan?>(delay);
                }
            };

            new ResiliencePipelineBuilder<BlobServiceClient>().AddRetry(retryOptions);

            return blobService;
        });

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
