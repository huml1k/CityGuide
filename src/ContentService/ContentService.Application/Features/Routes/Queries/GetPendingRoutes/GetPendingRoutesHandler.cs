using ContentService.Application.Common.Exceptions;
using ContentService.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Routes.Queries.GetPendingRoutes
{
    public class GetPendingRoutesHandler : IRequestHandler<GetPendingRoutesQuery,IReadOnlyCollection<GetPendingRoutesResponse>>
    {
        private readonly IRouteRepository _routeRepository;
        public GetPendingRoutesHandler(IRouteRepository routeRepository)
        {
            _routeRepository = routeRepository;
        }

        public async Task<IReadOnlyCollection<GetPendingRoutesResponse>> Handle(GetPendingRoutesQuery request, CancellationToken cancellationToken)
        {
            var routes = await _routeRepository.GetPendingModerationAsync(cancellationToken);

            if (routes is null || !routes.Any())
            {
                return Array.Empty<GetPendingRoutesResponse>();
            }

            if (routes.Any(x => x.RouteImages is null))
            {
                throw new BusinessRuleException(
                    "Some routes have missing images.");
            }

            if (routes.Any(x => x.RouteTags is null))
            {
                throw new BusinessRuleException(
                    "Some routes have missing tags.");
            }
            return routes
                .Select(route => new GetPendingRoutesResponse
                {
                    Id = route.Id,
                    Title = route.Title,
                    Description = route.Description,
                    DurationMinutes = route.DurationMinutes,

                    FavoritesCount =
                        route.RouteStats?.FavoritesCount ?? 0,

                    CoverImageExtension = route.RouteImages
                        .FirstOrDefault(x => x.IsCover)?
                        .FileExtension,

                    Tags = route.RouteTags
                        .Select(x => x.Tag.Name)
                        .ToList()
                })
                .ToList();
        }
    }
}
