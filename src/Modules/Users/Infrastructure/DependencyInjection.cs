﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Users.Application.Abstractions;
using Users.Application.Common;
using Users.Domain.UserRegistrations;
using Users.Domain.Users;
using Users.Infrastructure.Authentication;
using Users.Infrastructure.Blob;
using Users.Infrastructure.Domain.UserRegistrations;
using Users.Infrastructure.Domain.Users;
using Users.Infrastructure.Domain.Users.UsersCounter;
using Users.Infrastructure.Jobs.Setups;
using Users.Infrastructure.Outbox.Jobs;

namespace Users.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache();
        services.AddDistributedMemoryCache();

        services.AddDbContext<UsersDbContext>((sp, optionsBuilder) =>
        {
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DockerSqlDatabase"));

            optionsBuilder.EnableSensitiveDataLogging();
        });

        services.AddQuartzHostedService();
        services.AddQuartz();
        services.ConfigureOptions<ExpireNotConfirmedUserRegistrationsJobSetup>();
        services.ConfigureOptions<ProcessUsersOutboxMessagesJobSetup>();

        services.AddScoped<IUserBlobService, UserBlobService>();

        services.AddScoped<IUsersDbContext>(sp =>
            sp.GetRequiredService<UsersDbContext>());

        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddScoped<ITokenSetService, TokenSetService>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserRegistrationRepository, UserRegistrationRepository>();
        services.AddScoped<IUsersCounter, UsersCounter>();
        
        return services;
    }
}