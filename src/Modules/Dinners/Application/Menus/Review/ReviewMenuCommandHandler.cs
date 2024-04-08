using BuildingBlocks.Application;
using Dinners.Application.Common;
using Dinners.Domain.Menus;
using Dinners.Domain.Menus.Errors;
using Dinners.Domain.Menus.MenuReviews;
using ErrorOr;

namespace Dinners.Application.Menus.Review;

internal sealed class ReviewMenuCommandHandler : ICommandHandler<ReviewMenuCommand, ErrorOr<Guid>>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IMenuRepository _menuRepository;
    private readonly IExecutionContextAccessor _executionContextAccessor;
    private readonly IMenusReviewsRepository _menusReviewsRepository;

    public ReviewMenuCommandHandler(IReviewRepository menuReviewRepository, IMenuRepository menuRepository, IExecutionContextAccessor executionContextAccessor, IMenusReviewsRepository menusReviewsRepository)
    {
        _reviewRepository = menuReviewRepository;
        _menuRepository = menuRepository;
        _executionContextAccessor = executionContextAccessor;
        _menusReviewsRepository = menusReviewsRepository;
    }

    public async Task<ErrorOr<Guid>> Handle(ReviewMenuCommand request, CancellationToken cancellationToken)
    {
        Menu? menu = await _menuRepository.GetByIdAsync(MenuId.Create(request.MenuId), cancellationToken);

        if (menu is null)
        {
            return MenuErrorCodes.NotFound;
        }

        ErrorOr<MenuReview> review = menu.Review(_executionContextAccessor.UserId, 
            request.Rate, 
            menu.MenuConsumers.ToList(),
            DateTime.UtcNow,
            request.Comment);
    
        if (review.IsError)
        {
            return review.FirstError;
        }

        await _menuRepository.UpdateAsync(menu, cancellationToken);
        await _reviewRepository.AddAsync(review.Value);
        await _menusReviewsRepository.AddAsync(new MenusReviews(menu.Id, review.Value.Id), cancellationToken);

        return review.Value.Id.Value;
    }
}
