using ContentService.Application.DTOs;
using ContentService.Application.Features.Routes.DTOs;
using ContentService.Application.Features.Tags.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Routes.Queries.GetRouteById
{
    public class GetRouteByIdResponse
    {
        public Guid Id { get; set; }

        public Guid CreatorId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int DurationMinutes { get; set; }

        public int FavoritesCount { get; set; }

        public string? GoogleMapsUrl { get; set; }

        public IReadOnlyCollection<AudioFileDto> AudioFiles { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<RoutePointDto> Points { get; set; } = [];

        public List<RouteImageDto> Images { get; set; } = [];

        public IReadOnlyCollection<TagDto> Tags { get; set; } = [];
    }
}
