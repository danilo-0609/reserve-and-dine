using BuildingBlocks.Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;

namespace Dinners.Infrastructure.Outbox.BackgroundJobs;

[DisallowConcurrentExecution]
internal sealed class ProcessDinnersOutboxMessagesJob : IJob
{
    private readonly DinnersDbContext _dbContext;
    private readonly IPublisher _publisher;
    private readonly ILogger<ProcessDinnersOutboxMessagesJob> _logger;
    
    public ProcessDinnersOutboxMessagesJob(DinnersDbContext dinnersDbContext, 
        IPublisher publisher, 
        ILogger<ProcessDinnersOutboxMessagesJob> logger)
    {
        _dbContext = dinnersDbContext;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Executing {@Name}. At {@DateTime}",
            nameof(ProcessDinnersOutboxMessagesJob),
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
