using Dinners.Application.Common;
using ErrorOr;

namespace Dinners.Application.Reservations.Visit;

public sealed record VisitReservationCommand(Guid ReservationId) : ICommand<ErrorOr<Success>>;
