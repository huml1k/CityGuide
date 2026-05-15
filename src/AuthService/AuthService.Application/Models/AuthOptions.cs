namespace AuthService.Application.Models;

public sealed class AuthOptions
{
    public const string SectionName = "Auth";

    public string JwtIssuer { get; init; } = "CityGuide.AuthService";
    public string JwtAudience { get; init; } = "CityGuide.Services";
    public string JwtSecret { get; init; } = string.Empty;
    public int AccessTokenMinutes { get; init; } = 15;
    public int RefreshTokenDays { get; init; } = 7;
}

