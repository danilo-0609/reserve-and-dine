using Dinners.Application.Common;
using Dinners.Domain.Menus;
using Dinners.Domain.Menus.MenuReviews;
using Dinners.Domain.Reservations;
using Dinners.Domain.Reservations.Refunds;
using Dinners.Domain.Reservations.ReservationsPayments;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.RestaurantRatings;
using Dinners.Infrastructure.Blobs;
using Dinners.Infrastructure.Cache.Menus;
using Dinners.Infrastructure.Cache.Reservations;
using Dinners.Infrastructure.Cache.Restaurants;
using Dinners.Infrastructure.Domain.Menus;
using Dinners.Infrastructure.Domain.Menus.MenuReviews;
using Dinners.Infrastructure.Domain.Menus.Reviews;
using Dinners.Infrastructure.Domain.Reservations;
using Dinners.Infrastructure.Domain.Reservations.Payments;
using Dinners.Infrastructure.Domain.Reservations.Refunds;
using Dinners.Infrastructure.Domain.Restaurants;
using Dinners.Infrastructure.Domain.Restaurants.Ratings;
using Dinners.Infrastructure.EventsBus;
using Dinners.Infrastructure.Jobs.Setups;
using Dinners.Infrastructure.Outbox.BackgroundJobs;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Quartz;

namespace Dinners.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string redisConnectionString, string databaseConnectionString, string dockerDatabaseConnectionString)
    {
        services.AddMemoryCache();
        services.AddStackExchangeRedisCache(redisOptions =>
        {
            redisOptions.Configuration = redisConnectionString;
        });

        services.AddDbContext<DinnersDbContext>(async (sp, optionsBuilder) =>
        {
            var connectionString = await IsAzureDatabaseAvailable(databaseConnectionString) 
                ? databaseConnectionString : dockerDatabaseConnectionString;

            optionsBuilder.UseSqlServer(connectionString);
        });

        services.AddQuartzHostedService();
        services.AddQuartz();
        services.ConfigureOptions<ProcessDinnersOutboxMessagesJobSetup>();
        services.ConfigureOptions<CancelNotAsistedReservationsJobSetup>();
        services.ConfigureOptions<CancelNotPaidReservationsJobSetup>();
        services.ConfigureOptions<CancelReservationsAfterRestaurantWasClosedOutOfScheduleJobSetup>();

        services.AddScoped<IApplicationDbContext>(sp =>
            sp.GetRequiredService<DinnersDbContext>());

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IMenuBlobService, MenuBlobService>();
        services.AddScoped<IRestaurantBlobService, RestaurantBlobService>();

        services.AddTransient<IEventBus, EventBus>();

        services.AddScoped<IMenuRepository, MenuRepository>();
        services.Decorate<IMenuRepository, CacheMenuRepository>();

        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.Decorate<IReservationRepository, CacheReservationRepository>();

        services.AddScoped<IReservationPaymentRepository, PaymentRepository>();
        services.Decorate<IReservationPaymentRepository, CachePaymentRepository>();

        services.AddScoped<IRefundRepository, RefundRepository>();
        services.Decorate<IRefundRepository, CacheRefundRepository>();

        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.Decorate<IReviewRepository, CacheReviewRepository>();

        services.AddScoped<IMenusReviewsRepository, MenusReviewsRepository>();
        

        services.AddScoped<IRestaurantRatingRepository, RatingRepository>();
        services.Decorate<IRestaurantRatingRepository, CacheRatingRepository>();    

        services.AddScoped<IRestaurantRepository, RestaurantRepository>();
        services.Decorate<IRestaurantRepository, CacheRestaurantRepository>();

        return services;
    }

    private async static Task<bool> IsAzureDatabaseAvailable(string connectionString)
    {
        var retryPolicy = Policy
             .Handle<SqlException>()
             .WaitAndRetryAsync(
                 3,
                 retryAttempt => TimeSpan.FromSeconds(5),
                 onRetry: (exception, timeSpan, retryCount, context) =>
                 {
                     Console.WriteLine($"Connection lost, retry attempt {retryCount} at {DateTime.Now}. " +
                         $"Exception Message: {exception.Message}");
                 });

        bool isAvailable = false;

        await retryPolicy.ExecuteAsync(async () =>
        {
            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    isAvailable = true;
                }
                catch (Exception)
                {
                    isAvailable = false;
                }                   
            }

            return isAvailable;
        });

        return isAvailable;
    }
}
