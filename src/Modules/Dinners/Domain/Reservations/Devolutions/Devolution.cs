using BuildingBlocks.Domain.Entities;
using Dinners.Domain.Common;
using Dinners.Domain.Reservations.Devolutions.Events;

namespace Dinners.Domain.Reservations.Devolutions;

public sealed class Devolution : Entity<DevolutionId, Guid>
{
    public new DevolutionId Id { get; private set; }

    public ReservationId ReservationId { get; private set; }

    public Guid ClientId { get; private set; }

    public Price RefundedMoney { get; private set; }

    public DateTime RefundedAt { get; private set; }
    
    

    public static Devolution Refund(ReservationId reservationId,
        Guid clientId,
        Price refundedMoney,
        DateTime refundedAt)
    {
        Devolution devolution = new Devolution(DevolutionId.CreateUnique(), reservationId, clientId, refundedMoney, refundedAt);

        devolution.AddDomainEvent(new MoneyRefundedDomainEvent(Guid.NewGuid(), 
            devolution.Id,
            reservationId,
            refundedAt));

        return devolution;
    }

    private Devolution(DevolutionId id, 
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

    private Devolution() { }
}
