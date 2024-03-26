using Dinners.Domain.Common;

namespace Dinners.Domain.Menus.Details;

public sealed record MenuDetails
{
    public string Title { get; private set; }

    public string Description { get; private set; }

    public MenuType MenuType { get; private set; }

    public string DiscountTerms { get; private set; } = string.Empty;

    public Price Price { get; private set; }

    public decimal Discount { get; private set; }

    public List<MenuImageUrl> MenuImagesUrl { get; private set; }

    public List<Tag> Tags { get; private set; }

    public bool IsVegetarian { get; private set; }

    public string PrimaryChefName { get; private set; }

    public bool HasAlcohol { get; private set; }


    public static MenuDetails Create(string title,
        string description,
        MenuType menuType,
        Price price,
        decimal discount,
        List<string> menuImagesUrl,
        List<string> tags,
        bool isVegetarian,
        string primaryChefName,
        bool hasAlcohol,
        string discountTerms = "")
    {
        return new MenuDetails(title,
            description,
            menuType,
            price,
            discount,
            menuImagesUrl.ConvertAll(url => new MenuImageUrl(url)),
            tags.ConvertAll(value => new Tag(value)),
            isVegetarian,
            primaryChefName,
            hasAlcohol,
            discountTerms);
    }

    public void AddImage(string imageUrl)
    {
        MenuImagesUrl.Add(new MenuImageUrl(imageUrl));
    }

    public void DeleteImage(string imageUrl)
    {
        MenuImagesUrl.Remove(new MenuImageUrl(imageUrl));
    }

    private MenuDetails(string title,
        string description,
        MenuType menuType,
        Price price,
        decimal discount,
        List<MenuImageUrl> menuImagesUrl,
        List<Tag> tags,
        bool isVegetarian,
        string primaryChefName,
        bool hasAlcohol,
        string discountTerms = "")
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
        Price = price;
    }

    private MenuDetails() { }
}
