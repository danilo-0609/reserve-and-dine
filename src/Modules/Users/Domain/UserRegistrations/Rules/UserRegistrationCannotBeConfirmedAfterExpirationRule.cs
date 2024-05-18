using BuildingBlocks.Domain.Rules;
using ErrorOr;

namespace Users.Domain.UserRegistrations.Rules;

internal sealed class UserRegistrationCannotBeConfirmedAfterExpirationRule : IBusinessRule
{
    private readonly UserRegistrationStatus _actualRegistrationStatus;

    public UserRegistrationCannotBeConfirmedAfterExpirationRule(
        UserRegistrationStatus userRegistrationStatus)
    {
        _actualRegistrationStatus = userRegistrationStatus;
    }

    public Error Error => UserRegistrationErrorCodes.ConfirmedAfterExpiration;

    public bool IsBroken() => _actualRegistrationStatus == UserRegistrationStatus.Expired;

    public static string Message => "User registration cannot be confirmed after expiration";
}
