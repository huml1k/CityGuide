using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using UserService.Application.Dtos;
using UserService.Application.Interfaces.Service;
using UserService.Domain.Entities;
using UserService.Domain.Exeptions;
using UserService.Domain.Interfaces;

namespace UserService.Application.Services;

public class UserProfileService : IUserProfileService
{
    private readonly IUserProfilesRepository _repository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<UserProfileService> _logger;

    public UserProfileService(
        IUserProfilesRepository repository,
        IHttpContextAccessor httpContextAccessor,
        ILogger<UserProfileService> logger)
    {
        _repository = repository;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<UserProfileDto> GetMyProfileAsync(CancellationToken ct = default)
    {
        var userId = GetCurrentUserId();
        var profile = await _repository.GetByIdAsync(userId);
        if (profile is null)
        {
            return new UserProfileDto { UserId = userId };
        }

        return MapToDto(profile);
    }

    public async Task<UserProfileDto> GetByIdAsync(Guid userId, CancellationToken ct = default)
    {
        var profile = await _repository.GetByIdAsync(userId)
            ?? throw new NotFoundException($"Profile for user {userId} not found");

        return MapToDto(profile);
    }

    public async Task<Guid> CreateMyProfileAsync(CreateUserProfileRequest request, CancellationToken ct = default)
    {
        var userId = GetCurrentUserId();

        if (await _repository.GetByIdAsync(userId) is not null)
            throw new InvalidOperationException($"Profile for user {userId} already exists");

        var profile = new UserProfile
        {
            UserId = userId,
            FullName = request.FullName,
            AvatarUrl = request.AvatarUrl,
            Bio = request.Bio,
            Phone = request.Phone
        };

        await _repository.CreateAsync(profile);
        _logger.LogInformation("Profile created for user {UserId}", userId);
        return userId;
    }

    public async Task UpdateMyProfileAsync(UpdateUserProfileRequest request, CancellationToken ct = default)
    {
        var userId = GetCurrentUserId();

        var profile = new UserProfile
        {
            UserId = userId,
            FullName = request.FullName,
            AvatarUrl = request.AvatarUrl,
            Bio = request.Bio,
            Phone = request.Phone
        };

        await _repository.UpdateAsync(profile);
        _logger.LogInformation("Profile updated for user {UserId}", userId);
    }

    public async Task DeleteMyProfileAsync(CancellationToken ct = default)
    {
        var userId = GetCurrentUserId();
        await _repository.DeleteAsync(userId);
        _logger.LogInformation("Profile deleted for user {UserId}", userId);
    }

    private static UserProfileDto MapToDto(UserProfile p) =>
        new() { UserId = p.UserId, FullName = p.FullName, AvatarUr = p.AvatarUrl, Bio = p.Bio, Phone = p.Phone };

    private Guid GetCurrentUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        var claim =
            user?.FindFirst(JwtRegisteredClaimNames.Sub)
            ?? user?.FindFirst("sub")
            ?? user?.FindFirst(ClaimTypes.NameIdentifier);

        if (claim is null || !Guid.TryParse(claim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("User is not authenticated or invalid user ID");
        }

        return userId;
    }
}