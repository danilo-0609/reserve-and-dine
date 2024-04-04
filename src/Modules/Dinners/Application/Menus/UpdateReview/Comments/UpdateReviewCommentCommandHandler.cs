using Dinners.Application.Common;
using Dinners.Domain.Menus.MenuReviews;
using Dinners.Domain.Menus.MenuReviews.Errors;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Menus.UpdateReview.Comments;

internal sealed class UpdateReviewCommentCommandHandler : ICommandHandler<UpdateReviewCommentCommand, ErrorOr<Unit>>
{
    private readonly IReviewRepository _menuReviewRepository;

    public UpdateReviewCommentCommandHandler(IReviewRepository menuReviewRepository)
    {
        _menuReviewRepository = menuReviewRepository;
    }

    public async Task<ErrorOr<Unit>> Handle(UpdateReviewCommentCommand request, CancellationToken cancellationToken)
    {
        MenuReview? menuReview = await _menuReviewRepository.GetByIdAsync(MenuReviewId.Create(request.MenuReviewId), cancellationToken);

        if (menuReview is null)
        {
            return MenuReviewsErrorCodes.NotFound;
        }

        var menuReviewUpdate = menuReview.ModifyComment(request.Comment, DateTime.UtcNow);

        await _menuReviewRepository.UpdateAsync(menuReviewUpdate);

        return Unit.Value;
    }
}
