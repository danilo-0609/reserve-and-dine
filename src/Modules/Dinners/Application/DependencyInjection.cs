using Application.Services;
using Dinners.Application.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Dinners.Application;

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
            typeof(DinnersLoggingBehavior<,>));

        //Validator services
        services.AddScoped(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehavior<,>));

        //Unit of work services
        services.AddScoped(
            typeof(IPipelineBehavior<,>),
            typeof(UnitOfWorkBehavior<,>));

        services.AddValidatorsFromAssembly(ApplicationAssemblyReference.Assembly, includeInternalTypes: true);

        return services;
    }
}
