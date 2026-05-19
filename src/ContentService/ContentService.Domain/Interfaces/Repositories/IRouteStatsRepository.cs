using ContentService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Domain.Interfaces.Repositories
{
    public interface IRouteStatsRepository
    {
        Task<RouteStats?> GetByRouteIdAsync(
            Guid routeId,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            RouteStats routeStats,
            CancellationToken cancellationToken = default);

        void Update(RouteStats routeStats);
    }
}
