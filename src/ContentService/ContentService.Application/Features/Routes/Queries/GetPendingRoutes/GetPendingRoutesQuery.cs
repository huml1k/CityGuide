using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Routes.Queries.GetPendingRoutes
{
    public class GetPendingRoutesQuery : IRequest<IReadOnlyCollection<GetPendingRoutesResponse>>
    {
    }
}
