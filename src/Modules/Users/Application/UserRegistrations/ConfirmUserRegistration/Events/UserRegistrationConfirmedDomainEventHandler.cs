using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Users.Application.Common;
using Users.Application.Services;
using Users.Domain.UserRegistrations;
using Users.Domain.UserRegistrations.Events;
using Users.Domain.Users;

namespace Users.Application.UserRegistrations.ConfirmUserRegistration.Events;

internal sealed class UserRegistrationConfirmedDomainEventHandler : INotificationHandler<UserRegistrationConfirmedDomainEvent>
{
    private readonly IUserRegistrationRepository _userRegistrationRepository;
    private readonly ILogger<UserRegistrationConfirmedDomainEventHandler> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserRegistrationConfirmedDomainEventHandler(IUserRegistrationRepository userRegistrationRepository, ILogger<UserRegistrationConfirmedDomainEventHandler> logger, IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRegistrationRepository = userRegistrationRepository;
        _logger = logger;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UserRegistrationConfirmedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Working on {@Name}, at {@DateTime}", 
            nameof(UserRegistrationConfirmedDomainEventHandler),
            DateTime.Now);

        var userRegistration = await _userRegistrationRepository.GetByIdAsync(domainEvent.Id);
    
        if (userRegistration is null)
        {
            throw new Exception("User registration not found");
        }

        var user = userRegistration.CreateUser();

        await _userRepository.AddAsync(user.Value, cancellationToken);

        await _unitOfWork.SaveChangesAsync();
    }
}
