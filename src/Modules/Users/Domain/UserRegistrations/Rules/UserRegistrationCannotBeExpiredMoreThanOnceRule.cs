using BuildingBlocks.Domain.Rules;
using ErrorOr;
using Users.Domain.UserRegistrations.Errors;

namespace Users.Domain.UserRegistrations.Rules;

internal sealed class UserRegistrationCannotBeExpiredMoreThanOnceRule : IBusinessRule
{
    private readonly UserRegistrationStatus _actualRegistrationStatus;

    public UserRegistrationCannotBeExpiredMoreThanOnceRule(
        UserRegistrationStatus actualRegistrationStatus)
    {
        _actualRegistrationStatus = actualRegistrationStatus;
    }

    public Error Error => UserRegistrationErrors.AlreadyExpired;

    public bool IsBroken() => _actualRegistrationStatus == UserRegistrationStatus.Expired;

    public static string Message => "User registration cannot be expired more than once";
}
