using ErrorOr;
using Users.Application.Common;

namespace Users.Application.Users.ChangePassword;

public sealed record ChangePasswordCommand(Guid Id, 
    string OldPassword, 
    string NewPassword, 
    string NewPasswordConfirmed) : ICommand<ErrorOr<Success>>;
