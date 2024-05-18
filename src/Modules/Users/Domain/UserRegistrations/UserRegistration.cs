using BuildingBlocks.Domain.AggregateRoots;
using ErrorOr;
using MediatR;
using Users.Domain.Common;
using Users.Domain.UserRegistrations.Events;
using Users.Domain.UserRegistrations.Rules;
using Users.Domain.Users;

namespace Users.Domain.UserRegistrations;

public sealed class UserRegistration : AggregateRoot<UserRegistrationId, Guid>
{
    public new UserRegistrationId Id { get; private set; }

    public string Login { get; private set; }

    public Password Password { get; private set; }

    public string Email { get; private set; }

    public DateTime RegisteredDate { get; private set; }

    public UserRegistrationStatus Status { get; private set; }

    public DateTime? ConfirmedDate { get; private set; }

    public static ErrorOr<UserRegistration> RegisterNewUser(
        string login,
        string password,
        string email,
        IUsersCounter usersCounter,
        DateTime registeredDate)
    {
        Password hash = Password.CreateUnique(password);

        UserRegistration userRegistration = new UserRegistration(
            UserRegistrationId.CreateUnique(),
            login,
            hash,
            email,
            registeredDate,
            UserRegistrationStatus.WaitingForConfirmation,
            null);

        var isLoginUnique = userRegistration.CheckRule(new UserLoginMustBeUniqueRule(login, usersCounter));

        if (isLoginUnique.IsError)
        {
            return isLoginUnique.FirstError;
        }

        userRegistration.AddDomainEvent(new NewUserRegisteredDomainEvent(
            Guid.NewGuid(),
            userRegistration.Id,
            login,
            email,
            registeredDate));

        return userRegistration;
    }

    public ErrorOr<User> CreateUser()
    {
        var isRegisteredSuccessfully = CheckRule(new UserCannotBeCreatedWhenRegistrationIsNotConfirmedRule(Status));

        if (isRegisteredSuccessfully.IsError)
        {
            return isRegisteredSuccessfully.FirstError;
        }

        return User.CreateUserFromRegistration(
            Id,
            Login,
            Password,
            Email,
            DateTime.UtcNow);
    }

    public ErrorOr<Unit> Confirm()
    {
        var confirmedMoreThanOnce = CheckRule(new UserRegistrationCannotBeConfirmedMoreThanOnceRule(Status));

        if (confirmedMoreThanOnce.IsError)
        {
            return confirmedMoreThanOnce.FirstError;
        }

        var confirmedAfterExpiration = CheckRule(new UserRegistrationCannotBeConfirmedAfterExpirationRule(Status));

        if (confirmedAfterExpiration.IsError)
        {
            return confirmedAfterExpiration.FirstError;
        }

        Status = UserRegistrationStatus.Confirmed;
        ConfirmedDate = DateTime.UtcNow;

        AddDomainEvent(new UserRegistrationConfirmedDomainEvent(
            Guid.NewGuid(),
            Id,
            DateTime.UtcNow));

        return Unit.Value;
    }

    public ErrorOr<Unit> Expired()
    {
        var expiredMoreThanOnce = CheckRule(new UserRegistrationCannotBeExpiredMoreThanOnceRule(Status));

        if (expiredMoreThanOnce.IsError)
        {
            return expiredMoreThanOnce.FirstError;
        }

        Status = UserRegistrationStatus.Expired;

        AddDomainEvent(new UserRegistrationExpiredDomainEvent(
            Guid.NewGuid(),
            Id,
            DateTime.UtcNow));

        return Unit.Value;
    }

    private UserRegistration(
        UserRegistrationId id,
        string login,
        Password password,
        string email,
        DateTime registeredDate,
        UserRegistrationStatus userRegistrationStatus,
        DateTime? confirmedDate)
        : base(id)
    {
        Id = id;
        Login = login;
        Password = password;
        Email = email;
        RegisteredDate = registeredDate;
        Status = userRegistrationStatus;
        ConfirmedDate = confirmedDate;
    }

    private UserRegistration() { }
}
