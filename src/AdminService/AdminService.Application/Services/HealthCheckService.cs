using System.Diagnostics;
using AdminService.Application.Dtos;
using AdminService.Application.Interfaces;
using AdminService.Application.Options;
using Microsoft.Extensions.Options;

namespace AdminService.Application.Services;

public sealed class HealthCheckService : IHealthCheckService
{
    private const string Healthy = "Healthy";
    private const string Unhealthy = "Unhealthy";
    private const string Degraded = "Degraded";

    private static readonly (string Name, Func<CityGuideServicesOptions, string> Url, string? HealthPath)[] Dependencies =
    [
        ("AuthService", o => o.AuthService, null),
        ("UserService", o => o.UserService, null),
        ("ContentService", o => o.ContentService, null),
        ("NotificationService", o => o.NotificationService, "/api/notifications/healthCheck"),
        ("ApiGateway", o => o.ApiGateway, null),
    ];

    private readonly IDatabaseHealthChecker _databaseHealthChecker;
    private readonly HttpClient _httpClient;
    private readonly CityGuideServicesOptions _servicesOptions;

    public HealthCheckService(
        IDatabaseHealthChecker databaseHealthChecker,
        HttpClient httpClient,
        IOptions<CityGuideServicesOptions> servicesOptions)
    {
        _databaseHealthChecker = databaseHealthChecker;
        _httpClient = httpClient;
        _servicesOptions = servicesOptions.Value;
    }

    public async Task<HealthCheckResponse> CheckAsync(CancellationToken cancellationToken = default)
    {
        var databaseHealthy = await _databaseHealthChecker.CanConnectAsync(cancellationToken);

        var dependencies = new List<DependencyHealthCheck>();
        foreach (var (name, urlSelector, healthPath) in Dependencies)
        {
            dependencies.Add(await CheckDependencyAsync(
                name,
                urlSelector(_servicesOptions),
                healthPath,
                cancellationToken));
        }

        var anyDependencyUnhealthy = dependencies.Any(d => d.Status == Unhealthy);
        var overallStatus = databaseHealthy
            ? anyDependencyUnhealthy ? Degraded : Healthy
            : Unhealthy;

        return new HealthCheckResponse
        {
            Service = "AdminService",
            Status = overallStatus,
            Timestamp = DateTime.UtcNow,
            Database = new ComponentHealthCheck
            {
                Status = databaseHealthy ? Healthy : Unhealthy,
                Error = databaseHealthy ? null : "Unable to connect to the database.",
            },
            Dependencies = dependencies,
        };
    }

    private async Task<DependencyHealthCheck> CheckDependencyAsync(
        string name,
        string baseUrl,
        string? healthPath,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            return new DependencyHealthCheck
            {
                Name = name,
                BaseUrl = baseUrl,
                Status = Unhealthy,
                Error = "Service URL is not configured.",
            };
        }

        var requestUri = BuildRequestUri(baseUrl, healthPath);
        var stopwatch = Stopwatch.StartNew();

        try
        {
            using var response = await _httpClient.GetAsync(requestUri, cancellationToken);
            stopwatch.Stop();

            return new DependencyHealthCheck
            {
                Name = name,
                BaseUrl = baseUrl.TrimEnd('/'),
                Status = Healthy,
                ResponseTimeMs = (int)stopwatch.ElapsedMilliseconds,
                Error = response.IsSuccessStatusCode
                    ? null
                    : $"Responded with HTTP {(int)response.StatusCode} ({response.StatusCode}).",
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            return new DependencyHealthCheck
            {
                Name = name,
                BaseUrl = baseUrl.TrimEnd('/'),
                Status = Unhealthy,
                ResponseTimeMs = (int)stopwatch.ElapsedMilliseconds,
                Error = ex.Message,
            };
        }
    }

    private static string BuildRequestUri(string baseUrl, string? healthPath)
    {
        var normalizedBase = baseUrl.TrimEnd('/');
        return string.IsNullOrWhiteSpace(healthPath)
            ? normalizedBase
            : $"{normalizedBase}/{healthPath.TrimStart('/')}";
    }
}
