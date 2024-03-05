using Dinners.Application.Common;
using ErrorOr;

namespace Dinners.Application.Reservations.Refunds;

public sealed record GetRefundByIdQuery(Guid RefundId) : IQuery<ErrorOr<RefundResponse>>;
