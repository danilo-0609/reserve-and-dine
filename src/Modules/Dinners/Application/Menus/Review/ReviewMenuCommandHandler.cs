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
    private readonly IMenusReviewsRepository _menuReviewsRepository;
    private readonly IExecutionContextAccessor _executionContextAccessor;

    public ReviewMenuCommandHandler(IReviewRepository menuReviewRepository, IMenuRepository menuRepository, IExecutionContextAccessor executionContextAccessor, IMenusReviewsRepository menuReviewsRepository)
    {
        _reviewRepository = menuReviewRepository;
        _menuRepository = menuRepository;
        _executionContextAccessor = executionContextAccessor;
        _menuReviewsRepository = menuReviewsRepository;
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

        var menuUpdate = menu.Update(menu.MenuReviewIds.ToList(),
            menu.MenuDetails,
            menu.DishSpecification,
            menu.MenuConsumers.ToList(),
            menu.MenuImagesUrl.ToList(),
            menu.Tags.ToList(),
            menu.MenuSchedules.ToList(),
            menu.Ingredients.ToList(),
            DateTime.UtcNow);

        await _menuRepository.UpdateAsync(menuUpdate, cancellationToken);
        await _menuReviewsRepository.AddAsync(new MenusReviews(menu.Id, review.Value.Id), cancellationToken);
        await _reviewRepository.AddAsync(review.Value);

        return review.Value.Id.Value;
    }
}
