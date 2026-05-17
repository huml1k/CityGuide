namespace UserService.Application.Dtos;

public record UserProfileDto
{
    public Guid UserId { get; init; }
    public string? FullName { get; init; }
    public string? AvatarUr { get; init; }
    public string? Bio { get; init; }
    public string? Phone { get; init; }
}