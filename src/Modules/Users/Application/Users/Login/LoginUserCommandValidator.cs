using FluentValidation;

namespace Users.Application.Users.Login;

internal sealed class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(r => r.Email)
            .EmailAddress()
            .NotNull()
            .NotEmpty();

        RuleFor(r => r.Password)
            .NotEmpty()
            .NotNull();
    }
}
