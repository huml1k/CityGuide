namespace AdminService.Application.Dtos;

public sealed class HealthCheckResponse
{
    public required string Service { get; init; }

    public required string Status { get; init; }

    public DateTime Timestamp { get; init; }

    public required ComponentHealthCheck Database { get; init; }

    public IReadOnlyList<DependencyHealthCheck> Dependencies { get; init; } = [];
}

public sealed class ComponentHealthCheck
{
    public required string Status { get; init; }

    public string? Error { get; init; }
}

public sealed class DependencyHealthCheck
{
    public required string Name { get; init; }

    public required string BaseUrl { get; init; }

    public required string Status { get; init; }

    public int? ResponseTimeMs { get; init; }

    public string? Error { get; init; }
}
