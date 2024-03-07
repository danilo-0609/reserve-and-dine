namespace Dinners.Application.Restaurants.Post.Requests;

public sealed record RestaurantContactRequest(string Email = "",
    string Whatsapp = "",
    string Facebook = "",
    string PhoneNumber = "",
    string Instagram = "",
    string Twitter = "",
    string TikTok = "",
    string Website = "");
