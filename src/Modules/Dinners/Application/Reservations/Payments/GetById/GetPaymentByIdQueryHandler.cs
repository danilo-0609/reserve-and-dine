using Dinners.Application.Common;
using Dinners.Domain.Reservations.ReservationsPayments;
using Dinners.Domain.Reservations.ReservationsPayments.Errors;
using ErrorOr;

namespace Dinners.Application.Reservations.Payments.GetById;

internal sealed class GetPaymentByIdQueryHandler : IQueryHandler<GetPaymentByIdQuery, ErrorOr<PaymentResponse>>
{
    private readonly IReservationPaymentRepository _reservationPaymentRepository;

    public GetPaymentByIdQueryHandler(IReservationPaymentRepository reservationPaymentRepository)
    {
        _reservationPaymentRepository = reservationPaymentRepository;
    }

    public async Task<ErrorOr<PaymentResponse>> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
    {
        ReservationPayment? payment = await _reservationPaymentRepository.GetByIdAsync(ReservationPaymentId.Create(request.PaymentId), cancellationToken);

        if (payment is null)
        {
            return ReservationPaymentsErrorsCodes.NotFound;
        }

        return new PaymentResponse(payment.Id.Value,
            payment.ReservationId.Value,
            payment.PayerId,
            payment.Price,
            payment.PayedAt);
    }
}
