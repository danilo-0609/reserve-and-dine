using ErrorOr;
using Users.Application.Common;

namespace Users.Application.UserRegistrations.ConfirmUserRegistration;

public sealed record ConfirmUserRegistrationCommand(Guid Id) : ICommand<ErrorOr<Success>>;
