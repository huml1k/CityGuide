using AuthService.Application.Models;

namespace AuthService.Api.Contracts;

public sealed record AuthResponse(
    Guid UserId,
    string Email,
    string Role,
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    string RefreshToken,
    DateTime RefreshTokenExpiresAtUtc)
{
    public static AuthResponse FromResult(AuthResult result) =>
        new(
            result.UserId,
            result.Email,
            result.Role,
            result.AccessToken,
            result.AccessTokenExpiresAtUtc,
            result.RefreshToken,
            result.RefreshTokenExpiresAtUtc);
}

public sealed record IntrospectResponse(
    bool IsValid,
    Guid UserId,
    string Email,
    string Role,
    DateTime ExpiresAtUtc);

