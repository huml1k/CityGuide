using ContentService.Application.Common.Exceptions;
using ContentService.Application.DTOs;
using ContentService.Application.Features.Routes.DTOs;
using ContentService.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Routes.Queries.GetPendingRouteById
{
    public class GetPendingRouteByIdHandler : IRequestHandler<GetPendingRouteByIdQuery, GetPendingRouteByIdResponse>
    {
        private readonly IRouteRepository _routeRepository;

        public GetPendingRouteByIdHandler(IRouteRepository routeRepository)
        {
            _routeRepository = routeRepository;
        }

        public async Task<GetPendingRouteByIdResponse> Handle(GetPendingRouteByIdQuery request, CancellationToken cancellationToken)
        {
            var route = await _routeRepository.GetPendingModerationByIdAsync(request.RouteId,cancellationToken);

            if (route is null)
            {
                throw new RouteNotFoundException(request.RouteId);
            }

            if (route.DeletedAt.HasValue)
            {
                throw new BusinessRuleException(
                    "Route has been deleted.");
            }

            if (route.RoutePoints is null)
            {
                throw new BusinessRuleException(
                    "Route points are missing.");
            }

            if (route.RouteImages is null)
            {
                throw new BusinessRuleException(
                    "Route images are missing.");
            }

            if (route.AudioFiles is null)
            {
                throw new BusinessRuleException(
                    "Route audio files are missing.");
            }

            return new GetPendingRouteByIdResponse
            {
                Id = route.Id,
                CreatorId = route.CreatorId,
                Title = route.Title,
                Description = route.Description,
                DurationMinutes = route.DurationMinutes,
                GoogleMapsUrl = route.GoogleMapsUrl,
                CreatedAt = route.CreatedAt,

                FavoritesCount =
                    route.RouteStats?.FavoritesCount ?? 0,

                Tags = route.RouteTags
                    .Select(x => x.Tag.Name)
                    .ToList(),
                
                AudioFiles = route.AudioFiles
                    .Select(x => new AudioFileDto
                    {
                        Id = x.Id,
                        FileExtension = x.FileExtension,
                        DurationSeconds = x.DurationSeconds,
                        OrderIndex = x.OrderIndex,
                        OriginalFilename = x.OriginalFilename
                    })
                    .ToList(),


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
