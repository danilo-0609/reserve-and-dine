using BuildingBlocks.Domain.Events;
using Newtonsoft.Json;
using Users.Application.Common;
using Users.Infrastructure.Outbox;

namespace Users.Infrastructure;

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly UsersDbContext _dbContext;

    public UnitOfWork(UsersDbContext dbContext)
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
