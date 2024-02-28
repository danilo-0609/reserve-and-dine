using BuildingBlocks.Domain.Rules;
using Dinners.Domain.Restaurants.Errors;
using Dinners.Domain.Restaurants.RestaurantTables;
using ErrorOr;

namespace Dinners.Domain.Restaurants.Rules;

internal sealed class TableCannotBeReservedWhenTableIsReservedNowRule : IBusinessRule
{
    private readonly RestaurantTable _restaurantTable;

    public TableCannotBeReservedWhenTableIsReservedNowRule(RestaurantTable restaurantTable)
    {
        _restaurantTable = restaurantTable;
    }

    public Error Error => RestaurantErrorCodes.CannotReserveIfTableIsReserved;

    public bool IsBroken() => _restaurantTable.IsReserved == true;

    public static string Message => "Table cannot be reserved when table is reserved now";
}
