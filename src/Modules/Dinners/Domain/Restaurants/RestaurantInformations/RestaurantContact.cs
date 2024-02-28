namespace Dinners.Domain.Restaurants.RestaurantInformations;

public sealed record RestaurantContact
{
    public string Email { get; private set; } = string.Empty;

    public string Whatsapp { get; private set; } = string.Empty;

    public string Facebook { get; private set; } = string.Empty;

    public string PhoneNumber { get; private set; } = string.Empty;

    public string Instagram { get; private set; } = string.Empty;

    public string Twitter { get; private set; } = string.Empty;

    public string TikTok { get; private set; } = string.Empty;

    public string Website { get; private set; } = string.Empty;


    public static RestaurantContact Create(string email,
        string whatsapp,
        string facebook,
        string phoneNumber,
        string instagram,
        string twitter,
        string tikTok,
        string website)
    {
        return new RestaurantContact(email,
            whatsapp,
            facebook,
            phoneNumber,
            instagram,
            twitter,
            tikTok,
            website);
    }


    private RestaurantContact(string email,
        string whatsapp,
        string facebook,
        string phoneNumber,
        string instagram,
        string twitter,
        string tikTok,
        string website)
    {
        Email = email;
        Whatsapp = whatsapp;
        Facebook = facebook;
        PhoneNumber = phoneNumber;
        Instagram = instagram;
        Twitter = twitter;
        TikTok = tikTok;
        Website = website;
    }

    private RestaurantContact() { }
}

