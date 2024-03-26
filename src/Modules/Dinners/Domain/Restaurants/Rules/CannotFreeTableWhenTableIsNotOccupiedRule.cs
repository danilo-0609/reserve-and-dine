using BuildingBlocks.Domain.Rules;
using Dinners.Domain.Restaurants.Errors;
using ErrorOr;

namespace Dinners.Domain.Restaurants.Rules;

internal sealed class CannotFreeTableWhenTableIsNotOccupiedRule : IBusinessRule
{
    private readonly bool _isOccupied;

    public CannotFreeTableWhenTableIsNotOccupiedRule(bool isOccupied)
    {
        _isOccupied = isOccupied;
    }

    public Error Error => RestaurantErrorCodes.CannotFreeTableWhenTableIsNotOccupied;

    public bool IsBroken() => !_isOccupied;

    public static string Message => "Cannot free table when table is not occupied";
}
