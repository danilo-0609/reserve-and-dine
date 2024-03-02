using Dinners.Application.Common;
using Dinners.Domain.Menus.MenuReviews;
using Dinners.Domain.Menus.MenuReviews.Errors;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Menus.UpdateReview.Rate;

internal sealed class UpdateReviewRateCommandHandler : ICommandHandler<UpdateReviewRateCommand, ErrorOr<Unit>>
{
    private readonly IMenuReviewRepository _menuReviewRepository;

    public UpdateReviewRateCommandHandler(IMenuReviewRepository menuReviewRepository)
    {
        _menuReviewRepository = menuReviewRepository;
    }

    public async Task<ErrorOr<Unit>> Handle(UpdateReviewRateCommand request, CancellationToken cancellationToken)
    {
        MenuReview? menuReview = await _menuReviewRepository.GetByIdAsync(MenuReviewId.Create(request.MenuReviewId));

        if (menuReview is null)
        {
            return MenuReviewsErrorCodes.NotFound;
        }

        var menuReviewUpdate = menuReview.ModifyRate(request.Rate, DateTime.UtcNow);

        if (menuReviewUpdate.IsError)
        {
            return menuReviewUpdate.FirstError;
        }

        await _menuReviewRepository.UpdateAsync(menuReviewUpdate.Value);

        return Unit.Value;
    }
}
