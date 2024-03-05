using BuildingBlocks.Application;
using Dinners.Application.Common;
using Dinners.Domain.Reservations.Payments.Events;
using Dinners.Domain.Reservations.ReservationsPayments;
using Dinners.IntegrationEvents;

namespace Dinners.Application.Reservations.Payments.DomainEvents;

internal sealed class ReservationPayedDomainEventHandler : IDomainEventHandler<ReservationPayedDomainEvent>
{
    private readonly IEventBus _eventBus;
    private readonly IReservationPaymentRepository _reservationPaymentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReservationPayedDomainEventHandler(IEventBus eventBus, IReservationPaymentRepository reservationPaymentRepository, IUnitOfWork unitOfWork)
    {
        _eventBus = eventBus;
        _reservationPaymentRepository = reservationPaymentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ReservationPayedDomainEvent notification, CancellationToken cancellationToken)
    {
        var payment = ReservationPayment.Create(notification.ReservationPaymentId,
            notification.ReservationId,
            notification.ClientId,
            notification.Price,
            notification.OcurredOn);

        await _reservationPaymentRepository.AddAsync(payment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);


        await _eventBus.PublishAsync(new ReservationPayedIntegrationEvent(notification.DomainEventId,
            notification.ReservationPaymentId.Value,
            notification.ReservationId.Value,
            notification.ClientId,
            notification.Price.Amount,
            notification.Price.Currency,
            notification.OcurredOn));
    }
}
