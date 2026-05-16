namespace UserService.Domain.Interfaces.Service;


public interface IUserProfileService
{
    Task<UserProfileDto> GetMyProfileAsync(CancellationToken ct = default);
    Task<UserProfileDto> GetByIdAsync(Guid userId, CancellationToken ct = default);
    Task<Guid> CreateMyProfileAsync(CreateUserProfileRequest request, CancellationToken ct = default);
    Task UpdateMyProfileAsync(UpdateUserProfileRequest request, CancellationToken ct = default);
    Task DeleteMyProfileAsync(CancellationToken ct = default);
}