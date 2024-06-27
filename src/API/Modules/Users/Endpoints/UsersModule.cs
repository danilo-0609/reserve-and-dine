using API.Configuration;
using API.Modules.Users.Requests;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Users.Application.Abstractions;
using Users.Application.Users.Login;

namespace API.Modules.Users.Endpoints;

public sealed class UsersModule : CarterModule
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UsersModule(IHttpContextAccessor httpContextAccessor)
        : base("auth")
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("login", async ([FromServices] ISender sender, 
            [FromServices] ITokenSetService tokenSetService, 
            [FromBody] LoginRequest request) =>
        {
            var result = await sender.Send(new LoginUserCommand(request.Email, request.Password));

            if (!result.IsError)
            {
                tokenSetService.SetTokenInsideCookie(result.Value, _httpContextAccessor.HttpContext!);
            }

            return result.Match(
                onValue => Results.Ok(),
                onError => new ProblemError(_httpContextAccessor).Errors(onError));
        });
    }
}
