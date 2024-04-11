using BuildingBlocks.Domain.Entities;
using Newtonsoft.Json;

namespace Dinners.Domain.Reservations.ReservationsPayments;

public sealed record ReservationPaymentId : EntityId<Guid>
{
    public override Guid Value { get; protected set; }

    public static ReservationPaymentId Create(Guid paymentId) => new ReservationPaymentId(paymentId);

    public static ReservationPaymentId CreateUnique() => new ReservationPaymentId(Guid.NewGuid());

    [JsonConstructor]
    private ReservationPaymentId(Guid value)
    {
        Value = value;
    }

    private ReservationPaymentId() { }
}
