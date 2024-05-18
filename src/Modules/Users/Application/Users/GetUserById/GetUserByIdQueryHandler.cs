using ErrorOr;
using Users.Application.Common;
using Users.Domain.Users;

namespace Users.Application.Users.GetUserById;

internal sealed class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, ErrorOr<UserResponse>>
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<UserResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(UserId.Create(request.Id), cancellationToken);

        if (user is null)
        {
            return UserErrorCodes.NotFound;
        }

        UserResponse userResponse = new(user.Id.Value,
            user.Login,
            user.Email,
            user.CreatedDateTime);

        return userResponse;
    }
}
