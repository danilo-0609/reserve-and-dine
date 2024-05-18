using ErrorOr;
using Users.Application.Common;
using Users.Domain.UserRegistrations;

namespace Users.Application.UserRegistrations.GetUserRegistrationById;

internal sealed class GetUserRegistrationByIdQueryHandler : ICommandHandler<GetUserRegistrationByIdQuery, ErrorOr<UserRegistrationResponse>>
{
    private readonly IUserRegistrationRepository _userRegistrationRepository;

    public GetUserRegistrationByIdQueryHandler(IUserRegistrationRepository userRegistrationRepository)
    {
        _userRegistrationRepository = userRegistrationRepository;
    }

    public async Task<ErrorOr<UserRegistrationResponse>> Handle(GetUserRegistrationByIdQuery request, CancellationToken cancellationToken)
    {
        var userRegistration = await _userRegistrationRepository.GetByIdAsync(UserRegistrationId.Create(request.Id));
    
        if (userRegistration is null)
        {
            return UserRegistrationErrorCodes.NotFound;
        }

        return new UserRegistrationResponse(userRegistration.Id.Value,
            userRegistration.Login,
            userRegistration.Email,
            userRegistration.RegisteredDate);
    }    
}
