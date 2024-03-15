using FluentValidation;

namespace Dinners.Application.Menus.MenuImages.Get;

internal sealed class GetMenuImagesByIdQueryValidator : AbstractValidator<GetMenuImagesByIdQuery>
{
    public GetMenuImagesByIdQueryValidator()
    {
        RuleFor(r => r.MenuId)
            .NotEmpty().NotNull();
    }
}
