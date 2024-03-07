namespace Dinners.Application.Restaurants;

public sealed record RestaurantContactResponse(string Email,
    string Whatsapp,
    string Facebook,
    string PhoneNumber,
    string Instagram,
    string Twitter,
    string TikTok,
    string Website);
