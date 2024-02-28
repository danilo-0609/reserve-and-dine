using Dinners.Domain.Restaurants.Rules;
using ErrorOr;

namespace Dinners.Domain.Restaurants.Errors;

public static class RestaurantErrorCodes
{
    public static Error CannotChangeRestaurantScheduleStatus =>
        Error.Validation("Restaurant.CannotChangeRestaurantScheduleStatus", CannotChangeRestaurantScheduleStatusWhenUserIsNotAdministratorRule.Message);

    public static Error CannotCloseWhenRestaurantIsClosed =>
        Error.Validation("Restaurant.CannotCloseTheRestaurant", CannotCloseWhenRestaurantScheduleStatusIsClosedRule.Message);

    public static Error CannotOpenWhenRestaurantIsOpened =>
        Error.Validation("Restaurant.CannotOpenTheRestaurant", CannotOpenWhenRestaurantScheduleStatusIsOpenedRule.Message);

    public static Error TableDoesNotExist =>
        Error.Validation("Restaurant.TableDoesNotExist", "The table with the given number does not exist");

    public static Error CannotReserveIfTableIsReserved =>
        Error.Validation("Restaurant.CannotReserveIfTableIsReserved", TableCannotBeReservedWhenTableIsReservedNowRule.Message);

    public static Error EqualAvailableTableStatus =>
        Error.Validation("Restaurant.EqualAvailableTableStatus", "The current available table status is equal than the status requested. Cannot change.");
}
