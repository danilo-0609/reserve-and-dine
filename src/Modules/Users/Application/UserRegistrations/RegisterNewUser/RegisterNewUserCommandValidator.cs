using FluentValidation;

namespace Users.Application.UserRegistrations.RegisterNewUser;

internal sealed class RegisterNewUserCommandValidator : AbstractValidator<RegisterNewUserCommand>
{
    public RegisterNewUserCommandValidator()
    {
        RuleFor(r => r.Login)
            .NotEmpty()
            .NotNull();

        RuleFor(r => r.Email)
            .EmailAddress()
            .NotEmpty()
            .NotNull();

        RuleFor(r => r.Password)
            .NotEmpty()
            .NotNull()
            .MinimumLength(20);
    }
}
