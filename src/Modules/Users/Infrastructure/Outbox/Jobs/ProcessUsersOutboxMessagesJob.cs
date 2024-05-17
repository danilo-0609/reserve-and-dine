using BuildingBlocks.Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;

namespace Users.Infrastructure.Outbox.Jobs;

[DisallowConcurrentExecution]
internal sealed class ProcessUsersOutboxMessagesJob : IJob
{
    private readonly UsersDbContext _dbContext;
    private readonly IPublisher _publisher;
    private readonly ILogger<ProcessUsersOutboxMessagesJob> _logger;
    
    public ProcessUsersOutboxMessagesJob(UsersDbContext dinnersDbContext, 
        IPublisher publisher, 
        ILogger<ProcessUsersOutboxMessagesJob> logger)
    {
        _dbContext = dinnersDbContext;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Executing {@Name}. At {@DateTime}",
            nameof(ProcessUsersOutboxMessagesJob),
            DateTime.Now);

        List<OutboxMessage> messages = await _dbContext
            .OutboxMessages
            .Where(r => r.ProcessedOnUtc == null)
            .Take(20)
            .ToListAsync();
    
        foreach (OutboxMessage message in messages)
        {
            IDomainEvent? domainEvent = JsonConvert
                .DeserializeObject<IDomainEvent>(
                    message.Content,
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    });

            if (domainEvent is null)
            {
                _logger.LogError("Problems. Domain event is null");
            }

            try
            {
                _logger.LogInformation("Publishing domain event {Name}. At {OcurredOn}",
                    domainEvent.GetType().FullName,
                    DateTime.UtcNow);

                await _publisher.Publish(domainEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Domain event Error: {ex.Message}");
            }

            message.ProcessedOnUtc = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync();
    }
}
