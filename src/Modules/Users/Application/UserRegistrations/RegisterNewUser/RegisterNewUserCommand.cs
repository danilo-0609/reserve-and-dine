using ErrorOr;
using Users.Application.Common;

namespace Users.Application.UserRegistrations.RegisterNewUser;

public sealed record RegisterNewUserCommand(string Login,
    string Email,
    string Password) : ICommand<ErrorOr<Guid>>;
