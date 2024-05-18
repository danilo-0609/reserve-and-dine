using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Users.Application.Services;
using FluentValidation;

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


        return services;
    }
}
