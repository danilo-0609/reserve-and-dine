using BuildingBlocks.Domain.Entities;
using Dinners.Domain.Common;
using Dinners.Domain.Reservations.Refunds.Events;

namespace Dinners.Domain.Reservations.Refunds;

public sealed class Refund : Entity<RefundId, Guid>
{
    public new RefundId Id { get; private set; }

    public ReservationId ReservationId { get; private set; }

    public Guid ClientId { get; private set; }

    public Price RefundedMoney { get; private set; }

    public DateTime RefundedAt { get; private set; }
    

    public static Refund Payback(ReservationId reservationId,
        Guid clientId,
        Price refundedMoney,
        DateTime refundedAt)
    {
        Refund refund = new Refund(RefundId.CreateUnique(), reservationId, clientId, refundedMoney, refundedAt);

        refund.AddDomainEvent(new MoneyRefundedDomainEvent(Guid.NewGuid(), 
            refund.Id,
            reservationId,
            refund.ClientId,
            refund.RefundedMoney,
            refundedAt));

        return refund;
    }

    public static Refund Create(RefundId refundId,
        ReservationId reservationId,
        Guid clientId,
        Price refundedMoney,
        DateTime refundedAt)
    {
        return new Refund(refundId, reservationId, clientId, refundedMoney, refundedAt);
    }

    private Refund(RefundId id, 
        ReservationId reservationId, 
        Guid clientId, 
        Price refundedMoney, 
        DateTime refundedAt)
    {
        Id = id;
        ReservationId = reservationId;
        ClientId = clientId;
        RefundedMoney = refundedMoney;
        RefundedAt = refundedAt;
    }

    private Refund() { }
}
