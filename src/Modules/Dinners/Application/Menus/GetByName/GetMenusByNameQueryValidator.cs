using FluentValidation;

namespace Dinners.Application.Menus.GetByName;

internal sealed class GetMenusByNameQueryValidator : AbstractValidator<GetMenusByNameQuery>
{
    public GetMenusByNameQueryValidator()
    {
        RuleFor(r => r.Name)
            .NotNull().NotEmpty();
    }
}
