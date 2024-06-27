using Dinners.Application.Common;
using ErrorOr;

namespace Dinners.Application.Reservations.Finish;

public sealed record FinishReservationCommand(Guid ReservationId) : ICommand<ErrorOr<Success>>;
