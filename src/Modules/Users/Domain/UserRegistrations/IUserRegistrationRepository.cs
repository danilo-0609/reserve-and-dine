namespace Users.Domain.UserRegistrations;

public interface IUserRegistrationRepository
{
    Task AddAsync(UserRegistration userRegistration);

    Task<UserRegistration?> GetByIdAsync(UserRegistrationId userRegistrationId);

    Task<bool> IsEmailUnique(string email, CancellationToken cancellationToken);

    Task UpdateAsync(UserRegistration userRegistration);
}
