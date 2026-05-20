using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Routes.Commands.UpdateRoute
{
    public class UpdateRouteCommand : IRequest<UpdateRouteResponse>
    {
        public Guid RouteId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int DurationMinutes { get; set; }

        public string? GoogleMapsUrl { get; set; }
    }
}
