using BuildingBlocks.Application;
using Users.Application.Common;
using Users.Domain.UserRegistrations;
using Users.Domain.UserRegistrations.Events;
using Users.Domain.Users;

namespace Users.Application.UserRegistrations.ConfirmUserRegistration.Events;

internal sealed class UserRegistrationConfirmedDomainEventHandler : IDomainEventHandler<UserRegistrationConfirmedDomainEvent>
{
    private readonly IUserRegistrationRepository _userRegistrationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserRegistrationConfirmedDomainEventHandler(IUserRegistrationRepository userRegistrationRepository, IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRegistrationRepository = userRegistrationRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UserRegistrationConfirmedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
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
