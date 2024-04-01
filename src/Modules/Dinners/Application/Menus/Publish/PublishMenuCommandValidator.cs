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
            .Must(value => Enum.TryParse(value, out MenuType menuType))
            .WithMessage("Menu type must be a valid value")
            .NotEmpty();

        RuleFor(r => r.Price)
            .LessThanOrEqualTo(2_000_000);

        RuleFor(r => r.Currency)
            .NotEmpty()
            .Must(value => Enum.TryParse(value, out Currency currency))
            .WithMessage("Currency must be COP or USD");

        RuleFor(r => r.Discount)
            .NotNull();

        RuleFor(r => r.Tags)
            .NotEmpty()
            .NotNull();

        RuleFor(r => r.IsVegetarian)
            .NotNull();

        RuleFor(r => r.PrimaryChefName)
            .NotEmpty()
            .NotNull();

        RuleFor(r => r.HasAlcohol)
            .NotNull();

        RuleFor(r => r.Ingredients)
            .NotEmpty()
            .NotNull();
    }

    private enum MenuType
    {
        Breakfast,
        Lunch,
        Dinner
    }

    private enum Currency
    {
        USD,
        COP
    }
}
