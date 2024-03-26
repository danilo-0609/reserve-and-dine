using Dinners.Domain.Restaurants.RestaurantTables;

namespace Dinners.Application.Restaurants;

public sealed record RestaurantTableResponse(int Number,
    int Seats,
    bool IsPremium,
    bool IsOccupied,
    IReadOnlyList<ReservedHour> ReservedHours);
