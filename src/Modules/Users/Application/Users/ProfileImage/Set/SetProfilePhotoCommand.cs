using ErrorOr;
using Microsoft.AspNetCore.Http;
using Users.Application.Common;

namespace Users.Application.Users.ProfileImage.Set;

public sealed record SetProfilePhotoCommand(Guid Id, IFormFile FormFile, string FilePath) : ICommand<ErrorOr<Success>>;
