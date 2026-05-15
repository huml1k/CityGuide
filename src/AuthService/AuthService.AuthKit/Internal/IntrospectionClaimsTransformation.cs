using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace AuthService.AuthKit.Internal;

internal sealed class IntrospectionClaimsTransformation : IClaimsTransformation
{
    private readonly AuthIntrospectionClient _client;
    private readonly IOptions<CityGuideAuthOptions> _options;

    public IntrospectionClaimsTransformation(
        AuthIntrospectionClient client,
        IOptions<CityGuideAuthOptions> options)
    {
        _client = client;
        _options = options;
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

        var token = principal.FindFirst("raw_access_token")?.Value;
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
}

