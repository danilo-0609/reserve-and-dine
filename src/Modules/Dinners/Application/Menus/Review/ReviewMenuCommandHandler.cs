using BuildingBlocks.Application;
using Dinners.Application.Common;
using Dinners.Domain.Menus;
using Dinners.Domain.Menus.Errors;
using Dinners.Domain.Menus.MenuReviews;
using ErrorOr;

namespace Dinners.Application.Menus.Review;

internal sealed class ReviewMenuCommandHandler : ICommandHandler<ReviewMenuCommand, ErrorOr<Guid>>
{
    private readonly IMenuReviewRepository _menuReviewRepository;
    private readonly IMenuRepository _menuRepository;
    private readonly IExecutionContextAccessor _executionContextAccessor;

    public ReviewMenuCommandHandler(IMenuReviewRepository menuReviewRepository, IMenuRepository menuRepository, IExecutionContextAccessor executionContextAccessor)
    {
        _menuReviewRepository = menuReviewRepository;
        _menuRepository = menuRepository;
        _executionContextAccessor = executionContextAccessor;
    }

    public async Task<ErrorOr<Guid>> Handle(ReviewMenuCommand request, CancellationToken cancellationToken)
    {
        Menu? menu = await _menuRepository.GetByIdAsync(MenuId.Create(request.MenuId));

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
            menu.MenuSpecification,
            menu.DishSpecification,
            menu.MenuSchedule,
            menu.MenuConsumers.ToList(),
            DateTime.UtcNow);

        await _menuReviewRepository.AddAsync(review.Value);
        await _menuRepository.UpdateAsync(menuUpdate);

        return review.Value.Id.Value;
    }
}
