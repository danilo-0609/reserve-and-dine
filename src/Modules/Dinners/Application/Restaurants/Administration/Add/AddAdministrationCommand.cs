using Dinners.Application.Common;
using ErrorOr;

namespace Dinners.Application.Restaurants.Administration.Add;

public sealed record AddAdministrationCommand(Guid RestaurantId,
    string Name,
    string AdministratorTitle,
    Guid NewAdministratorId) : ICommand<ErrorOr<Guid>>;
