using ContentService.Application.Common.Exceptions;
using ContentService.Application.Features.Routes;
using ContentService.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Routes.Queries.GetRoutes
{
    public class GetRoutesHandler : IRequestHandler<GetRoutesQuery,IReadOnlyCollection<GetRoutesResponse>>
    {
        private readonly IRouteRepository _routeRepository;
        public GetRoutesHandler(IRouteRepository routeRepository)
        {
            _routeRepository = routeRepository;
        }

        public async Task<IReadOnlyCollection<GetRoutesResponse>> Handle(GetRoutesQuery request, CancellationToken cancellationToken)
        {
            var routes = await _routeRepository.GetApprovedAsync(cancellationToken);

            if (routes is null || !routes.Any())
            {
                return Array.Empty<GetRoutesResponse>();
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
                .Select(route =>
                {
                    var (coverId, coverExt) = RouteCoverImageMapper.GetCover(route.RouteImages);
                    return new GetRoutesResponse
                    {
                        Id = route.Id,
                        Title = route.Title,
                        Description = route.Description,
                        DurationMinutes = route.DurationMinutes,
                        FavoritesCount = route.RouteStats?.FavoritesCount ?? 0,
                        CoverImageId = coverId,
                        CoverImageExtension = coverExt,
                        Tags = route.RouteTags
                            .Select(x => x.Tag.Name)
                            .ToList()
                    };
                })
                .ToList();
        }
    }
}
