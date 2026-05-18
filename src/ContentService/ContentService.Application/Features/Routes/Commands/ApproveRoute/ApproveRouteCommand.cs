using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Routes.Commands.ApproveRoute
{

    public record ApproveRouteCommand(Guid RouteId) : IRequest<ApproveRouteResponse>;
}
