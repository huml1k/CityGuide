using ContentService.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Routes.Queries.SearchRoutes
{
    public class SearchRoutesHandler : IRequestHandler<SearchRoutesQuery, IReadOnlyCollection<SearchRoutesResponse>>
    {
        private readonly IRouteRepository _routeRepository;

        public SearchRoutesHandler (IRouteRepository routeRepository)
        {
            _routeRepository = routeRepository;
        }
        public async Task<IReadOnlyCollection<SearchRoutesResponse>> Handle(SearchRoutesQuery request, CancellationToken cancellationToken)
        {
            var routes = await _routeRepository.SearchAsync(request.Search, cancellationToken);

            return routes
                .Select(route => new SearchRoutesResponse
                {
                    Id = route.Id,
                    Title = route.Title,
                    Description = route.Description,
                    FavoritesCount = route.RouteStats.FavoritesCount,
                    CoverImageExtension = route.RouteImages
                        .FirstOrDefault(x => x.IsCover)?
                        .FileExtension

                })
                .ToList();
        }
    } 
}
