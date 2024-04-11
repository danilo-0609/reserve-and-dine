namespace Dinners.Application.Menus;

public sealed record MenuResponse(
    Guid MenuId,
    Guid RestaurantId,
    MenuDetailsResponse MenuDetails,
    DishSpecificationResponse DishSpecification,
    List<MenuScheduleResponse> MenuSchedules,
    List<Guid> MenuImagesUrlId,
    DateTime CreatedOn,
    DateTime? UpdatedOn);
