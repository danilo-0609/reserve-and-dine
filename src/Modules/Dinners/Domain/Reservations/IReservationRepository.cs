using Dinners.Domain.Restaurants;

namespace Dinners.Domain.Reservations;

public interface IReservationRepository
{
    Task AddAsync(Reservation reservation, CancellationToken cancellationToken);

    Task<Reservation?> GetByIdAsync(ReservationId reservationId, CancellationToken cancellationToken);

    Task DeleteAsync(ReservationId reservationId, CancellationToken cancellationToken);

    Task UpdateAsync(Reservation reservation, CancellationToken cancellationToken);

    Task<Reservation?> GetByRestaurantIdAndReservationTimeAsync(RestaurantId restaurantId, DateTime reservationDateTime, CancellationToken cancellation);
}
