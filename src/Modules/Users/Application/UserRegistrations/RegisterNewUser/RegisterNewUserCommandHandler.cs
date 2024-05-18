using ErrorOr;
using Users.Application.Common;
using Users.Domain.UserRegistrations;

namespace Users.Application.UserRegistrations.RegisterNewUser;

internal sealed class RegisterNewUserCommandHandler : ICommandHandler<RegisterNewUserCommand, ErrorOr<Guid>>
{
    private readonly IUserRegistrationRepository _userRegistrationRepository;
    private readonly IUsersCounter _usersCounter;

    public RegisterNewUserCommandHandler(IUserRegistrationRepository userRegistrationRepository, IUsersCounter usersCounter)
    {
        _userRegistrationRepository = userRegistrationRepository;
        _usersCounter = usersCounter;
    }

    public async Task<ErrorOr<Guid>> Handle(RegisterNewUserCommand request, CancellationToken cancellationToken)
    {
        var userRegistration = UserRegistration.RegisterNewUser(request.Login,
            request.Password,
            request.Email,
            _usersCounter,
            DateTime.UtcNow);
    
        if (userRegistration.IsError)
        {
            return userRegistration.FirstError;
        }

        var isEmailUnique = await _userRegistrationRepository.IsEmailUnique(request.Email, cancellationToken);

        if (isEmailUnique is false)
        {
            return UserRegistrationErrorCodes.EmailIsNotUnique;
        }

        await _userRegistrationRepository.AddAsync(userRegistration.Value);

        return userRegistration.Value.Id.Value;
    }
}
