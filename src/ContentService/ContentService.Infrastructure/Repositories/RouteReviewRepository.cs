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
    public class RouteReviewRepository : IRouteReviewRepository
    {
        private readonly ContentDbContext _context;

        public RouteReviewRepository(ContentDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(RouteReview review, CancellationToken cancellationToken = default)
        {
            await _context.RouteReviews.AddAsync(review, cancellationToken);
        }

        public void Delete(RouteReview review)
        {
            _context.RouteReviews.Remove(review);
        }

        public async Task<bool> ExistsAsync(Guid routeId, Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.RouteReviews.AnyAsync(x => x.RouteId == routeId && x.UserId == userId,cancellationToken); 
        }

        public async Task<double> GetAverageRatingAsync(Guid routeId, CancellationToken cancellationToken = default)
        {
            return await _context.RouteReviews.Where(x => x.RouteId == routeId).AverageAsync(x => x.Rating, cancellationToken);
        }

        public async Task<RouteReview?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.RouteReviews.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyCollection<RouteReview>> GetByRouteIdAsync(Guid routeId, CancellationToken cancellationToken = default)
        {
            return await _context.RouteReviews.Where(x => x.RouteId == routeId).ToListAsync(cancellationToken);
        }

        public async Task<int> GetReviewsCountAsync(Guid routeId, CancellationToken cancellationToken = default)
        {
            return await _context.RouteReviews.CountAsync(x => x.RouteId == routeId, cancellationToken);
        }

        public async Task<RouteReview?> GetUserReviewAsync(Guid routeId, Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.RouteReviews.FirstOrDefaultAsync(x => x.UserId == userId && x.RouteId == routeId, cancellationToken); 
        }

        public void Update(RouteReview review)
        {
            _context.RouteReviews.Update(review);
        }
    }
}
