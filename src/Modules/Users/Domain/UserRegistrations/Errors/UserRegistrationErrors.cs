using ErrorOr;
using Users.Domain.UserRegistrations.Rules;

namespace Users.Domain.UserRegistrations.Errors;

public static class UserRegistrationErrors
{
    public static Error LoginIsNotUnique =>
        Error.Validation("UserRegistration.LoginIsNotUnique", UserLoginMustBeUniqueRule.Message);

    public static Error RegistrationIsNotConfirmed =>
        Error.Validation("UserRegistration.NotConfirmedYet", UserCannotBeCreatedWhenRegistrationIsNotConfirmedRule.Message);

    public static Error AlreadyConfirmed =>
       Error.Validation("UserRegistration.ConfirmedAlready", UserRegistrationCannotBeConfirmedMoreThanOnceRule.Message);

    public static Error ConfirmedAfterExpiration =>
        Error.Validation("UserRegistration.ConfirmedAfterExpiration", UserRegistrationCannotBeConfirmedAfterExpirationRule.Message);

    public static Error AlreadyExpired =>
       Error.Validation("UserRegistration.ExpiredAlready", UserRegistrationCannotBeExpiredMoreThanOnceRule.Message);

}
