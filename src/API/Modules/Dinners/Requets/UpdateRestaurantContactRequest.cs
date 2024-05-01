namespace API.Modules.Dinners.Requets;

public sealed record UpdateRestaurantContactRequest(string Email = "",
    string Whatsapp = "",
    string Facebook = "",
    string PhoneNumber = "",
    string Instagram = "",
    string Twitter = "",
    string TikTok = "",
    string Website = "");
