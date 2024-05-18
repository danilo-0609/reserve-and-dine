using ErrorOr;
using Users.Application.Common;

namespace Users.Application.Users.Login;

public sealed record LoginUserCommand(string Email, string Password) : ICommand<ErrorOr<string>>;
