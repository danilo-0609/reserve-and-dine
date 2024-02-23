using BuildingBlocks.Domain.AggregateRoots;

namespace Dinners.Domain.Reservations;

public sealed record ReservationId : AggregateRootId<Guid>
{
    public override Guid Value { get; protected set; }

    public static ReservationId Create(Guid reservationId) => new ReservationId(reservationId);

    public static ReservationId CreateUnique() => new ReservationId(Guid.NewGuid());

    private ReservationId(Guid value)
    {
        Value = value;
    }
}
