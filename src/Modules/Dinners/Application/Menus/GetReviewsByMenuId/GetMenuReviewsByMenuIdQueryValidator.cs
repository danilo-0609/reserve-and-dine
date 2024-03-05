using FluentValidation;

namespace Dinners.Application.Menus.GetReviewsByMenuId;

internal sealed class GetMenuReviewsByMenuIdQueryValidator : AbstractValidator<GetMenuReviewsByMenuIdQuery>
{
    public GetMenuReviewsByMenuIdQueryValidator()
    {
        RuleFor(r => r.MenuId)
            .NotEmpty().NotNull();
    }
}
