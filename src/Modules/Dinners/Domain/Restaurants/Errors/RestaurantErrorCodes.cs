using Dinners.Domain.Restaurants.Rules;
using ErrorOr;

namespace Dinners.Domain.Restaurants.Errors;

public static class RestaurantErrorCodes
{
    public static Error NotFound =>
        Error.NotFound("Restaurant.NotFound", "Restaurant was not found");

    public static Error ImageNotFound =>
        Error.NotFound("Restaurant.ImageNotFound", "The restaurant image was not found");

    public static Error CannotDeleteRestaurant =>
        Error.Unauthorized("Restaurant.CannotDeleteRestaurant", "Cannot delete restaurant if you are not a restaurant administrator");

    public static Error CannotAccessToAdministrationContent =>
        Error.Unauthorized("Restaurant.CannotAccessToAdministrationContent", "Cannot access to administration content if user is not a restaurant administrator");

    public static Error CannotChangeRestaurantProperties =>
        Error.Unauthorized("Restaurant.CannotChangeRestaurantProperties", CannotChangeRestaurantPropertiesWhenUserIsNotAdministratorRule.Message);

    public static Error CannotAddTableWithDuplicateNumber =>
        Error.Validation("Restaurant.CannotAddTableWithDuplicateNumber", "Cannot add table when there's another table with that number");

    public static Error CannotCloseWhenRestaurantIsClosed =>
        Error.Validation("Restaurant.CannotCloseTheRestaurant", CannotCloseWhenRestaurantScheduleStatusIsClosedRule.Message);

    public static Error CannotOpenWhenRestaurantIsOpened =>
        Error.Validation("Restaurant.CannotOpenTheRestaurant", CannotOpenWhenRestaurantScheduleStatusIsOpenedRule.Message);

    public static Error TableDoesNotExist =>
        Error.Validation("Restaurant.TableDoesNotExist", "The table with the given number does not exist");

    public static Error CannotReserveIfTableIsReserved =>
        Error.Validation("Restaurant.CannotReserveIfTableIsReserved", TableCannotBeReservedWhenTableIsReservedNowRule.Message);

    public static Error CannotBeReservedAtCertainTimeWhenTableIsReservedAtThatTime =>
        Error.Validation("Table.CannotBeReservedAtCertainTimeWhenTableIsReservedAtThatTime", TableCannotBeReservedAtCertainTimeWhenTableIsReservedAtThatTimeNowRule.Message);

    public static Error EqualAvailableTableStatus =>
        Error.Validation("Restaurant.EqualAvailableTableStatus", "The current available table status is equal than the status requested. Cannot change.");

    public static Error CannotReserveWhenTimeOfReservationIsOutOfSchedule =>
        Error.Validation("Restaurant.CannotReserveWhenTimeOfReservationIsOutOfSchedule", CannotReserveWhenTimeOfReservationIsOutOfScheduleRule.Message);

    public static Error CannotReserveWhenRestaurantHasClosedOutOfSchedule =>
        Error.Validation("Restaurant.CannotReserveWhenRestaurantHasClosedOutOfSchedule", CannotReserveWhenRestaurantHasClosedOutOfScheduleRule.Message);

    public static Error TableIsNotFree =>
        Error.Validation("Restaurant.TableIsNotFree", TableMustNotBeOccupiedToAssistRule.Message);

    public static Error CannotFreeTableWhenTableIsNotOccupied =>
        Error.Validation("Restaurant.CannotFreeTableWhenTableIsNotOccupied", CannotFreeTableWhenTableIsNotOccupiedRule.Message);

    public static Error RateWhenUserIsAdministrator =>
        Error.Validation("Restaurant.RateWhenUserIsAdministrator", CannotRateWhenUserIsAdministratorRule.Message);
}
