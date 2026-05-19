using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using UserService.Application.Interfaces.Clients;
using UserService.Application.Interfaces.Service;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;

namespace UserService.Application;

public class FavoriteService : IFavoriteService
{
    private readonly IFavoritesRepository _favoriteRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IContentServiceClient _contentServiceClient;
    private readonly ILogger<FavoriteService> _logger;
    
    public FavoriteService(
        IFavoritesRepository favoriteRepository,
        IHttpContextAccessor httpContextAccessor,
        IContentServiceClient contentServiceClient,
        ILogger<FavoriteService> logger)
    {
        _favoriteRepository = favoriteRepository;
        _httpContextAccessor = httpContextAccessor;
        _contentServiceClient = contentServiceClient;
        _logger = logger;
    }
    public async Task<Guid> AddFavoriteAsync(Guid routeId, CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        
        if (await _favoriteRepository.IsFavoriteAsync(userId, routeId, cancellationToken))
        {
            throw new InvalidOperationException($"Route {routeId} is already in favorites for user {userId}");
        }
        
        var favorite = new Favorite
        {
            UserId = userId, 
            RouteId = routeId
        };
        
        await _favoriteRepository.AddFavoriteAsync(favorite, cancellationToken);

        await _contentServiceClient.IncrementFavoritesAsync(routeId);

        _logger.LogInformation($"User {userId} added route {routeId} to favorites", userId, routeId);
        
        return favorite.Id;
    }

    public async Task RemoveFavoriteAsync(Guid routeId, CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        
        var favorite = await _favoriteRepository.GetByUserAndRouteAsync(userId, routeId, cancellationToken);
        
        if (favorite == null)
        {
            _logger.LogWarning($"Favorite not found for user {userId} and route {routeId}", userId, routeId);
            return;
        }
        
        _favoriteRepository.Delete(favorite);
        //await _favoriteRepository.SaveChangesAsync(cancellationToken);

        await _contentServiceClient.DecrementFavoritesAsync(routeId);

        _logger.LogInformation($"User {userId} removed route {routeId} from favorites", userId, routeId);
    }

    public async Task<bool> IsFavoriteAsync(Guid routeId, CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await _favoriteRepository.IsFavoriteAsync(userId, routeId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Guid>> GetUserFavoriteRouteIdsAsync(CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await _favoriteRepository.GetUserFavoritesAsync(userId, cancellationToken);
    }

    public async Task<Dictionary<Guid, bool>> CheckMultipleFavoritesAsync(IEnumerable<Guid> routeIds, CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        var distinctRouteIds = routeIds.Distinct().ToList();
        
        var favoriteRouteIds = await _favoriteRepository.GetUserFavoritesAsync(userId, cancellationToken);
        var favoriteSet = new HashSet<Guid>(favoriteRouteIds);
        
        return distinctRouteIds.ToDictionary(routeId => routeId, routeId => favoriteSet.Contains(routeId));
    }
    
    private Guid GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("sub");
        
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("User is not authenticated or invalid user ID");
        }
        
        return userId;
    }
}