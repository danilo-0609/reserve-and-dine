namespace Dinners.Application.Authorization;

public sealed record Policy
{
    public static string CanUserDeleteOrUpdateMenu => nameof(CanUserDeleteOrUpdateMenu);
    public static string CanUserPublishMenu => nameof(CanUserPublishMenu);
    public static string UserMustHaveConsumedTheMenuToReview => nameof(UserMustHaveConsumedTheMenuToReview);
    public static string UserCanOnlyUpdateTheirReviews => nameof(UserCanOnlyUpdateTheirReviews);
    public static string CanAccessToReservation => nameof(CanAccessToReservation);
    public static string CanGetReservation => nameof(CanGetReservation);
    public static string CanModifyRestaurantProperties => nameof(CanModifyRestaurantProperties);
    public static string UserCanRateRestaurant => nameof(UserCanRateRestaurant);
    public static string CanDeleteOrUpdateRate => nameof(CanDeleteOrUpdateRate);
    public static string CanRateRestaurantWhenHasVisitedIt => nameof(CanRateRestaurantWhenHasVisitedIt);
}
