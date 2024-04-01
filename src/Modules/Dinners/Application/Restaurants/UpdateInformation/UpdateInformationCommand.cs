using Dinners.Application.Common;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Restaurants.UpdateInformation;

public sealed record UpdateInformationCommand(Guid RestaurantId,
    string Title,
    string Description,
    string Type) : ICommand<ErrorOr<Unit>>;
