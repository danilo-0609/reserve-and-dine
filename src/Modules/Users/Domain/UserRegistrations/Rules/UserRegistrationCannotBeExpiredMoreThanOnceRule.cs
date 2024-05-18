using BuildingBlocks.Domain.Rules;
using ErrorOr;

namespace Users.Domain.UserRegistrations.Rules;

internal sealed class UserRegistrationCannotBeExpiredMoreThanOnceRule : IBusinessRule
{
    private readonly UserRegistrationStatus _actualRegistrationStatus;

    public UserRegistrationCannotBeExpiredMoreThanOnceRule(
        UserRegistrationStatus actualRegistrationStatus)
    {
        _actualRegistrationStatus = actualRegistrationStatus;
    }

    public Error Error => UserRegistrationErrorCodes.AlreadyExpired;

    public bool IsBroken() => _actualRegistrationStatus == UserRegistrationStatus.Expired;

    public static string Message => "User registration cannot be expired more than once";
}
