using Dinners.Domain.Menus;
using Dinners.Domain.Menus.MenuReviews;
using Dinners.Domain.Reservations;
using Dinners.Domain.Reservations.Refunds;
using Dinners.Domain.Reservations.ReservationsPayments;
using Dinners.Domain.Restaurants.RestaurantRatings;
using Domain.Restaurants;
using Microsoft.EntityFrameworkCore;

namespace Dinners.Infrastructure;

public interface IApplicationDbContext
{
    DbSet<Menu> Menus { get; }

    DbSet<MenuReview> Reviews { get; }

    DbSet<Reservation> Reservations { get; }

    DbSet<ReservationPayment> ReservationPayments { get; }

    DbSet<Refund> Refunds { get; }

    DbSet<Restaurant> Restaurants { get; }

    DbSet<RestaurantRating> Ratings { get; }  
}
