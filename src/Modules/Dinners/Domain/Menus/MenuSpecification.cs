namespace Dinners.Domain.Menus;

public sealed record MenuSpecification
{
    public string Title { get; private set; }

    public string Description { get; private set; }

    public MenuType MenuType { get; private set; }

    public string DiscountTerms { get; private set; } = string.Empty;

    public decimal Discount {  get; private set; }

    public List<string> MenuImagesUrl { get; private set; }

    public List<string> Tags { get; private set; }

    public bool IsVegetarian { get; private set; }

    public string PrimaryChefName { get; private set; }

    public bool HasAlcohol { get; private set; }

    public MenuSpecification(string title, 
        string description, 
        MenuType menuType, 
        string discountTerms, 
        decimal discount, 
        List<string> menuImagesUrl, 
        List<string> tags, 
        bool isVegetarian, 
        string primaryChefName, 
        bool hasAlcohol)
    {
        Title = title;
        Description = description;
        MenuType = menuType;
        DiscountTerms = discountTerms;
        Discount = discount;
        MenuImagesUrl = menuImagesUrl;
        Tags = tags;
        IsVegetarian = isVegetarian;
        PrimaryChefName = primaryChefName;
        HasAlcohol = hasAlcohol;
    }
}
