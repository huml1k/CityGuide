namespace AuthService.Application.Models;

public sealed record RefreshSession(
    Guid UserId,
    string Email,
    string Role,
    string RefreshTokenHash,
    DateTime RefreshExpiresAtUtc,
    string AccessTokenJti,
    DateTime AccessExpiresAtUtc);

