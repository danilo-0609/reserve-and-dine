using Dinners.Application.Common;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Restaurants.Rate.Upgrade;

public sealed record UpgradeRateCommand(Guid RateId,
    int Stars,
    string Comment = "") : ICommand<ErrorOr<Unit>>;
