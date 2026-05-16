using ContentService.Domain.Entities;
using ContentService.Domain.Interfaces.Repositories;
using ContentService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ContentService.Infrastructure.Repositories
{
    public class RouteImageRepository : IRouteImageRepository
    {
        private readonly ContentDbContext _context;

        public RouteImageRepository(ContentDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(RouteImage image, CancellationToken cancellationToken = default)
        {
            await _context.RouteImages.AddAsync(image, cancellationToken);
        }

        public void Delete(RouteImage image)
        {
            image.DeletedAt = DateTime.UtcNow;

            _context.RouteImages.Update(image);
        }

        public async Task<RouteImage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.RouteImages.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyCollection<RouteImage>> GetByRouteIdAsync(Guid routeId, CancellationToken cancellationToken = default)
        {
            return await _context.RouteImages.Where(x => x.RouteId == routeId).ToListAsync(cancellationToken);
        }
    }
}
