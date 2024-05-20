using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Users.Application.Services;
using FluentValidation;
using Microsoft.Extensions.Configuration;

namespace Users.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        //MediatR servicess
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblyContaining<ApplicationAssemblyReference>();
        });

        //Logging services
        services.AddScoped(
            typeof(IPipelineBehavior<,>),
            typeof(UsersLoggingBehavior<,>));

        //Validator services
        services.AddScoped(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehavior<,>));

        //Unit of work services
        services.AddScoped(
            typeof(IPipelineBehavior<,>),
            typeof(UnitOfWorkBehavior<,>));

        services.AddValidatorsFromAssembly(ApplicationAssemblyReference.Assembly, includeInternalTypes: true);

        services.AddSingleton(serviceProvider =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            var connectionString = configuration.GetConnectionString("DockerSqlDatabase");

            return new SqlConnectionFactory(connectionString!);
        });

        return services;
    }
}
