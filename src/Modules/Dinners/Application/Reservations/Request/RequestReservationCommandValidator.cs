﻿using FluentValidation;

namespace Dinners.Application.Reservations.Request;

internal sealed class RequestReservationCommandValidator : AbstractValidator<RequestReservationCommand>
{
    public RequestReservationCommandValidator()
    {
        RuleFor(r => r.ReservedTable)
            .NotNull().NotEmpty();

        RuleFor(r => r.Start)
            .NotNull().NotEmpty();

        RuleFor(r => r.End)
            .NotNull().NotEmpty();

        RuleFor(r => r.RestaurantId)
            .NotNull().NotEmpty();

        RuleFor(r => r.Name)
            .NotNull().NotEmpty();

        RuleFor(r => r.NumberOfAttendees)
            .NotNull().NotEmpty();

        RuleFor(r => r.MenuIds)
            .NotNull();
    }
}
