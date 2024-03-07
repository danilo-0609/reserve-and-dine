using Dinners.Application.Common;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Restaurants.Administration.Delete;

public sealed record DeleteAdministrationCommand(Guid RestaurantId,
    Guid AdministratorId) : ICommand<ErrorOr<Unit>>;
