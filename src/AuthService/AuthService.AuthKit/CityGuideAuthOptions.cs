namespace AuthService.AuthKit;

public sealed class CityGuideAuthOptions
{
    public const string SectionName = "CityGuideAuth";

    public string JwtIssuer { get; init; } = "CityGuide.AuthService";
    public string JwtAudience { get; init; } = "CityGuide.Services";
    public string JwtSecret { get; init; } = string.Empty;
    public bool ValidateLifetime { get; init; } = true;
    public bool EnableIntrospection { get; init; } = true;
    public string IntrospectionBaseUrl { get; init; } = "http://authservice:8080";
    public string IntrospectionPath { get; init; } = "/api/auth/introspect";
}

