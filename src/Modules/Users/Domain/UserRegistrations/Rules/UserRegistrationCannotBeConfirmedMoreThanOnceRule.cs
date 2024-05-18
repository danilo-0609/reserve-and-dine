using BuildingBlocks.Domain.Rules;
using ErrorOr;

namespace Users.Domain.UserRegistrations.Rules;

internal sealed class UserRegistrationCannotBeConfirmedMoreThanOnceRule : IBusinessRule
{
    private readonly UserRegistrationStatus _actualRegistrationStatus;

    public UserRegistrationCannotBeConfirmedMoreThanOnceRule(UserRegistrationStatus userRegistrationStatus)
    {
        _actualRegistrationStatus = userRegistrationStatus;
    }

    public Error Error => UserRegistrationErrorCodes.AlreadyConfirmed;

    public bool IsBroken() => _actualRegistrationStatus == UserRegistrationStatus.Confirmed;

    public static string Message => "User registration cannot be confirmed more than once";
}