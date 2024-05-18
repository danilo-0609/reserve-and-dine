using ErrorOr;
using Users.Application.Common;

namespace Users.Application.Users.ProfileImage.Delete;

public sealed record DeleteProfilePhotoCommand(Guid Id) : ICommand<ErrorOr<Success>>;
