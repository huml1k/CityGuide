using ContentService.Domain.Entities;
using ContentService.Domain.Interfaces.Repositories;
using ContentService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Infrastructure.Repositories
{
    public class RouteRepository : IRouteRepository
    {
        private readonly ContentDbContext _context;

        public RouteRepository(ContentDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Route route, CancellationToken cancellationToken = default)
        {
            await _context.Routes.AddAsync(route, cancellationToken);
        }

        public void Delete(Route route) //soft delete
        {
            route.DeletedAt = DateTime.UtcNow;

            _context.Routes.Update(route);
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Routes.AnyAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyCollection<Route>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Routes.ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyCollection<Route>> GetByCreatorIdAsync(Guid creatorId, CancellationToken cancellationToken = default)
        {
            return await _context.Routes.Where(x => x.CreatorId == creatorId).ToListAsync(cancellationToken);
        }

        public async Task<Route?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Routes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyCollection<Route>> GetByTagAsync(Guid tagId, CancellationToken cancellationToken = default)
        {
            return await _context.RouteTags.Where(x => x.TagId == tagId).Select(x => x.Route).ToListAsync(cancellationToken);
        }

        public async Task<Route?> GetFullRouteByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Routes.Include(x => x.RoutePoints)
                .Include(x => x.RouteImages)
                .Include(x => x.AudioFiles)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyCollection<Route>> SearchByTitleAsync(string search, CancellationToken cancellationToken = default)
        {
            return await _context.Routes.Where(x => x.Title == search).ToListAsync(cancellationToken);
        }

        void IRouteRepository.Update(Route route)
        {
            _context.Routes.Update(route);
        }
    }
}
