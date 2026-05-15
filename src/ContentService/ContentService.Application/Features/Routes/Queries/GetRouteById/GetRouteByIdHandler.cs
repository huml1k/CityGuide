using ContentService.Application.Features.Routes.DTOs;
using ContentService.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Routes.Queries.GetRouteById
{
    public class GetRouteByIdHandler : IRequestHandler<GetRouteByIdQuery, GetRouteByIdResponse>
    {
        private readonly IRouteRepository _routeRepository;

        public GetRouteByIdHandler(IRouteRepository routeRepository)
        {
            _routeRepository = routeRepository;
        }

        public async Task<GetRouteByIdResponse> Handle(GetRouteByIdQuery request, CancellationToken cancellationToken)
        {
            var route = await _routeRepository.GetFullRouteByIdAsync(request.RouteId,cancellationToken);

            if (route is null)
            {
                throw new Exception("Route not found");
            }

            return new GetRouteByIdResponse
            {
                Id = route.Id,
                CreatorId = route.CreatorId,
                Title = route.Title,
                Description = route.Description,
                DurationMinutes = route.DurationMinutes,
                Price = route.Price,
                Status = route.Status,
                GoogleMapsUrl = route.GoogleMapsUrl,
                CreatedAt = route.CreatedAt,

                Points = route.RoutePoints
                    .Select(x => new RoutePointDto
                    {
                        Id = x.Id,
                        Latitude = x.Latitude,
                        Longitude = x.Longitude,
                        OrderIndex = x.OrderIndex,
                        Title = x.Title,
                        Description = x.Description
                    })
                    .ToList(),

                Images = route.RouteImages
                    .Select(x => new RouteImageDto
                    {
                        Id = x.Id,
                        FileExtension = x.FileExtension,
                        IsCover = x.IsCover,
                        OrderIndex = x.OrderIndex
                    })
                    .ToList()
            };
        }
    }
}
