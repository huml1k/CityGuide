namespace UserService.Application.Dtos;

public record UpdateUserProfileRequest
{
    public string? FullName { get; init; }
    public string? AvatarUrl { get; init; }
    public string? Bio { get; init; }
    public string? Phone { get; init; }
}