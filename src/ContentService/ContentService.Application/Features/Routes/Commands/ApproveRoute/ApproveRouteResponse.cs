using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Routes.Commands.ApproveRoute
{
    public class ApproveRouteResponse
    {
        public Guid RouteId { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
