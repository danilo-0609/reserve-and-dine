using Dinners.Application.Common;
using Dinners.Domain.Menus;
using Dinners.Domain.Menus.MenuReviews;
using Dinners.Domain.Menus.MenuReviews.Errors;
using ErrorOr;

namespace Dinners.Application.Menus.GetReviewsByMenuId;

internal sealed class GetMenuReviewsByMenuIdQueryHandler : IQueryHandler<GetMenuReviewsByMenuIdQuery, ErrorOr<List<MenuReviewResponse>>>
{
    private readonly IMenuRepository _menuRepository;
    private readonly IReviewRepository _menuReviewRepository;

    public GetMenuReviewsByMenuIdQueryHandler(IMenuRepository menuRepository, IReviewRepository menuReviewRepository)
    {
        _menuRepository = menuRepository;
        _menuReviewRepository = menuReviewRepository;
    }

    public async Task<ErrorOr<List<MenuReviewResponse>>> Handle(GetMenuReviewsByMenuIdQuery request, CancellationToken cancellationToken)
    {
        List<MenuReviewId> menuReviewsIds = await _menuRepository.GetMenuReviewsIdByIdAsync(MenuId.Create(request.MenuId), cancellationToken); 
    
        if (!menuReviewsIds.Any())
        {
            return MenuReviewsErrorCodes.NotFound;
        }

        List<MenuReview> menuReviews = new();

        menuReviewsIds.ForEach(async menuReviewId =>
        {
            var menuReview = await _menuReviewRepository.GetByIdAsync(menuReviewId, cancellationToken);

            menuReviews.Add(menuReview!);
        });

        var menuReviewResponses = menuReviews.ConvertAll(menuReview =>
        {
            return new MenuReviewResponse(menuReview.Id.Value,
                menuReview.Rate,
                menuReview.ClientId,
                menuReview.Comment,
                menuReview.ReviewedAt,
                menuReview.UpdatedAt);
        });

        return menuReviewResponses;
    }
}
