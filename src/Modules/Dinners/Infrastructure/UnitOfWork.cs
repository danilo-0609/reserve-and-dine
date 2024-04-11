using BuildingBlocks.Domain.Events;
using Dinners.Application.Common;
using Dinners.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Dinners.Infrastructure;

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly DinnersDbContext _dbContext;

    public UnitOfWork(DinnersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await ConvertDomainEventsToOutboxMessages();
    
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task ConvertDomainEventsToOutboxMessages()
    {
        var domainEvents = _dbContext
            .ChangeTracker
            .Entries<IHasDomainEvents>()
            .Select(x => x.Entity)
            .SelectMany(entity =>
            {
                var @events = entity.GetDomainEvents();

                return @events;
            })
            .ToList();

        _dbContext
            .ChangeTracker
            .Entries<IHasDomainEvents>()
            .Where(x => x.Entity.DomainEvents.Any())
            .Select(x =>
            {
                x.Entity.ClearDomainEvents();

                return 0;
            });

        foreach (var entity in _dbContext.ChangeTracker.Entries<IHasDomainEvents>())
        {
            entity.State = EntityState.Detached;
        }

        List<OutboxMessage> outboxMessages = domainEvents
            .Select(domainEvent => new OutboxMessage
            {
                Id = domainEvent.DomainEventId,
                Type = domainEvent.GetType().Name,
                OcurredOnUtc = domainEvent.OcurredOn,
                Content = JsonConvert.SerializeObject(
                       domainEvent,
                       new JsonSerializerSettings
                       {
                           TypeNameHandling = TypeNameHandling.All,
                           
                       })
            }).ToList();

        await _dbContext
            .OutboxMessages
            .AddRangeAsync(outboxMessages);
    }
}
