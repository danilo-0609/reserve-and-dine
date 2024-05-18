using ErrorOr;
using Users.Domain.UserRegistrations.Rules;

namespace Users.Domain.UserRegistrations;

public static class UserRegistrationErrorCodes
{
    public static Error LoginIsNotUnique =>
        Error.Validation("UserRegistration.LoginIsNotUnique", UserLoginMustBeUniqueRule.Message);

    public static Error EmailIsNotUnique =>
        Error.Validation("UserRegistration.EmailIsNotUnique", "Email must be unique");

    public static Error RegistrationIsNotConfirmed =>
        Error.Validation("UserRegistration.NotConfirmedYet", UserCannotBeCreatedWhenRegistrationIsNotConfirmedRule.Message);

    public static Error AlreadyConfirmed =>
       Error.Validation("UserRegistration.ConfirmedAlready", UserRegistrationCannotBeConfirmedMoreThanOnceRule.Message);

    public static Error ConfirmedAfterExpiration =>
        Error.Validation("UserRegistration.ConfirmedAfterExpiration", UserRegistrationCannotBeConfirmedAfterExpirationRule.Message);

    public static Error AlreadyExpired =>
       Error.Validation("UserRegistration.ExpiredAlready", UserRegistrationCannotBeExpiredMoreThanOnceRule.Message);

    public static Error NotFound =>
        Error.NotFound("UserRegistration.NotFound", "User registration was not found");
}
