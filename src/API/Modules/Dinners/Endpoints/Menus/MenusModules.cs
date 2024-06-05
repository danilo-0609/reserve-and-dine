using API.AuthorizationPolicies.Dinners.Menus.Publish;
using API.Configuration;
using API.Modules.Dinners.Requests.Menus;
using Carter;
using Dinners.Application.Menus.GetById;
using Dinners.Application.Menus.GetByIngredients;
using Dinners.Application.Menus.GetByName;
using Dinners.Application.Menus.GetReviewsByMenuId;
using Dinners.Application.Menus.MenuImages.Add;
using Dinners.Application.Menus.MenuImages.Delete;
using Dinners.Application.Menus.MenuImages.Get;
using Dinners.Application.Menus.MenuSchedules;
using Dinners.Application.Menus.MenuSpecification;
using Dinners.Application.Menus.Publish;
using Dinners.Application.Menus.Review;
using Dinners.Application.Menus.UpdateReview.Comments;
using Dinners.Application.Menus.UpdateReview.Rate;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Modules.Dinners.Endpoints.Menus;

public sealed class MenusModules : CarterModule
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public MenusModules(IHttpContextAccessor httpContextAccessor)
        : base("/menus")
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id}", async (Guid id, [FromServices] ISender sender) =>
        {
            var query = await sender.Send(new GetMenuByIdQuery(id));

            return query.Match(
                onValue => Results.Ok(onValue),
                onError => new ProblemError(_httpContextAccessor).Errors(onError));
        });

        app.MapPost("/", async (
            [FromBody] PublishMenuRequest request, 
            [FromServices] ISender sender, 
            [FromServices] IAuthorizationService authorizationService) =>
        {
            var authorization = await authorizationService.AuthorizeAsync(
                _httpContextAccessor!.HttpContext!.User,
                request.RestaurantId,
                new CanPublishAMenuRequirement());

            if (!authorization.Succeeded)
            {
                return Results.Forbid();
            }

            var command = await sender.Send(new PublishMenuCommand(request.RestaurantId,
                request.Title,
                request.Description,
                request.MenuType,
                request.Price,
                request.Currency,
                request.Discount,
                request.Tags,
                request.IsVegetarian,
                request.PrimaryChefName,
                request.HasAlcohol,
                request.Ingredients,
                request.MainCourse,
                request.SideDishes,
                request.Appetizers,
                request.Beverages,
                request.Desserts,
                request.Sauces,
                request.Condiments,
                request.Coffee,
                request.DiscountTerms));


            return command.Match(
                onValue => Results.Created(onValue.ToString(), onValue),
                onError => new ProblemError(_httpContextAccessor).Errors(onError));
        })
        .RequireAuthorization();

        app.MapGet("/ingredient", async (
            [FromBody] string ingredient, 
            [FromServices] ISender sender) =>
        {
            var query = await sender.Send(new GetMenusByIngredientQuery(ingredient));

            return query.Match(
                onValue => Results.Ok(onValue),
                onError => new ProblemError(_httpContextAccessor).Errors(onError));
        });

        app.MapGet("/name/{name}", async (string name, ISender sender) =>
        {
            var query = await sender.Send(new GetMenusByNameQuery(name));

            return query.Match(
                onValue => Results.Ok(onValue),
                onError => new ProblemError(_httpContextAccessor).Errors(onError));
        });

        app.MapGet("/reviews/{id}", async (Guid id, ISender sender) =>
        {
            var query = await sender.Send(new GetMenuReviewsByMenuIdQuery(id));

            return query.Match(
                onValue => Results.Ok(onValue),
                onError => new ProblemError(_httpContextAccessor).Errors(onError));
        });

        app.MapPut("/images/{id}", async ([FromForm] IFormFile file, Guid id, [FromServices] ISender sender) =>
        {
            var filePath = await GetFilePath(file);

            var command = await sender.Send(new AddMenuImageCommand(id,
                file,
                filePath));

            return command.Match(
                onValue => Results.NoContent(),
                onError => new ProblemError(_httpContextAccessor).Errors(onError));
        });

        static async Task<string> GetFilePath(IFormFile file)
        {
            string filePath = Path.GetTempFileName();

            using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            return filePath;
        }

        app.MapDelete("/images/{id}", async (Guid id, string menuImageUrl, [FromServices] ISender sender) =>
        {
            var command = await sender.Send(new DeleteMenuImageCommand(id, menuImageUrl));

            return command.Match(
                 onValue => Results.NoContent(),
                 onError => new ProblemError(_httpContextAccessor).Errors(onError));
        });

        app.MapGet("/images/{menuId}/{imageId}", async (Guid menuId, Guid imageId, ISender sender) =>
        {
            var query = await sender.Send(new GetMenuImageByIdQuery(menuId, imageId));

            return query.Match(
                onValue => Results.File(onValue.Content!, onValue.ContentType),
                onError => new ProblemError(_httpContextAccessor).Errors(onError));
        });

        app.MapPut("/schedules/{id}", async (Guid id, [FromBody] SetMenuScheduleRequest request, [FromServices] ISender sender) =>
        {
            var command = await sender.Send(new SetMenuScheduleCommand(id,
                request.Day,
                request.Start,
                request.End));

            return command.Match(
                 onValue => Results.NoContent(),
                 onError => new ProblemError(_httpContextAccessor).Errors(onError));
        });

        app.MapPut("/specification/{id}", async (Guid id, [FromBody] UpdateMenuDetailsRequest request, [FromServices] ISender sender) =>
        {
            var command = await sender.Send(new UpdateMenuDetailsCommand(id,
                request.Title,
                request.Description,
                request.MenuType,
                request.DiscountTerms,
                request.Money,
                request.Currency,
                request.Discount,
                request.IsVegetarian,
                request.PrimaryChefName,
                request.HasAlcohol));

            return command.Match(
                 onValue => Results.NoContent(),
                 onError => new ProblemError(_httpContextAccessor).Errors(onError));
        });

        app.MapPost("/reviews/{id}", async (Guid id, [FromBody] ReviewMenuRequest request, [FromServices] ISender sender) =>
        {
            var command = await sender.Send(new ReviewMenuCommand(id, request.Rate, request.Comment));

            return command.Match(
                 onValue => Results.Created(onValue.ToString(), onValue),
                 onError => new ProblemError(_httpContextAccessor).Errors(onError));
        });

        app.MapPut("/reviews/comments/{menuReviewId}", async (Guid menuReviewId, string comment, [FromServices] ISender sender) =>
        {
            var command = await sender.Send(new UpdateReviewCommentCommand(menuReviewId, comment));

            return command.Match(
                 onValue => Results.NoContent(),
                 onError => new ProblemError(_httpContextAccessor).Errors(onError));
        });

        app.MapPut("/reviews/rates/{menuReviewId}", async (Guid menuReviewId, decimal rate, [FromServices] ISender sender) =>
        {
            var command = await sender.Send(new UpdateReviewRateCommand(menuReviewId, rate));

            return command.Match(
                 onValue => Results.NoContent(),
                 onError => new ProblemError(_httpContextAccessor).Errors(onError));
        });
    }
}
