using Dinners.Application.Common;
using Dinners.Domain.Reservations.Refunds;
using ErrorOr;

namespace Dinners.Application.Reservations.Refunds;

internal sealed class GetRefundByIdQueryHandler : IQueryHandler<GetRefundByIdQuery, ErrorOr<RefundResponse>>
{
    private readonly IRefundRepository _refundRepository;

    public GetRefundByIdQueryHandler(IRefundRepository refundRepository)
    {
        _refundRepository = refundRepository;
    }

    public async Task<ErrorOr<RefundResponse>> Handle(GetRefundByIdQuery request, CancellationToken cancellationToken)
    {
        Refund? refund = await _refundRepository.GetByIdAsync(RefundId.Create(request.RefundId), cancellationToken);
    
        if (refund is null)
        {
            return Error.NotFound("Refund.NotFound", "Refund was not found");
        }

        return new RefundResponse(refund.Id.Value, 
            refund.ClientId,
            refund.RefundedMoney,
            refund.RefundedAt);
    }
}
