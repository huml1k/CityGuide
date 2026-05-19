using ContentService.Domain.Entities;
using ContentService.Domain.Interfaces.Repositories;
using ContentService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Infrastructure.Repositories
{
    public class RouteStatsRepository : IRouteStatsRepository
    {
        private readonly ContentDbContext _context;

        public RouteStatsRepository(ContentDbContext context)
        {
            _context = context;
        }

        public async Task<RouteStats?> GetByRouteIdAsync(
            Guid routeId,
            CancellationToken cancellationToken = default)
        {
            return await _context.RouteStats
                .FirstOrDefaultAsync(
                    x => x.RouteId == routeId,
                    cancellationToken);
        }

        public async Task AddAsync(
            RouteStats routeStats,
            CancellationToken cancellationToken = default)
        {
            await _context.RouteStats.AddAsync(
                routeStats,
                cancellationToken);
        }

        public void Update(RouteStats routeStats)
        {
            _context.RouteStats.Update(routeStats);
        }
    }
}
