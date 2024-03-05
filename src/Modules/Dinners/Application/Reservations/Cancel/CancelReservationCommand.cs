using Dinners.Application.Common;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Reservations.Cancel;

public sealed record CancelReservationCommand(Guid ReservationId) : ICommand<ErrorOr<Unit>>;
 