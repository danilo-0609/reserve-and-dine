using BuildingBlocks.Domain.Entities;
using Dinners.Domain.Reservations.ReservationsPayments.Rules;
using ErrorOr;

namespace Dinners.Domain.Reservations.ReservationsPayments;

public sealed class ReservationPayment : Entity<ReservationPaymentId, Guid>
{
    public new ReservationPaymentId Id { get; private set; }

    public ReservationId ReservationId { get; private set; }

    public Guid PayerId { get; private set; }

    public decimal MoneyAmount { get; private set; }

    public DateTime PayedAt { get; private set; }

    internal static ErrorOr<ReservationPayment> PayFromReservation(
        Guid payerId, 
        ReservationId reservationId,
        decimal moneyAmount,
        ReservationStatus reservationStatus,
        DateTime payedAt)
    {
        var payment = new ReservationPayment(
            ReservationPaymentId.CreateUnique(),
            payerId,
            moneyAmount,
            payedAt,
            reservationId);

        var statusIsConfirmedRule = payment.CheckRule(new PaymentCannotBeMadeWhenReservationStatusIsNotConfirmedRule(reservationStatus));

        if (statusIsConfirmedRule.IsError)
        {
            return statusIsConfirmedRule.FirstError;
        }

        return payment;
    }

    public ReservationPayment(ReservationPaymentId id,
        Guid payerId,
        decimal moneyAmount,
        DateTime payedAt,
        ReservationId reservationId)
    {
        Id = id;
        PayerId = payerId;
        MoneyAmount = moneyAmount;
        PayedAt = payedAt;
        ReservationId = reservationId;
    }
}
