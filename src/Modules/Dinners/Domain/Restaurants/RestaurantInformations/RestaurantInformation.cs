namespace Dinners.Domain.Restaurants.RestaurantInformations;

public sealed record RestaurantInformation
{
    private readonly List<Chef> _chefs = new();
    private readonly List<Speciality> _specialties = new();
    private readonly List<RestaurantImageUrl> _imagesUrl = new();

    public string Title { get; private set; }

    public string Description { get; private set; }

    public string Type { get; private set; }

    public IReadOnlyList<Chef> Chefs => _chefs.AsReadOnly();

    public IReadOnlyList<Speciality> Specialties => _specialties.AsReadOnly();

    public IReadOnlyList<RestaurantImageUrl> RestaurantImagesUrl => _imagesUrl.AsReadOnly();

    public static RestaurantInformation Create(
        string title,
        string description,
        string type,
        List<string> chefs,
        List<string> specialties,
        List<string> imagesUrl)
    {
        return new RestaurantInformation(title,
            description,
            type,
            chefs.ConvertAll(value => new Chef(value)),
            specialties.ConvertAll(value => new Speciality(value)),
            imagesUrl.ConvertAll(url => new RestaurantImageUrl(url)));
    }

    public void AddImage(string imageUrl)
    {
        _imagesUrl.Add(new RestaurantImageUrl(imageUrl));
    }

    public void RemoveImage(string imageUrl)
    {
        _imagesUrl.Remove(new RestaurantImageUrl(imageUrl));
    }

    private RestaurantInformation(string title,
        string description,
        string type,    
        List<Chef> chefs,
        List<Speciality> specialties,
        List<RestaurantImageUrl> imagesUrl)
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
