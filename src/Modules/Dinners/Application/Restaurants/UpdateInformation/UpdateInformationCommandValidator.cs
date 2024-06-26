﻿using FluentValidation;

namespace Dinners.Application.Restaurants.UpdateInformation;

internal sealed class UpdateInformationCommandValidator : AbstractValidator<UpdateInformationCommand>
{
    public UpdateInformationCommandValidator()
    {
        RuleFor(r => r.RestaurantId)
            .NotNull();

        RuleFor(r => r.Title)
            .NotNull().NotEmpty();

        RuleFor(r => r.Description)
            .NotNull().NotEmpty();

        RuleFor(r => r.Type)
            .NotNull().NotEmpty();
    }
}
