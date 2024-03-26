using BuildingBlocks.Domain.Rules;
using Dinners.Domain.Restaurants.Errors;
using ErrorOr;

namespace Dinners.Domain.Restaurants.Rules;

internal sealed class TableMustNotBeOccupiedToAssistRule : IBusinessRule
{
    private readonly bool _isOccupied;

    public TableMustNotBeOccupiedToAssistRule(bool isOccupied)
    {
        _isOccupied = isOccupied;
    }

    public Error Error => RestaurantErrorCodes.TableIsNotFree;

    public bool IsBroken() => _isOccupied;

    public static string Message => "Table must not be occupied for assiting";
}
