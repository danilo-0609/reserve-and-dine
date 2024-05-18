using FluentValidation;

namespace Users.Application.UserRegistrations.ConfirmUserRegistration;

internal sealed class ConfirmUserRegistrationCommandValidator : AbstractValidator<ConfirmUserRegistrationCommand>
{
    public ConfirmUserRegistrationCommandValidator()
    {
        RuleFor(r => r.Id)
            .NotEmpty()
            .NotNull();
    }
}
