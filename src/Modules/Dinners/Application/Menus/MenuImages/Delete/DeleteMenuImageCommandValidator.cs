using FluentValidation;

namespace Dinners.Application.Menus.MenuImages.Delete;

internal sealed class DeleteMenuImageCommandValidator : AbstractValidator<DeleteMenuImageCommand>
{
    public DeleteMenuImageCommandValidator()
    {
        RuleFor(r => r.Id)
            .NotEmpty();

        RuleFor(r => r.MenuImageUrl)
            .NotEmpty();
    }
}
