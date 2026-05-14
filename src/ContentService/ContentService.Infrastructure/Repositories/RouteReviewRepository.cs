using ContentService.Domain.Entities;
using ContentService.Domain.Interfaces.Repositories;
using ContentService.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
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

        public Task AddAsync(RouteReview review, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void Delete(RouteReview review)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Guid routeId, Guid userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<double> GetAverageRatingAsync(Guid routeId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<RouteReview?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<RouteReview>> GetByRouteIdAsync(Guid routeId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetReviewsCountAsync(Guid routeId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<RouteReview?> GetUserReviewAsync(Guid routeId, Guid userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void Update(RouteReview review)
        {
            throw new NotImplementedException();
        }
    }
}
