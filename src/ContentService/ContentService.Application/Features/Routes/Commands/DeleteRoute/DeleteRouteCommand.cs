using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Routes.Commands.DeleteRoute
{
    public class DeleteRouteCommand : IRequest
    {
        public Guid RouteId { get; set; }
    }
}
