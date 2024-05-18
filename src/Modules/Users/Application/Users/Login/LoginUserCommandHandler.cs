using ErrorOr;
using Users.Application.Abstractions;
using Users.Application.Common;
using Users.Domain.Common;
using Users.Domain.Users;

namespace Users.Application.Users.Login;

internal sealed class LoginUserCommandHandler : ICommandHandler<LoginUserCommand, ErrorOr<string>>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtProvider _jwtProvider;

    public LoginUserCommandHandler(IUserRepository userRepository, IJwtProvider jwtProvider)
    {
        _userRepository = userRepository;
        _jwtProvider = jwtProvider;
    }

    public async Task<ErrorOr<string>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            return UserErrorCodes.NotFound;
        }

        if (user.Password != Password.Create(request.Password))
        {
            return UserErrorCodes.IncorrectPassword;
        }

        var token = _jwtProvider.Generate(user);

        return token;
    }
}
