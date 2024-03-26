using Dinners.Domain.Menus;
using Dinners.Domain.Menus.MenuReviews;
using Dinners.Domain.Reservations;
using Dinners.Domain.Reservations.Refunds;
using Dinners.Domain.Reservations.ReservationsPayments;
using Dinners.Domain.Restaurants.RestaurantRatings;
using Dinners.Infrastructure.Domain.Menus.MenuReviews;
using Dinners.Infrastructure.Domain.ReservationsMenus;
using Dinners.Infrastructure.Domain.Restaurants.RestaurantsRatings;
using Dinners.Infrastructure.Outbox;
using Domain.Restaurants;
using Microsoft.EntityFrameworkCore;

namespace Dinners.Infrastructure;

internal sealed class DinnersDbContext : DbContext, IApplicationDbContext
{

    public DinnersDbContext(DbContextOptions<DinnersDbContext> options) 
        : base(options)
    {
    }

    public DbSet<Menu> Menus { get; set; }

    public DbSet<MenusReviews> MenusReviews { get; set; }

    public DbSet<MenuReview> Reviews { get; set; }

    public DbSet<ReservationMenus> ReservationMenus { get; set; }

    public DbSet<Reservation> Reservations { get; set; }

    public DbSet<ReservationPayment> ReservationPayments { get; set; }

    public DbSet<Refund> Refunds { get; set; }

    public DbSet<Restaurant> Restaurants { get; set; }

    public DbSet<RestaurantRatings> RestaurantRatings { get; set; }

    public DbSet<RestaurantRating> Ratings { get; set; }

    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DinnersDbContext).Assembly);
    }

    public DinnersDbContext() { }
}
