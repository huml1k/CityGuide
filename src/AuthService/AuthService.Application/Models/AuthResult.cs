namespace AuthService.Application.Models;

public sealed record AuthResult(
    Guid UserId,
    string Email,
    string Role,
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    string RefreshToken,
    DateTime RefreshTokenExpiresAtUtc);

