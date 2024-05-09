namespace Users.Application.Users;

public sealed record UserResponse(
    Guid UserId,
    string Login, 
    string Email,
    DateTime registeredDate);
