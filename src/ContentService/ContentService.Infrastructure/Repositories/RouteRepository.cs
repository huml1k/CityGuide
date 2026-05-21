using ContentService.Domain.Entities;
using ContentService.Domain.Enums;
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
            return await _context.Routes.Include(x => x.RouteStats).Include(x => x.RouteImages).ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyCollection<Route>> GetByCreatorIdAsync(Guid creatorId, CancellationToken cancellationToken = default)
        {
            return await _context.Routes
                .AsNoTracking()
                .Where(x => x.CreatorId == creatorId && x.DeletedAt == null)
                .Include(x => x.RouteStats)
                .Include(x => x.RouteImages)
                .Include(x => x.RouteTags)
                    .ThenInclude(x => x.Tag)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<Route?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Routes.Include(r => r.RouteTags).FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
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
                .Include(x => x.RouteStats)
                .Include(x => x.RouteTags)
                    .ThenInclude(rt => rt.Tag)
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

        public async Task<IReadOnlyCollection<Route>> SearchAsync(string search, CancellationToken cancellationToken = default)
        {
            return await _context.Routes
                .AsNoTracking()
                .Include(x => x.RouteStats)
                .Include(x => x.RouteImages)
                .Include(x => x.RouteTags)
                    .ThenInclude(x => x.Tag)
                .Where(x =>
                    x.Title.Contains(search) ||
                    x.RouteTags.Any(t =>
                        t.Tag.Name.Contains(search)))
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyCollection<Route>> GetApprovedAsync( CancellationToken cancellationToken)
        {
            return await _context.Routes
                .Include(x => x.RouteStats)
                .Include(x => x.RouteImages)
                .Include(x => x.RouteTags)
                    .ThenInclude(x => x.Tag)
                .Where(x => x.Status == RouteStatus.approved)
                .ToListAsync(cancellationToken);
        }

        public async Task<Route?> GetApprovedByIdAsync(Guid routeId, CancellationToken cancellationToken)
        {
            return await _context.Routes
                .Include(x => x.RouteStats)
                .Include(x => x.RouteImages)
                .Include(x => x.RoutePoints)
                .Include(x => x.AudioFiles)
                .Include(x => x.RouteTags)
                    .ThenInclude(x => x.Tag)
                .FirstOrDefaultAsync(
                    x => x.Id == routeId &&
                         x.Status == RouteStatus.approved,
                    cancellationToken);
        }

        public async Task<IReadOnlyCollection<Route>> GetPendingModerationAsync(CancellationToken cancellationToken)
        {
            return await _context.Routes
                .Include(x => x.RouteStats)
                .Include(x => x.RouteImages)
                .Include(x => x.RouteTags)
                    .ThenInclude(x => x.Tag)
                .Where(x => x.Status == RouteStatus.pendingModeration)
                .ToListAsync(cancellationToken);
        }

        public async Task<Route?> GetPendingModerationByIdAsync(Guid routeId, CancellationToken cancellationToken)
        {
            return await _context.Routes
                .Include(x => x.RouteStats)
                .Include(x => x.RouteImages)
                .Include(x => x.RoutePoints)
                .Include(x => x.AudioFiles)
                .Include(x => x.RouteTags)
                    .ThenInclude(x => x.Tag)
                .FirstOrDefaultAsync(
                    x => x.Id == routeId &&
                         x.Status == RouteStatus.pendingModeration,
                    cancellationToken);
        }
    }
}
