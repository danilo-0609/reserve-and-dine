using BuildingBlocks.Domain.Entities;
using Dinners.Domain.Common;
using Dinners.Domain.Reservations.Payments.Events;
using Dinners.Domain.Reservations.ReservationsPayments.Rules;
using ErrorOr;

namespace Dinners.Domain.Reservations.ReservationsPayments;

public sealed class ReservationPayment : Entity<ReservationPaymentId, Guid>
{
    public new ReservationPaymentId Id { get; private set; }

    public ReservationId ReservationId { get; private set; }

    public Guid PayerId { get; private set; }

    public Price Price { get; private set; }

    public DateTime PayedAt { get; private set; }

    internal static ErrorOr<ReservationPayment> PayFromReservation(
        Guid payerId, 
        ReservationId reservationId,
        Price price,
        ReservationStatus reservationStatus,
        DateTime payedAt)
    {
        var payment = new ReservationPayment(
            ReservationPaymentId.CreateUnique(),
            payerId,
            price,
            payedAt,
            reservationId);

        var statusMustBeRequestedRule = payment.CheckRule(new PaymentCannotBeMadeWhenReservationStatusIsNotRequestedRule(reservationStatus));

        if (statusMustBeRequestedRule.IsError)
        {
            return statusMustBeRequestedRule.FirstError;
        }

        payment.AddDomainEvent(new ReservationPayedDomainEvent(Guid.NewGuid(),
            payment.Id,
            payment.ReservationId,
            payerId,
            price,
            payedAt));

        return payment;
    }

    public static ReservationPayment Create(ReservationPaymentId id,
        ReservationId reservationId,
        Guid payerId,
        Price price,
        DateTime payedAt)
    {
        return new ReservationPayment(id, payerId, price, payedAt, reservationId);
    }

    private ReservationPayment(ReservationPaymentId id,
        Guid payerId,
        Price price,
        DateTime payedAt,
        ReservationId reservationId)
    {
        Id = id;
        PayerId = payerId;
        Price = price;
        PayedAt = payedAt;
        ReservationId = reservationId;
    }

    private ReservationPayment() { }
}
