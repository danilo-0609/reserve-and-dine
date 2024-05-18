using Users.Domain.Users;

namespace Users.Application.Abstractions;

public interface IJwtProvider
{
    string Generate(User user);
}
