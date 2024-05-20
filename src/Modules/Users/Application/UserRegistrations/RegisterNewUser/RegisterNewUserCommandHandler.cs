using ErrorOr;
using Users.Application.Common;
using Users.Domain.UserRegistrations;
using Users.Domain.Users;

namespace Users.Application.UserRegistrations.RegisterNewUser;

internal sealed class RegisterNewUserCommandHandler : ICommandHandler<RegisterNewUserCommand, ErrorOr<Guid>>
{
    private readonly IUserRegistrationRepository _userRegistrationRepository;
    private readonly IUserRepository _userRepository;

    public RegisterNewUserCommandHandler(IUserRegistrationRepository userRegistrationRepository, IUsersCounter usersCounter, IUserRepository userRepository)
    {
        _userRegistrationRepository = userRegistrationRepository;
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<Guid>> Handle(RegisterNewUserCommand request, CancellationToken cancellationToken)
    {
        var isLoginUnique = await _userRepository.IsLoginUnique(request.Login, cancellationToken);

        if (isLoginUnique is false)
        {
            return UserRegistrationErrorCodes.LoginIsNotUnique;
        }

        var isEmailUnique = await _userRegistrationRepository.IsEmailUnique(request.Email, cancellationToken);

        if (isEmailUnique is false)
        {
            return UserRegistrationErrorCodes.EmailIsNotUnique;
        }

        var userRegistration = UserRegistration.RegisterNewUser(request.Login,
            request.Password,
            request.Email,
            DateTime.UtcNow);

        await _userRegistrationRepository.AddAsync(userRegistration);

        return userRegistration.Id.Value;
    }
}
