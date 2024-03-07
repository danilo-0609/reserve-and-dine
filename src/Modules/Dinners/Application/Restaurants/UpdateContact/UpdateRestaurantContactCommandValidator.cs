using FluentValidation;

namespace Dinners.Application.Restaurants.UpdateContact;

internal sealed class UpdateRestaurantContactCommandValidator : AbstractValidator<UpdateRestaurantContactCommand>
{
    public UpdateRestaurantContactCommandValidator()
    {
        RuleFor(r => r.RestaurantId)
            .NotEmpty().NotNull();

        RuleFor(r => r.Email)
            .NotNull();

        RuleFor(r => r.Whatsapp)
            .NotNull();

        RuleFor(r => r.Facebook)
            .NotNull();

        RuleFor(r => r.PhoneNumber)
            .NotNull();

        RuleFor(r => r.Instagram)
            .NotNull();

        RuleFor(r => r.Twitter)
            .NotNull();

        RuleFor(r => r.TikTok)
            .NotNull();

        RuleFor(r => r.Website)
            .NotNull();
    }
}
