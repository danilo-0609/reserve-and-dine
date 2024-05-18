using Microsoft.AspNetCore.Http;

namespace Users.Application.Abstractions;

public interface ITokenSetService
{
    void SetTokenInsideCookie(string token, HttpContext context);
}
