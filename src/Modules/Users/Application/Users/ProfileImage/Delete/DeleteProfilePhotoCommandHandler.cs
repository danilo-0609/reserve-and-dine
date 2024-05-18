using ErrorOr;
using Users.Application.Abstractions;
using Users.Application.Common;
using Users.Domain.Users;

namespace Users.Application.Users.ProfileImage.Delete;

internal sealed class DeleteProfilePhotoCommandHandler : ICommandHandler<DeleteProfilePhotoCommand, ErrorOr<Success>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserBlobService _userBlobService;

    public DeleteProfilePhotoCommandHandler(IUserBlobService userBlobService, IUserRepository userRepository)
    {
        _userBlobService = userBlobService;
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<Success>> Handle(DeleteProfilePhotoCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(UserId.Create(request.Id), cancellationToken);
        
        if (user is null)
        {
            return UserErrorCodes.NotFound;
        }

        await _userBlobService.DeleteBlobAsync(user.ProfileImageUrl);

        user.DeleteProfileImageUrl();

        await _userRepository.UpdateAsync(user, cancellationToken);

        return new Success();
    }
}
