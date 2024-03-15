using Dinners.Domain.Restaurants.Rules;
using ErrorOr;

namespace Dinners.Domain.Restaurants.Errors;

public static class RestaurantErrorCodes
{
    public static Error NotFound =>
        Error.Validation("Restaurant.NotFound", "Restaurant was not found");

    public static Error ImagesNotFound =>
        Error.NotFound("Restaurant.ImagesNotFound", "Restaurant images were not found");

    public static Error CannotDeleteRestaurant =>
        Error.Unauthorized("Restaurant.CannotDeleteRestaurant", "Cannot delete restaurant if you are not a restaurant administrator");

    public static Error CannotAccessToAdministrationContent =>
        Error.Unauthorized("Restaurant.CannotAccessToAdministrationContent", "Cannot access to administration content if user is not a restaurant administrator");

    public static Error CannotChangeRestaurantProperties =>
        Error.Unauthorized("Restaurant.CannotChangeRestaurantScheduleStatus", CannotChangeRestaurantPropertiesWhenUserIsNotAdministratorRule.Message);

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

    public static Error EqualAvailableTableStatus =>
        Error.Validation("Restaurant.EqualAvailableTableStatus", "The current available table status is equal than the status requested. Cannot change.");

    public static Error CannotReserveWhenRestaurantIsClosed =>
        Error.Validation("Restaurant.CannotReserveWhenScheduleStatusIsClosed", TableCannotReserveWhenRestaurantIsClosedRule.Message);

    public static Error TableIsNotFree =>
        Error.Validation("Restaurant.TableIsNotFree", TableMustNotBeOccuppiedToAssistRule.Message);

    public static Error RateWhenUserIsAdministrator =>
        Error.Validation("Restaurant.RateWhenUserIsAdministrator", CannotRateWhenUserIsAdministratorRule.Message);
}
