namespace AuthService.AuthKit;

public sealed class CityGuideAuthOptions
{
    public const string SectionName = "CityGuideAuth";

    public string JwtIssuer { get; init; } = "CityGuide.AuthService";
    public string JwtAudience { get; init; } = "CityGuide.Services";
    public string JwtSecret { get; init; } = string.Empty;
    public bool ValidateLifetime { get; init; } = true;
}
