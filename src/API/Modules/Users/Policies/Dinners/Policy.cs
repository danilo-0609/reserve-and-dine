namespace API.Modules.Users.Policies.Dinners;

public record Policy
{ 
    public static string CanDeleteOrUpdateMenu => nameof(CanDeleteOrUpdateMenu);
    public static string CanPublishMenu => nameof(CanPublishMenu);
}
