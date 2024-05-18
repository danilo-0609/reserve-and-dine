using ErrorOr;

namespace Users.Domain.Users;

public static class UserErrorCodes
{
    public static Error NotFound =>
        Error.NotFound("User.NotFound", "User was not found");

    public static Error IncorrectPassword =>
        Error.Unauthorized("User.IncorrectPassword", "Password is incorrect");

    public static Error PasswordConfirmationIncorrect =>
        Error.Unauthorized("User.PasswordConfirmationIncorrect", "Password confirmation is incorrect. Doesn not match the new password");
}
