namespace AuthService.Application.Models;

public sealed record AccessTokenPayload(
    Guid UserId,
    string Email,
    string Role,
    string Jti,
    DateTime ExpiresAtUtc);

