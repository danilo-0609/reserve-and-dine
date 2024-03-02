using FluentValidation;

namespace Dinners.Application.Menus.DishSpecifications;

internal sealed class UpdateDishSpecificationCommandValidator : AbstractValidator<UpdateDishSpecificationCommand>
{
    public UpdateDishSpecificationCommandValidator()
    {
        RuleFor(r => r.MenuId)
            .NotEmpty().NotNull();

        RuleFor(r => r.Ingredients)
            .NotEmpty().NotNull();
    }
}
