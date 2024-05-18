using API.Configuration;
using API.Modules.Users.Requests;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using Users.Application.UserRegistrations.ConfirmUserRegistration;
using Users.Application.UserRegistrations.GetUserRegistrationById;
using Users.Application.UserRegistrations.RegisterNewUser;

namespace API.Modules.Users.Endpoints;

public class RegistrationsModule : CarterModule
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RegistrationsModule(IHttpContextAccessor httpContextAccessor)
        : base("registrations")
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("register", async ([FromServices] ISender sender, [FromBody] RegisterNewUserRequest request) =>
        {
            var result = await sender.Send(new RegisterNewUserCommand(request.Login,
                request.Email,
                request.Password));

            return result.Match(
                onValue => Results.Ok(onValue),
                onError => new ProblemError(_httpContextAccessor).Errors(onError));
        });

        app.MapPost("confirm/{id}", async ([FromServices] ISender sender, Guid id) =>
        {
            var result = await sender.Send(new ConfirmUserRegistrationCommand(id));

            return result.Match(
                onValue => Results.Created(onValue.ToString(), onValue),
                onError => new ProblemError(_httpContextAccessor).Errors(onError));
        });

        app.MapGet("state/{id}", async ([FromServices] ISender sender, Guid id) =>
        {
            var result = await sender.Send(new GetUserRegistrationByIdQuery(id));

            return result.Match(
                onValue => Results.Ok(onValue),
                onError => new ProblemError(_httpContextAccessor).Errors(onError));
        });

    }
}
