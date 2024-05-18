using Dinners.IntegrationEvents;
using MassTransit;
using Users.Application.Common;
using Users.Domain.Users;

namespace Users.Application.Consumers;

internal sealed class NewRestaurantAdministratorAddedIntegrationEventConsumer : IConsumer<NewRestaurantAdministratorAddedIntegrationEvent>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public NewRestaurantAdministratorAddedIntegrationEventConsumer(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<NewRestaurantAdministratorAddedIntegrationEvent> context)
    {
        var user = await _userRepository.GetByIdAsync(UserId.Create(context.Message.UserId), CancellationToken.None);
    
        if (user is null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        user.AddRole(Role.RestaurantAdministrator);

        await _userRepository.UpdateAsync(user, CancellationToken.None);

        await _unitOfWork.SaveChangesAsync();
    }
}
