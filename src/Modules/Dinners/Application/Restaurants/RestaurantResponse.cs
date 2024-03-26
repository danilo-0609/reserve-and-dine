namespace Dinners.Application.Restaurants;

public sealed record RestaurantResponse(Guid RestaurantId,
    string AvailableTablesStatus,
    RestaurantInformationResponse RestaurantInformation,
    RestaurantLocalizationResponse RestaurantLocalization,
    string ScheduleStatus,
    List<RestaurantScheduleResponse> RestaurantSchedule,
    RestaurantContactResponse RestaurantContact,
    List<RestaurantClientResponse> RestaurantClients,
    List<RestaurantTableResponse> RestaurantTables,
    DateTime PostedAt);
