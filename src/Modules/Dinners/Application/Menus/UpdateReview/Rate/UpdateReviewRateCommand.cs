using Dinners.Application.Common;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Menus.UpdateReview.Rate;

public sealed record UpdateReviewRateCommand(Guid MenuReviewId,
    decimal Rate) : ICommand<ErrorOr<Unit>>;
