using FluentValidation;

namespace Dinners.Application.Menus.GetById;

internal sealed class GetMenuByIdQueryValidator : AbstractValidator<GetMenuByIdQuery>
{
    public GetMenuByIdQueryValidator()
    {
        RuleFor(r => r.MenuId)
            .NotNull();
    }
}
