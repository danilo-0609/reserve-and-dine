using ErrorOr;
using Users.Application.Common;
using Users.Domain.Common;
using Users.Domain.Users;

namespace Users.Application.Users.ChangePassword;

internal sealed class ChangePasswordCommandHandler : ICommandHandler<ChangePasswordCommand, ErrorOr<Success>>
{
    private readonly IUserRepository _userRepository;

    public ChangePasswordCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<Success>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(UserId.Create(request.Id), cancellationToken);
    
        if (user is null)
        {
            return UserErrorCodes.NotFound;
        }

        if (user.Password != Password.Create(request.OldPassword))
        {
            return UserErrorCodes.IncorrectPassword;
        }

        if (Password.Create(request.NewPassword) != Password.Create(request.NewPasswordConfirmed))
        {
            return UserErrorCodes.PasswordConfirmationIncorrect;
        }

        user.ChangePassword(Password.Create(request.NewPassword));

        await _userRepository.UpdateAsync(user, cancellationToken);

        return new Success();
    }
}
