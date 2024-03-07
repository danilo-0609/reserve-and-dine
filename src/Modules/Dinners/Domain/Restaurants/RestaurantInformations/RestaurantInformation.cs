namespace Dinners.Domain.Restaurants.RestaurantInformations;

public sealed record RestaurantInformation
{
    private readonly List<string>? _chefs = new();
    private readonly List<string>? _specialties = new();
    private readonly List<string>? _imagesUrl = new();

    public string Title { get; private set; }

    public string Description { get; private set; }

    public string Type { get; private set; }

    public IReadOnlyList<string>? Chefs => _chefs.AsReadOnly();

    public IReadOnlyList<string>? Specialties => _specialties.AsReadOnly();

    public IReadOnlyList<string>? RestaurantImagesUrl => _imagesUrl.AsReadOnly();

    public static RestaurantInformation Create(
        string title,
        string description,
        string type,
        List<string>? chefs,
        List<string>? specialties,
        List<string>? imagesUrl)
    {
        return new RestaurantInformation(title,
            description,
            type,
            chefs,
            specialties,
            imagesUrl);
    }


    private RestaurantInformation(string title,
        string description,
        string type,
        List<string>? chefs,
        List<string>? specialties,
        List<string>? imagesUrl)
    {
        Title = title;
        Description = description;
        Type = type;

        _chefs = chefs;
        _specialties = specialties;
        _imagesUrl = imagesUrl;
    }

    private RestaurantInformation() { }
}
