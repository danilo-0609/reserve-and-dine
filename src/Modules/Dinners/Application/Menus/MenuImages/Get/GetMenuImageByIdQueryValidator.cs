using FluentValidation;

namespace Dinners.Application.Menus.MenuImages.Get;

internal sealed class GetMenuImageByIdQueryValidator : AbstractValidator<GetMenuImageByIdQuery>
{
    public GetMenuImageByIdQueryValidator()
    {
        RuleFor(r => r.MenuId)
            .NotEmpty().NotNull();
    
        RuleFor(r => r.ImageId)
            .NotEmpty().NotNull();
    }
}
