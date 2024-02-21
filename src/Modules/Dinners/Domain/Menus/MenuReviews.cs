namespace Dinners.Domain.Menus;

public sealed record MenuReviews
{
    public decimal Rate {  get; private set; }    
    
    public string Comment { get; private set; } = string.Empty;
}

