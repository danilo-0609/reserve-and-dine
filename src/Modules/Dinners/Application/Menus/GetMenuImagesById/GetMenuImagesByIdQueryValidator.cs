using FluentValidation;

namespace Dinners.Application.Menus.GetMenuImagesById;

internal sealed class GetMenuImagesByIdQueryValidator : AbstractValidator<GetMenuImagesByIdQuery>
{
    public GetMenuImagesByIdQueryValidator()
    {
        RuleFor(r => r.MenuId)
            .NotEmpty().NotNull();
    }
}
