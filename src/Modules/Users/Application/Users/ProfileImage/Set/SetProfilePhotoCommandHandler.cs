using ErrorOr;
using Users.Application.Abstractions;
using Users.Application.Common;
using Users.Domain.Users;

namespace Users.Application.Users.ProfileImage.Set;

public sealed class SetProfilePhotoCommandHandler : ICommandHandler<SetProfilePhotoCommand, ErrorOr<Success>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserBlobService _userBlobService;

    public SetProfilePhotoCommandHandler(IUserRepository userRepository, IUserBlobService userBlobService)
    {
        _userRepository = userRepository;
        _userBlobService = userBlobService;
    }

    public async Task<ErrorOr<Success>> Handle(SetProfilePhotoCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(UserId.Create(request.Id), cancellationToken);
    
        if (user is null)
        {
            return UserErrorCodes.NotFound;
        }

        string blob = await _userBlobService.UploadFileBlobAsync(request.FilePath, request.FormFile.Name);
    
        user.SetProfileImageUrl(blob);

        await _userRepository.UpdateAsync(user, cancellationToken);

        return new Success();
    }
}
