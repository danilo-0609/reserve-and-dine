using FluentValidation;

namespace Dinners.Application.Restaurants.AvailableTables;

internal sealed class ModifyAvailableTableStatusCommandValidator : AbstractValidator<ModifyAvailableTableStatusCommand>
{
    public ModifyAvailableTableStatusCommandValidator()
    {
        RuleFor(r => r.RestaurantId)
            .NotNull().NotEmpty();

        RuleFor(r => r.AvailableTableStatus)
            .NotNull().NotEmpty()
            .Must(value => Enum.TryParse(value, out AvailableTableStatus result))
            .WithMessage("Available table status must be NoAvailables, Few or Availables");
    }

    private enum AvailableTableStatus
    {
        NoAvailables,
        Few,
        Availables
    }
}
