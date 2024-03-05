using Dinners.Application.Common;
using ErrorOr;

namespace Dinners.Application.Reservations.GetById;

public sealed record GetReservationByIdQuery(Guid ReservationId) : IQuery<ErrorOr<ReservationResponse>>;