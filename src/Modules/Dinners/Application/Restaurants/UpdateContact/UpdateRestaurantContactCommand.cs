using Dinners.Application.Common;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Restaurants.UpdateContact;

public sealed record UpdateRestaurantContactCommand(Guid RestaurantId,
    string Email = "",
    string Whatsapp = "",
    string Facebook = "",
    string PhoneNumber = "",
    string Instagram = "",
    string Twitter = "",
    string TikTok = "",
    string Website = "") : ICommand<ErrorOr<Unit>>;
