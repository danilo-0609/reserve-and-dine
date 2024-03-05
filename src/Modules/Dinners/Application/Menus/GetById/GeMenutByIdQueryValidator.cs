using FluentValidation;

namespace Dinners.Application.Menus.GetById;

internal sealed class GeMenutByIdQueryValidator : AbstractValidator<GetMenuByIdQuery>
{
    public GeMenutByIdQueryValidator()
    {
        RuleFor(r => r.MenuId)
            .NotNull();
    }
}
