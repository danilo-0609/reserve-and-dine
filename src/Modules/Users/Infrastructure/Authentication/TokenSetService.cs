using Microsoft.AspNetCore.Http;
using Users.Application.Abstractions;

namespace Users.Infrastructure.Authentication;

internal sealed class TokenSetService : ITokenSetService
{
    public void SetTokenInsideCookie(string token, HttpContext context)
    {
        context.Response.Cookies.Append("accessToken", token,
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddMinutes(5),
                HttpOnly = true,
                IsEssential = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });
    }
}
