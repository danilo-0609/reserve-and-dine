using Dinners.Domain.Common;

namespace Dinners.Application.Restaurants;

public sealed record RestaurantTableResponse(int Number,
    int Seats,
    bool IsPremium,
    bool IsOccuppied,
    IReadOnlyDictionary<DateTime, TimeRange> ReservedHours);
