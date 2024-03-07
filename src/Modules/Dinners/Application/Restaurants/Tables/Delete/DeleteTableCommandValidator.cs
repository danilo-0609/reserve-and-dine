using FluentValidation;

namespace Dinners.Application.Restaurants.Tables.Delete;

internal sealed class DeleteTableCommandValidator : AbstractValidator<DeleteTableCommand>
{
    public DeleteTableCommandValidator()
    {
        RuleFor(r => r.RestaurantId)
            .NotNull().NotEmpty();

        RuleFor(r => r.Number)
            .NotEmpty().NotNull();
    }
}
