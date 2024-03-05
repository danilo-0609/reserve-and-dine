namespace Dinners.Application.Menus;

public sealed record MenuResponse(
    Guid MenuId,
    Guid RestaurantId,
    MenuDetailsResponse MenuDetails,
    DishSpecificationResponse DishSpecification,
    MenuScheduleResponse MenuSchedule,
    DateTime CreatedOn,
    DateTime? UpdatedOn);
