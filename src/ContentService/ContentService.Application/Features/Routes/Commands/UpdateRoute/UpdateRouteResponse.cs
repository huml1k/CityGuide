using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Routes.Commands.UpdateRoute
{
    public class UpdateRouteResponse
    {
        public Guid Id { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
