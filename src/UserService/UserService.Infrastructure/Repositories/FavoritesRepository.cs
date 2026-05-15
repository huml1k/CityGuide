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
    
    public async Task<IReadOnlyCollection<Favorite>> GetUserFavoritesAsync(Guid userId)
    {
        return await _context.Favorites.Where(f => f.UserId == userId).ToListAsync();
    }

    public async Task<IReadOnlyCollection<Favorite>> GetRouteFavoritesAsync(Guid routeId)
    {
        return await _context.Favorites.Where(f => f.RouteId == routeId).ToListAsync();
    }

    public async Task AddFavoriteAsync(Favorite favorite)
    {
        _context.Favorites.Add(favorite);
        await _context.SaveChangesAsync();
    }

    public void Delete(Favorite favorite)
    {
        _context.Favorites.Remove(favorite);
    }

    public async Task<bool> IsFavoriteAsync(Guid  routeId, Guid userId)
    {
        return await _context.Favorites.AnyAsync(x => x.RouteId == routeId && x.UserId == userId);
    }

    public async Task<IReadOnlyCollection<Favorite>> GetAllFavoritesAsync()
    {
        return await _context.Favorites.ToListAsync();
    }
}