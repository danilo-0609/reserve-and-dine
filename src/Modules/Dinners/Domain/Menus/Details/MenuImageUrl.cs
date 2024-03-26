namespace Dinners.Domain.Menus.Details;

public sealed record MenuImageUrl
{
    public string Value { get; private set; }

    public MenuImageUrl(string value)
    {
        Value = value;
    }

    private MenuImageUrl() { }
}
