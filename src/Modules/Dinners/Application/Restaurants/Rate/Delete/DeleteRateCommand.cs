using Dinners.Application.Common;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Restaurants.Rate.Delete;

public sealed record DeleteRateCommand(Guid RatingId) : ICommand<ErrorOr<Unit>>;
