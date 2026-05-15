using ContentService.Application.DTOs;
using ContentService.Application.Features.Routes.DTOs;
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

        public decimal Price { get; set; }

        public string Status { get; set; } = string.Empty;

        public string? GoogleMapsUrl { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<RoutePointDto> Points { get; set; } = [];

        public List<RouteImageDto> Images { get; set; } = [];
    }
}
