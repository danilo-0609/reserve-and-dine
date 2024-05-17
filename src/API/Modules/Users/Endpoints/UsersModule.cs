using Carter;

namespace API.Modules.Users.Endpoints;

public class UsersModule : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("login", () =>
        {
            return Results.Ok();
        });
    }
}
