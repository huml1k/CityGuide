using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Routes.Commands.RejectRoute
{
    public record RejectRouteCommand(Guid RouteId) : IRequest<RejectRouteResponse>;
}
