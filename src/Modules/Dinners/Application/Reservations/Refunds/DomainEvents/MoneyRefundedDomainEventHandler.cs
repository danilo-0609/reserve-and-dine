using BuildingBlocks.Application;
using Dinners.Application.Common;
using Dinners.Domain.Reservations.Refunds;
using Dinners.Domain.Reservations.Refunds.Events;
using Dinners.IntegrationEvents;

namespace Dinners.Application.Reservations.Refunds.DomainEvents;

internal sealed class MoneyRefundedDomainEventHandler : IDomainEventHandler<MoneyRefundedDomainEvent>
{
    private readonly IRefundRepository _refundRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEventBus _eventBus;

    public MoneyRefundedDomainEventHandler(IRefundRepository refundRepository, IUnitOfWork unitOfWork, IEventBus eventBus)
    {
        _refundRepository = refundRepository;
        _unitOfWork = unitOfWork;
        _eventBus = eventBus;
    }

    public async Task Handle(MoneyRefundedDomainEvent notification, CancellationToken cancellationToken)
    {
        Refund refund = Refund.Create(notification.RefundId, 
            notification.ClientId,
            notification.RefundedMoney,
            notification.OcurredOn);
    
        await _refundRepository.AddAsync(refund);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _eventBus.PublishAsync(new MoneyRefundedIntegrationEvent(notification.DomainEventId,
            notification.RefundId.Value,
            notification.ReservationId.Value,
            notification.ClientId,
            notification.RefundedMoney.Amount,
            notification.RefundedMoney.Currency,
            notification.OcurredOn));
    }
}
