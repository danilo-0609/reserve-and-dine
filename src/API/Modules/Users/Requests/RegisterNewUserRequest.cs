namespace API.Modules.Users.Requests;

public record RegisterNewUserRequest(string Login,
    string Email,
    string Password);
