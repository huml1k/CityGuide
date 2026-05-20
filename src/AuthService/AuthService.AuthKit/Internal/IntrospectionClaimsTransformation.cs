using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace AuthService.AuthKit.Internal;

internal sealed class IntrospectionClaimsTransformation : IClaimsTransformation
{
    private const string RawAccessTokenClaim = "raw_access_token";

    private readonly AuthIntrospectionClient _client;
    private readonly IOptions<CityGuideAuthOptions> _options;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public IntrospectionClaimsTransformation(
        AuthIntrospectionClient client,
        IOptions<CityGuideAuthOptions> options,
        IHttpContextAccessor httpContextAccessor)
    {
        _client = client;
        _options = options;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var settings = _options.Value;
        if (!settings.EnableIntrospection)
        {
            return principal;
        }

        if (principal.Identity?.IsAuthenticated != true)
        {
            return principal;
        }

        if (principal.HasClaim("introspected", "true"))
        {
            return principal;
        }

        var token = principal.FindFirst(RawAccessTokenClaim)?.Value;
        if (string.IsNullOrWhiteSpace(token))
        {
            token = ExtractBearerToken(_httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString());
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            return principal;
        }

        var payload = await _client.IntrospectAsync(token, CancellationToken.None);
        if (payload is null || !payload.IsValid)
        {
            return new ClaimsPrincipal();
        }

        if (principal.Identity is not ClaimsIdentity identity)
        {
            return principal;
        }

        identity.AddClaim(new Claim("introspected", "true"));
        return principal;
    }

    private static string? ExtractBearerToken(string? authorization)
    {
        if (string.IsNullOrWhiteSpace(authorization) ||
            !authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return authorization["Bearer ".Length..].Trim();
    }
}

