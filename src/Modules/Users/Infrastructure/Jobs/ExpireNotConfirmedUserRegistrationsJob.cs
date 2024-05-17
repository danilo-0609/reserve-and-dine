using Microsoft.EntityFrameworkCore;
using Quartz;
using Users.Application.Common;
using Users.Domain.UserRegistrations;

namespace Users.Infrastructure.Jobs;

[DisallowConcurrentExecution]
internal sealed class ExpireNotConfirmedUserRegistrationsJob : IJob
{
    private readonly UsersDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public ExpireNotConfirmedUserRegistrationsJob(UsersDbContext dbContext, IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var expirationLimit = DateTime.UtcNow.AddHours(-2);

        var notConfirmedRegistrations = await _dbContext
            .UserRegistrations
            .Where(r => r.Status == UserRegistrationStatus.WaitingForConfirmation &&
                r.RegisteredDate > expirationLimit)
            .Take(20)
            .ToListAsync();
    
        foreach (var registration in notConfirmedRegistrations)
        {
            registration.Expired();

            _dbContext.UserRegistrations.Update(registration);
        }

        await _unitOfWork.SaveChangesAsync();
    }
}