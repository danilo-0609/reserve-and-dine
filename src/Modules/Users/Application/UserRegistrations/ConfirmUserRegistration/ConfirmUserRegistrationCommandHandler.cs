using ErrorOr;
using Users.Application.Common;
using Users.Domain.UserRegistrations;

namespace Users.Application.UserRegistrations.ConfirmUserRegistration;

internal sealed class ConfirmUserRegistrationCommandHandler : ICommandHandler<ConfirmUserRegistrationCommand, ErrorOr<Success>>
{
    private readonly IUserRegistrationRepository _userRegistrationRepository;

    public ConfirmUserRegistrationCommandHandler(IUserRegistrationRepository userRegistrationRepository)
    {
        _userRegistrationRepository = userRegistrationRepository;
    }

    public async Task<ErrorOr<Success>> Handle(ConfirmUserRegistrationCommand request, CancellationToken cancellationToken)
    {
        var userRegistration = await _userRegistrationRepository.GetByIdAsync(UserRegistrationId.Create(request.Id));
    
        if (userRegistration is null)
        {
            return UserRegistrationErrorCodes.NotFound;
        }

        userRegistration.Confirm();

        await _userRegistrationRepository.UpdateAsync(userRegistration);

        return new Success();
    }
}
