using FluentValidation;

namespace Dinners.Application.Menus.DishSpecifications;

internal sealed class UpdateDishSpecificationCommandValidator : AbstractValidator<UpdateDishSpecificationCommand>
{
    public UpdateDishSpecificationCommandValidator()
    {
        RuleFor(r => r.MenuId)
            .NotEmpty().NotNull();

        RuleFor(r => r.MainCourse)
            .NotNull();

        RuleFor(r => r.SideDishes)
            .NotNull();

        RuleFor(r => r.Appetizers)
            .NotNull();

        RuleFor(r => r.Beverages)
            .NotNull();

        RuleFor(r => r.Desserts)
            .NotNull();

        RuleFor(r => r.Sauces)
            .NotNull();

        RuleFor(r => r.Condiments)
            .NotNull();

        RuleFor(r => r.Coffee)
            .NotNull(); 
    }
}
