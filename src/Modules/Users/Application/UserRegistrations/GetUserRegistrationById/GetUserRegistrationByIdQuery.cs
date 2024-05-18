using ErrorOr;
using Users.Application.Common;

namespace Users.Application.UserRegistrations.GetUserRegistrationById;

public sealed record GetUserRegistrationByIdQuery(Guid Id) : ICommand<ErrorOr<UserRegistrationResponse>>;
