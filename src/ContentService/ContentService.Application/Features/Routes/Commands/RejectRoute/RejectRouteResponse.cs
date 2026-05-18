using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Routes.Commands.RejectRoute
{
    public class RejectRouteResponse
    {
        public Guid RouteId { get; set; }

        public string Status { get; set; } = string.Empty;

    }
}
