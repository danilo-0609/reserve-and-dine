using Dinners.Application.Common;
using ErrorOr;

namespace Dinners.Application.Reservations.Payments.GetById;

public sealed record GetPaymentByIdQuery(Guid PaymentId) : IQuery<ErrorOr<PaymentResponse>>;
