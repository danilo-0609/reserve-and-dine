using BuildingBlocks.Domain.Rules;
using Dinners.Domain.Restaurants.Errors;
using ErrorOr;

namespace Dinners.Domain.Restaurants.Rules;

internal sealed class TableMustNotBeOccuppiedToAssistRule : IBusinessRule
{
    private readonly bool _isOccuppied;

    public TableMustNotBeOccuppiedToAssistRule(bool isOccuppied)
    {
        _isOccuppied = isOccuppied;
    }

    public Error Error => RestaurantErrorCodes.TableIsNotFree;

    public bool IsBroken() => _isOccuppied;

    public static string Message => "Table must not be occuppied for assiting";
}
