using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Routes.Queries.GetPendingRouteById
{
    public class GetPendingRouteByIdQuery : IRequest<GetPendingRouteByIdResponse>
    {
        public Guid RouteId { get; set; }
    }
}
