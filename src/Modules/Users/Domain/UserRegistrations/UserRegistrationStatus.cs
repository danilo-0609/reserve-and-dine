namespace Users.Domain.UserRegistrations;

public sealed class UserRegistrationStatus
{
    public static UserRegistrationStatus WaitingForConfirmation =>
        new UserRegistrationStatus(nameof(WaitingForConfirmation));

    public static UserRegistrationStatus Confirmed =>
        new UserRegistrationStatus(nameof(Confirmed));

    public static UserRegistrationStatus Expired =>
        new UserRegistrationStatus(nameof(Expired));

    public string Value { get; private set; }

    public UserRegistrationStatus(string value)
    {
        Value = value;
    }
}
