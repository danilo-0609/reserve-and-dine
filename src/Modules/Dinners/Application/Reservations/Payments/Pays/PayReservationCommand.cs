using Dinners.Application.Common;
using ErrorOr;

namespace Dinners.Application.Reservations.Payments.Pays;

public sealed record PayReservationCommand(Guid ReservationId) : ICommand<ErrorOr<Guid>>;
