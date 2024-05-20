using BuildingBlocks.Domain.Rules;
using ErrorOr;

namespace Users.Domain.UserRegistrations.Rules;

internal class UserCannotBeCreatedWhenRegistrationIsNotConfirmedRule : IBusinessRule
{
    private readonly UserRegistrationStatus _actualRegistrationStatus;

    public UserCannotBeCreatedWhenRegistrationIsNotConfirmedRule(
            UserRegistrationStatus userRegistrationStatus)
    {
        _actualRegistrationStatus = userRegistrationStatus;
    }

    public Error Error => UserRegistrationErrorCodes.RegistrationIsNotConfirmed;

    public bool IsBroken() => _actualRegistrationStatus.Value != UserRegistrationStatus.Confirmed.Value;

    public static string Message => "User cannot be created when registration is not confirmed";
}