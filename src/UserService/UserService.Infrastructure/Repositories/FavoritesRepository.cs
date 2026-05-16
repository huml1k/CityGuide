using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;
using UserService.Infrastructure.Persistence;

namespace UserService.Infrastructure.Repositories;

public class FavoritesRepository : IFavoritesRepository
{
    private UserDbContext _context;
    
    public FavoritesRepository(UserDbContext context)
    {
        _context = context;
    }
    
    public async Task<IReadOnlyCollection<Guid>> GetUserFavoritesAsync(Guid userId, CancellationToken ct = default)
    {
        return await _context.Favorites.Where(f => f.UserId == userId).Select(x => x.Id).ToListAsync(ct);
    }

    public async Task<IReadOnlyCollection<Guid>> GetRouteFavoritesAsync(Guid routeId,  CancellationToken ct = default)
    {
        return await _context.Favorites.Where(f => f.RouteId == routeId).Select(x => x.Id).ToListAsync(ct);
    }

    public async Task AddFavoriteAsync(Favorite favorite, CancellationToken ct = default(CancellationToken))
    {
        _context.Favorites.Add(favorite);
        await _context.SaveChangesAsync(ct);
    }

    public void Delete(Favorite favorite)
    {
        _context.Favorites.Remove(favorite);
    }

    public async Task<bool> IsFavoriteAsync(Guid  routeId, Guid userId, CancellationToken ct = default(CancellationToken))
    {
        return await _context.Favorites.AnyAsync(x => x.RouteId == routeId && x.UserId == userId, ct);
    }

    public async Task<IReadOnlyCollection<Favorite>> GetAllFavoritesAsync()
    {
        return await _context.Favorites.ToListAsync();
    }

    public async Task<Favorite> GetByUserAndRouteAsync(Guid routeId, Guid userId, CancellationToken ct = default)
    {
        return await _context.Favorites.Where(f => f.RouteId == routeId && f.UserId == userId).FirstAsync(ct);
    }
}