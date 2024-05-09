using ErrorOr;
using Users.Application.Common;

namespace Users.Application.Users.GetUserById;

internal sealed record GetUserByIdQuery(Guid Id) : IQuery<ErrorOr<UserResponse>>;
