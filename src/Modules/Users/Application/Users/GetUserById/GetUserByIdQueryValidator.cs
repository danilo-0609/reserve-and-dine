using FluentValidation;

namespace Users.Application.Users.GetUserById;

internal sealed class GetUserByIdQueryValidator : AbstractValidator<GetUserByIdQuery>
{
    public GetUserByIdQueryValidator()
    {
        RuleFor(r => r.Id)
            .NotEmpty()
            .NotNull();
    }
}
