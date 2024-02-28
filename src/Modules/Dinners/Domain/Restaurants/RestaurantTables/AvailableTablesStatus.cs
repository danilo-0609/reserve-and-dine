namespace Dinners.Domain.Restaurants.RestaurantTables;

public sealed record AvailableTablesStatus
{
    public string Value { get; private set; }

    public static AvailableTablesStatus NoAvailables => new AvailableTablesStatus(nameof(NoAvailables));

    public static AvailableTablesStatus Few => new AvailableTablesStatus(nameof(Few));

    public static AvailableTablesStatus Availables => new AvailableTablesStatus(nameof(Availables));

    public AvailableTablesStatus(string value)
    {
        Value = value;
    }
}
