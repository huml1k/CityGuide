using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Routes.Queries.GetRouteById
{
    public class GetRouteByIdQuery : IRequest<GetRouteByIdResponse>
    {
        public Guid RouteId { get; set; }
    }
}
