using System.Net.Http.Json;
using AuthService.AuthKit.Contracts;

namespace AuthService.AuthKit.Internal;

internal sealed class AuthIntrospectionClient
{
    private readonly HttpClient _httpClient;
    private readonly CityGuideAuthOptions _options;

    public AuthIntrospectionClient(HttpClient httpClient, CityGuideAuthOptions options)
    {
        _httpClient = httpClient;
        _options = options;
    }

    public async Task<IntrospectResponse?> IntrospectAsync(string accessToken, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync(
            _options.IntrospectionPath,
            new IntrospectRequest(accessToken),
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<IntrospectResponse>(cancellationToken: cancellationToken);
    }
}

