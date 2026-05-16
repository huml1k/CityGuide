namespace AuthService.Application.Models;

public sealed record TokenPair(
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    string AccessTokenJti,
    string RefreshToken,
    DateTime RefreshTokenExpiresAtUtc);

