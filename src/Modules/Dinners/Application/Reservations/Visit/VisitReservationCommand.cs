using Dinners.Application.Common;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Reservations.Visit;

public sealed record VisitReservationCommand(Guid ReservationId) : ICommand<ErrorOr<Unit>>;
