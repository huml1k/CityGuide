using System.Security.Claims;
using ContentService.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;

namespace ContentService.Application.Features.Routes.Queries.GetMyRoutes;

public sealed class GetMyRoutesHandler : IRequestHandler<GetMyRoutesQuery, IReadOnlyCollection<GetMyRoutesResponse>>
{
    private readonly IRouteRepository _routeRepository;
    private readonly IHttpContextAccessor _accessor;

    public GetMyRoutesHandler(IRouteRepository routeRepository, IHttpContextAccessor accessor)
    {
        _routeRepository = routeRepository;
        _accessor = accessor;
    }

    public async Task<IReadOnlyCollection<GetMyRoutesResponse>> Handle(
        GetMyRoutesQuery request,
        CancellationToken cancellationToken)
    {
        var creatorId = GetCurrentUserId();
        var routes = await _routeRepository.GetByCreatorIdAsync(creatorId, cancellationToken);

        if (routes is null || routes.Count == 0)
        {
            return Array.Empty<GetMyRoutesResponse>();
        }

        return routes
            .Select(route => new GetMyRoutesResponse
            {
                Id = route.Id,
                Title = route.Title,
                Description = route.Description,
                DurationMinutes = route.DurationMinutes,
                Status = route.Status.ToString(),
                GoogleMapsUrl = route.GoogleMapsUrl,
                FavoritesCount = route.RouteStats?.FavoritesCount ?? 0,
                Tags = route.RouteTags?
                    .Select(x => x.Tag.Name)
                    .ToList() ?? new List<string>()
            })
            .ToList();
    }

    private Guid GetCurrentUserId()
    {
        var user = _accessor.HttpContext?.User;
        var userIdClaim =
            user?.FindFirst(JwtRegisteredClaimNames.Sub)
            ?? user?.FindFirst("sub")
            ?? user?.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("User is not authenticated or invalid user ID");
        }

        return userId;
    }
}
