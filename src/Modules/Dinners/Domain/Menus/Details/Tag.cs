namespace Dinners.Domain.Menus.Details;

public sealed record Tag
{
    public string Value { get; private set; }

    public Tag(string value)
    {
        Value = value;
    }

    private Tag() { }
}
