using ContentService.Application.Common.Exceptions;
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
            var routes = await _routeRepository.GetAllAsync(cancellationToken);

            if (routes is null)
            {
                throw new NotFoundException(
                    "Routes were not found.");
            }

            if (!routes.Any())
            {
                throw new NotFoundException(
                    "Routes collection is empty.");
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
                .Select(route => new GetRoutesResponse
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
