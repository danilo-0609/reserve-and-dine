using ErrorOr;
using Users.Application.Common;
using Users.Domain.UserRegistrations;
using Users.Domain.Users;

namespace Users.Application.UserRegistrations.ConfirmUserRegistration;

internal sealed class ConfirmUserRegistrationCommandHandler : ICommandHandler<ConfirmUserRegistrationCommand, ErrorOr<Success>>
{
    private readonly IUserRegistrationRepository _userRegistrationRepository;
    private readonly IUserRepository _userRepository;

    public ConfirmUserRegistrationCommandHandler(IUserRegistrationRepository userRegistrationRepository, IUserRepository userRepository)
    {
        _userRegistrationRepository = userRegistrationRepository;
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<Success>> Handle(ConfirmUserRegistrationCommand request, CancellationToken cancellationToken)
    {
        var userRegistration = await _userRegistrationRepository.GetByIdAsync(UserRegistrationId.Create(request.Id));
    
        if (userRegistration is null)
        {
            return UserRegistrationErrorCodes.NotFound;
        }

        var confirm = userRegistration.Confirm();

        if (confirm.IsError)
        {
            return confirm.FirstError;
        }

        var user = userRegistration.CreateUser();

        await _userRepository.AddAsync(user.Value, cancellationToken);
        await _userRegistrationRepository.UpdateAsync(userRegistration);

        return new Success();
    }
}
