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
using Dinners.Infrastructure.Domain.Menus;
using Dinners.Infrastructure.Domain.Menus.Reviews;
using Dinners.Infrastructure.Domain.Reservations;
using Dinners.Infrastructure.Domain.Reservations.Payments;
using Dinners.Infrastructure.Domain.Reservations.Refunds;
using Dinners.Infrastructure.Domain.Restaurants;
using Dinners.Infrastructure.Domain.Restaurants.Ratings;
using Dinners.Infrastructure.EventsBus;
using Dinners.Infrastructure.Jobs.Setups;
using Dinners.Infrastructure.Outbox.BackgroundJobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Dinners.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string databaseConnectionString)
    {

        services.AddMemoryCache();

        services.AddQuartzHostedService();
        services.AddQuartz();
        services.ConfigureOptions<ProcessDinnersOutboxMessagesJobSetup>();
        services.ConfigureOptions<CancelNotAsistedReservationsJobSetup>();
        services.ConfigureOptions<CancelNotPaidReservationsJobSetup>();


        services.AddDbContext<DinnersDbContext>((_, optionsBuilder) =>
        {
            optionsBuilder.UseSqlServer(databaseConnectionString);
        });

        services.AddScoped<IApplicationDbContext>(sp =>
            sp.GetRequiredService<DinnersDbContext>());

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IMenuBlobService, MenuBlobService>();
        services.AddScoped<IRestaurantBlobService, RestaurantBlobService>();

        services.AddTransient<IEventBus, EventBus>();

        services.AddScoped<IMenuRepository, MenuRepository>();
        services.Decorate<IMenuRepository, CacheMenuRepository>();

        services.AddScoped<IMenuReviewRepository, ReviewRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IReservationPaymentRepository, PaymentRepository>();
        services.AddScoped<IRefundRepository, RefundRepository>();
        services.AddScoped<IRestaurantRatingRepository, RatingRepository>();
        services.AddScoped<IRestaurantRepository, RestaurantRepository>();


        return services;
    }
}
