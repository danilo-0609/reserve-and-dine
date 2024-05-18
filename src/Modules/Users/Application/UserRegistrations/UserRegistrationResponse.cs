namespace Users.Application.UserRegistrations;

public sealed record UserRegistrationResponse(
    Guid Id,
    string Login,
    string Email,
    DateTime RegisteredTime);