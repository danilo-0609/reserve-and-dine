using Dinners.Application.Common;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Restaurants.Administration.Update;

public sealed record UpdateAdministrationCommand(Guid RestaurantId,
    string Name,
    string AdministratorTitle,
    Guid AdminId) : ICommand<ErrorOr<Unit>>;
