namespace ContentService.Application.Features.Routes.Queries.GetMyRoutes;

public sealed class GetMyRoutesResponse
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int DurationMinutes { get; set; }

    public string Status { get; set; } = string.Empty;

    public string? GoogleMapsUrl { get; set; }

    public int FavoritesCount { get; set; }

    public IReadOnlyCollection<string> Tags { get; set; } = Array.Empty<string>();
}
