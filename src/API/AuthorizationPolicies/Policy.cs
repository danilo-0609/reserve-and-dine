namespace API.AuthorizationPolicies;

public sealed record Policy
{
    public static string CanDeleteOrUpdateMenu => nameof(CanDeleteOrUpdateMenu);
    public static string CanPublishMenu => nameof(CanPublishMenu);
    public static string CanAccessToReservation => nameof(CanAccessToReservation);
    public static string CanGetReservation => nameof(CanGetReservation);
}
