using FluentValidation;

namespace Dinners.Application.Menus.Publish;

internal sealed class PublishMenuCommandValidator : AbstractValidator<PublishMenuCommand>
{
    public PublishMenuCommandValidator()
    {
        RuleFor(r => r.RestaurantId)
            .NotNull();

        RuleFor(r => r.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .NotNull();

        RuleFor(r => r.Description)
            .NotEmpty()
            .WithMessage("Description is required")
            .NotNull();

        RuleFor(r => r.MenuType)
            .Equal("Breakfast")
            .Equal("Lunch")
            .Equal("Dinner")
            .WithMessage("Menu type must be a valid value")
            .NotEmpty();

        RuleFor(r => r.Price)
            .LessThanOrEqualTo(2_000_000);

        RuleFor(r => r.Currency)
            .NotEmpty()
            .Equal("COP")
            .Equal("USD")
            .WithMessage("Currency must be COP or USD");

        RuleFor(r => r.Discount)
            .NotEmpty();

        RuleFor(r => r.Tags)
            .NotEmpty()
            .NotNull();

        RuleFor(r => r.IsVegetarian)
            .NotNull()
            .NotEmpty();

        RuleFor(r => r.PrimaryChefName)
            .NotEmpty()
            .NotNull();

        RuleFor(r => r.HasAlcohol)
            .NotEmpty()
            .NotNull();

        RuleFor(r => r.Ingredients)
            .NotEmpty()
            .NotNull();

        RuleFor(r => r.DayOfWeeks)
            .NotEmpty()
            .NotNull();
    
        RuleFor(r => r.Start)
            .NotNull()
            .NotEmpty();

        RuleFor(r => r.End)
            .NotNull()
            .NotEmpty();
    }
}
