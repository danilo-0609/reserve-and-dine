using Dinners.Application.Common;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Dinners.Application.Restaurants.RestaurantImages.Insert;

internal sealed record InsertRestaurantImagesCommand(Guid Id, IFormFile FormFile, string FilePath) : ICommand<ErrorOr<Unit>>;

